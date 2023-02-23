using UnityEngine;

public class TrashMonsterMechanics : EnemyMechanics {
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int Attacking = Animator.StringToHash("Attack");
    private static readonly int HitRun = Animator.StringToHash("Hit & Run");

    private const float ATTACK_COOLDOWN = 5f;
    private const float ATTACK_DURATION = 1.5f;
    private const float HITRUN_DURATION = 2f;

    [SerializeField] AudioClip attackSFX;

    [SerializeField] float attackRange = 1.5f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackKnockback = 3f;

    private float attackCooldown = 0f;

    protected override void Awake() {
        base.Awake();
        DEATH_ANIMATION_LENGTH = 0.917f;
    }
    
    protected override void Start() {
        base.Start();
        hitPoints = 2;
        currentHealth = hitPoints;
        aggroRange = 3f;
        blindAggroRange = 1f;
    }
    
    protected override void Update() {
        base.Update();
        Run();
        if (state == State.Attacking)
            Attack();
    }
    
    protected override void DeductTimers() {
        base.DeductTimers();
        attackCooldown -= Time.deltaTime;
    }

    protected override void FindTarget() {
        base.FindTarget();
        float distanceFromTarget = this.transform.position.x - PlayerMechanics.Instance.GetPosition().x;
        bool canSeeTarget = CanSeeTarget(aggroRange, blindAggroRange);

        if (distanceFromTarget <= attackRange && canSeeTarget && attackCooldown <= Mathf.Epsilon)
            state = State.Attacking;
    }

    void Run() {
        bool bringerIsWalking = Mathf.Abs(this.enemyRigidbody.velocity.x) > Mathf.Epsilon;
        enemyAnimator.SetBool(IsRunning, bringerIsWalking);
    }

    void Attack() {
        waitTimeInPosition = ATTACK_DURATION;
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
}

