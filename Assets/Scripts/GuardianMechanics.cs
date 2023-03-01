using System.Collections;
using UnityEngine;

public class GuardianMechanics : AttackingEnemyMechanics {
    private static readonly int Teleporting = Animator.StringToHash("Teleport");
    
    private const float MIN_DISTANCE_TELEPORT = 2f;
    private const float MAX_DISTANCE_TELEPORT = 10f;
    // [SerializeField] float TOLERANCE = 1f;
    
    RaycastHit2D raycastHitMax;
    RaycastHit2D raycastHitMin;
    
    // Boundaries
    private float maxPosition;
    private float minPosition;
    
    // Laser beam points
    private float y0 = -2f;
    private float x1 = 7f;
    private float x3 = 2f;
    
    private float calibrator = 3f;
    private float xStep;

    [SerializeField] Transform laserPoint;
    [SerializeField] int raysCast = 6;

    private RaycastHit2D[] raycastsHit;
    
    private float teleportInterval;
    
    protected override void Awake() {
        base.Awake();
        state = State.Attacking;
        DEATH_ANIMATION_LENGTH = 0.833f;
        ATTACK_DURATION = 1f;
    }
    
    protected override void Start() {
        base.Start();
        hitPoints = 1;
        currentHealth = hitPoints;
        attackDamage = 1;
        attackKnockback = 3f;
        raycastsHit = new RaycastHit2D[raysCast];
        // Calculate the step between each ray
        xStep = (x1-x3) / (raysCast-1);
        
        SetBoundaries();
        FixInitialPosition();
    }

    protected override void Update() {
        base.Update();
        
        SetBoundaries();

        if (raycastHitMax.collider != null)
            Debug.DrawLine(this.transform.position, raycastHitMax.point, Color.red);
        else
            Debug.DrawRay(this.transform.position, Vector2.right, Color.blue);
        if (raycastHitMin.collider != null)
            Debug.DrawLine(this.transform.position, raycastHitMin.point, Color.red);
        else
            Debug.DrawRay(this.transform.position, Vector2.left, Color.blue);

        DebugDrawRays();
        
        // if (damagedDuration < Mathf.Epsilon && teleportInterval < Mathf.Epsilon && state == State.Teleporting)
        //     Teleport();
    }

    protected override void DeductTimers() {
        base.DeductTimers();
        teleportInterval -= Time.deltaTime;
    }

    private void SetBoundaries() {
        LayerMask groundHazardMask = LayerMask.GetMask("Ground", "Hazards");
        raycastHitMax = Physics2D.Raycast(this.transform.position, Vector2.right, Mathf.Infinity, 1 << groundHazardMask);
        maxPosition = raycastHitMax.collider ? raycastHitMax.point.x : Mathf.Infinity;
        raycastHitMin = Physics2D.Raycast(this.transform.position, Vector2.left, Mathf.Infinity, 1 << groundHazardMask);
        minPosition = raycastHitMin.collider ? raycastHitMin.point.x : -Mathf.Infinity;
        // print("Max Position = " + maxPosition);
        // print("Min Position = " + minPosition);
    }

    private void FixInitialPosition() {
        float xPosition = this.transform.position.x;
        Vector2 facingDirection = new Vector2(this.transform.localScale.x, 0f);
        if((xPosition - minPosition < MIN_DISTANCE_TELEPORT && facingDirection.x < 0) || (maxPosition - xPosition < MIN_DISTANCE_TELEPORT && facingDirection.x > 0))
           FlipSprite();
    }

    protected override void Attack() {
        damagedDuration = ATTACK_DURATION;
        enemyAnimator.SetTrigger(Attacking);
        AudioSource.PlayClipAtPoint(attackSFX, this.transform.position);

        StartCoroutine(ShootLaser());

        attackCooldown = Random.Range(MIN_WAIT_TIME+2, MAX_WAIT_TIME);
    }

    private IEnumerator ShootLaser() {
        // Delay damage dealing
        yield return new WaitForSeconds(ATTACK_DURATION/4);
        
        bool rayHit = false;
        Vector2 knockbackKick = Vector2.zero;
        
        // Cast rays along the trajectory of the laser beam
        for (int i = 0; i < raysCast; i++) {
            // Calculate the direction of the ray
            float x = x1 - xStep*i;
            float angle = Mathf.Atan(x/y0);
            float magnitude = Mathf.Abs(y0) / Mathf.Cos(angle);
            Vector2 direction = new Vector2(x/calibrator, y0/calibrator);
        
            Vector2 distanceFromTarget = PlayerMechanics.Instance.GetPosition() - this.transform.position;
            // Cast the ray and check if it hit the player character
            RaycastHit2D raycastHit = Physics2D.Raycast(laserPoint.position, direction.normalized, magnitude/calibrator, 1 << LayerMask.NameToLayer("Player"));
            raycastsHit[i] = raycastHit;
            if (raycastHit.collider != null) {
                // The player has been hit by the laser beam
                knockbackKick = distanceFromTarget.normalized * attackKnockback;
                knockbackKick.y = 0f;
                rayHit = true;
            }
        }
        if(rayHit)
            PlayerMechanics.Instance.TakeDamage(this, knockbackKick, attackDamage);
        // if(teleportInterval < Mathf.Epsilon)
        //     state = (State)Random.Range(3,5);
    }

    // Show attack rays in editor while Gizmos are enabled
    private void DebugDrawRays() {
        for (int i = 0; i < raysCast; i++) {
            float x = x1 - xStep*i;
            Vector2 direction = new Vector2(x/calibrator, y0/calibrator);
            if (raycastsHit[i].collider != null) {
                Debug.DrawLine(laserPoint.position, raycastsHit[i].point, Color.red);
            }
            else {
                Debug.DrawRay(laserPoint.position, direction, Color.green);
            }
        }
    }
    
    private bool IsPointOnWall(Vector2 point) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, 0.1f, LayerMask.GetMask("Ground"));
        return colliders.Length > 0;
    }

    private bool IsWithinBoundaries(Vector2 point) {
        return point.x > minPosition && point.x < maxPosition;
    }

    protected override void FlipSprite() {
        this.transform.localScale = new Vector2(-this.transform.localScale.x, transform.localScale.y);
        // Flip the x values for ray casting
        x1 = -x1;
        x3 = -x3;
        xStep = -xStep;
    }

    private void Teleport() {
        Vector2 facingDirection = new Vector2(this.transform.localScale.x, 0f);
        Vector3 newPosition = currentPosition + facingDirection * Random.Range(MIN_DISTANCE_TELEPORT, MAX_DISTANCE_TELEPORT);
        bool canTeleport = IsWithinBoundaries(newPosition);
        if (canTeleport) {
            enemyAnimator.SetTrigger(Teleporting);
            FlipSprite();
            this.transform.position = newPosition;
            teleportInterval = Random.Range((MIN_WAIT_TIME+2) * 3, MAX_WAIT_TIME*3);
            
            state = State.Attacking;
        }
    }
}
