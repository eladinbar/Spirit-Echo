using UnityEngine;

public class BringerOfDeathMechanics : EnemyMechanics {
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Attacking = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");
    
    private const float ATTACK_COOLDOWN = 3f;
    private const float ATTACK_DURATION = 1f;
    
    private Animator bringerAnimator;

    [SerializeField] float attackRange = 2.5f;
    [SerializeField] int attackDamage = 5;
    [SerializeField] float attackKnockback = 5f;

    private float attackCooldown = 0f;

    protected override void Awake() {
        base.Awake();
        bringerAnimator = GetComponent<Animator>();
    }
    
    protected override void Start() {
        base.Start();
        hitPoints = 100;
    }
    
    protected override void Update() {
        base.Update();
        Walk();
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

    void Walk() {
        bool bringerIsWalking = Mathf.Abs(this.enemyRigidbody.velocity.x) > Mathf.Epsilon;
        bringerAnimator.SetBool(IsWalking, bringerIsWalking);
    }

    void Attack() {
        waitTimeInPosition = ATTACK_DURATION;
        enemyRigidbody.velocity = Vector2.zero;
        bringerAnimator.SetTrigger(Attacking);
        Vector2 distanceFromTarget = PlayerMechanics.Instance.GetPosition() - this.transform.position;
        if (distanceFromTarget.magnitude <= attackRange) {
            Vector2 knockbackKick = distanceFromTarget.normalized * attackKnockback;
            knockbackKick.y = 0f;
            PlayerMechanics.Instance.TakeDamage(this, knockbackKick, attackDamage);
        }
        attackCooldown = ATTACK_COOLDOWN;
    }
}
