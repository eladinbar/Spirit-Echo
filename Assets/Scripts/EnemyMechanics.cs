using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMechanics : MonoBehaviour {
    protected enum State {
        Roaming,
        ChaseTarget,
        Attacking
    }
    
    private const float MIN_WAIT_TIME = 1f;
    private const float MAX_WAIT_TIME = 5f;
    
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected int hitPoints = 1;
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] protected float blindAggroRange = 1.5f;
    
    protected Rigidbody2D enemyRigidbody;
    protected BoxCollider2D wallDetector;
    
    protected Vector2 currentPosition;
    protected Vector2 roamPosition;
    protected State state;
    
    protected float waitTimeInPosition = 0f;

    public int HitPoints {
        get => hitPoints;
        set => hitPoints = value;
    }

    public float MoveSpeed => moveSpeed;

    protected virtual void Awake() {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        wallDetector = GetComponent<BoxCollider2D>();
        state = State.Roaming;
    }

    protected virtual void Start() {
        currentPosition = this.transform.position;
        roamPosition = GetRoamingPosition();
    }
    
    protected virtual void Update() {
        FindTarget();
        DeductTimers();
        
        switch (state) {
            case State.Roaming:
                Roam();
                break;
            
            case State.ChaseTarget:
                ChaseTarget();
                break;
        }
    }

    protected virtual void DeductTimers() {
        waitTimeInPosition -= Time.deltaTime;
    }

    internal virtual void Roam() {
        if (waitTimeInPosition <= Mathf.Epsilon || enemyRigidbody.velocity.x > Mathf.Epsilon) {
            MoveTo(roamPosition);
            const float reachedPositionDistance = 1f;
            if (Vector2.Distance(this.transform.position, roamPosition) < reachedPositionDistance) {
                // Reached Roam Position - remain idle for 'waitTimeInPosition' seconds
                waitTimeInPosition = Random.Range(MIN_WAIT_TIME, MAX_WAIT_TIME);
                enemyRigidbody.velocity = Vector2.zero;
                roamPosition = GetRoamingPosition(); // Get a new roaming position
            }
            currentPosition = this.transform.position;
        }
    }

    internal virtual void ChaseTarget() {
        MoveTo(PlayerMechanics.Instance.GetPosition());
    }

    void MoveTo(Vector2 movePosition) {
        // Determine movement direction
        if (this.transform.position.x - movePosition.x > Mathf.Epsilon)
            moveSpeed = moveSpeed < 0 ? moveSpeed : -moveSpeed;
        else
            moveSpeed = moveSpeed > 0 ? moveSpeed : -moveSpeed;

        FlipSprite();
        enemyRigidbody.velocity = new Vector2(moveSpeed, 0f);
    }

    Vector2 GetRoamingPosition() {
        Vector2 direction = new Vector2(Random.Range(-1, 2), 0);
        return currentPosition + direction * Random.Range(2, 10);
    }
    
    Vector2 GetLeftRoamingPosition() {
        Vector2 direction = new Vector2(-1, 0);
        return currentPosition + direction * Random.Range(2, 10);
    }
    
    Vector2 GetRightRoamingPosition() {
        Vector2 direction = new Vector2(1, 0);
        return currentPosition + direction * Random.Range(2, 10);
    }

    protected virtual void FindTarget() {
        bool canSeeTarget = CanSeeTarget(aggroRange, blindAggroRange);
        // bool canSeeTargetByRange = Math.Abs(enemyPosition.x - PlayerMechanics.Instance.GetPosition().x) < aggroRange &&
        //                            Math.Abs(enemyPosition.y - PlayerMechanics.Instance.GetPosition().y) < 1f; // Old
        
        state = canSeeTarget ? State.ChaseTarget : State.Roaming;
    }

    protected bool CanSeeTarget(float distance, float blindDistance) {
        Vector3 enemyPosition = this.transform.position;
        Vector3 facingDirection = Vector3.left * Mathf.Sign(this.transform.localScale.x);

        Vector2 endOfSight = enemyPosition + facingDirection * distance;
        Vector2 blindEndOfSight = enemyPosition - facingDirection * blindDistance;
        
        RaycastHit2D raycastHit = Physics2D.Linecast(enemyPosition, endOfSight, 1 << LayerMask.NameToLayer("Player"));
        RaycastHit2D blindRaycastHit = Physics2D.Linecast(enemyPosition, blindEndOfSight, 1 << LayerMask.NameToLayer("Player"));
        
        //TODO - debug
        if(raycastHit.collider != null)
            Debug.DrawLine(transform.position, raycastHit.point, Color.red);
        else
            Debug.DrawLine(transform.position, endOfSight, Color.green);
        if(blindRaycastHit.collider != null)
            Debug.DrawLine(transform.position, blindRaycastHit.point, Color.black);
        else
            Debug.DrawLine(transform.position, blindEndOfSight, Color.blue);
        //TODO - debug
        
        return raycastHit.collider != null || blindRaycastHit.collider != null;
    }

    void OnTriggerEnter2D(Collider2D collider2d) {
        waitTimeInPosition = 0f;
        if (state == State.Attacking) {
            // Target is within range - wait longer
            waitTimeInPosition += Random.Range(MIN_WAIT_TIME*2, MAX_WAIT_TIME*2);
            enemyRigidbody.velocity = Vector2.zero;
        }
        if (wallDetector.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            moveSpeed = -moveSpeed;
            FlipSprite();
            // Get new roaming position in direction of movement - remain idle for 'waitTimeInPosition/2' seconds
            waitTimeInPosition += Random.Range(MIN_WAIT_TIME/2, MAX_WAIT_TIME/2);
            enemyRigidbody.velocity = Vector2.zero;
            roamPosition = Mathf.Sign(moveSpeed) < 0 ? GetLeftRoamingPosition() : GetRightRoamingPosition();
        }
    }

    private void OnTriggerStay2D(Collider2D collider2d) {
        enemyRigidbody.velocity = Vector2.zero;
    }

    void FlipSprite() {
        this.transform.localScale = new Vector2(-Mathf.Sign(moveSpeed), transform.localScale.y);
    }
}
