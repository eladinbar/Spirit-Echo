using UnityEngine;

public abstract class RoamingAttackingEnemyMechanics : EnemyMechanics
{
    protected static readonly int Attacking = Animator.StringToHash("Attack");
    
    protected float ATTACK_COOLDOWN = 5f;
    protected float ATTACK_DURATION = 1.5f;
    
    [SerializeField] protected AudioClip attackSFX;
    
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] protected float blindAggroRange = 1.5f;
    
    protected Vector2 roamPosition;
    
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float attackKnockback;
    protected float attackCooldown = 0f;
    

    protected override void Update() {
        base.Update();
        FindTarget();

        if (waitTimeInPosition < Mathf.Epsilon) {
            switch (state) {
                case State.Roaming:
                    Roam();
                    break;

                case State.ChaseTarget:
                    ChaseTarget();
                    break;
            }
        }

        Move();
        if (damagedDuration < Mathf.Epsilon && attackCooldown < Mathf.Epsilon && state == State.Attacking)
            Attack();
    }

    protected override void DeductTimers() {
        base.DeductTimers();
        attackCooldown -= Time.deltaTime;
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
    
    protected abstract void Move();
    
    void Attack() {
        damagedDuration = ATTACK_DURATION;
        enemyRigidbody.velocity = Vector2.zero;
        Vector2 distanceFromTarget = PlayerMechanics.Instance.GetPosition() - this.transform.position;
        if (distanceFromTarget.x * moveSpeed < 0) {
            moveSpeed = -moveSpeed;
            FlipSprite();
        }

        enemyAnimator.SetTrigger(Attacking);
        audioSource.PlayOneShot(attackSFX);
        if (distanceFromTarget.magnitude <= attackRange) {
            Vector2 knockbackKick = distanceFromTarget.normalized * attackKnockback;
            knockbackKick.y = 0f;
            PlayerMechanics.Instance.TakeDamage(this, knockbackKick, attackDamage);
        }
        attackCooldown = ATTACK_COOLDOWN;
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

        State previousState = state;
        state = canSeeTarget ? State.ChaseTarget : State.Roaming;
        // If state changed, define a new roaming position
        if (previousState != state)
            roamPosition = GetRoamingPosition();
        // If target spotted, stop waiting
        if (canSeeTarget)
            waitTimeInPosition = 0f;
        
        float distanceFromTarget = this.transform.position.x - PlayerMechanics.Instance.GetPosition().x;
        if (distanceFromTarget <= attackRange && canSeeTarget && attackCooldown <= Mathf.Epsilon)
            state = State.Attacking;
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

    void OnTriggerExit2D(Collider2D collider2d) {
        waitTimeInPosition = 0f;
        if (state == State.Attacking) {
            // Target is within range - wait longer
            waitTimeInPosition += Random.Range(MIN_WAIT_TIME*2, MAX_WAIT_TIME*2);
            enemyRigidbody.velocity = Vector2.zero;
        }
        if (collider2d.CompareTag("Tilemap")) {
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
}
