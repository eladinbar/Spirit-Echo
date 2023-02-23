using UnityEngine;

public class BringerOfDeathMechanics : EnemyMechanics {
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Attacking = Animator.StringToHash("Attack");
    private static readonly int Casting = Animator.StringToHash("Cast");

    private const float ATTACK_COOLDOWN = 3f;
    private const float ATTACK_DURATION = 1f;

    [SerializeField] AudioClip attackSFX;

    [SerializeField] float attackRange = 2.5f;
    [SerializeField] int attackDamage = 5;
    [SerializeField] float attackKnockback = 5f;

    private float attackCooldown = 0f;

    protected override void Awake() {
        base.Awake();
        DEATH_ANIMATION_LENGTH = 0.833f;
    }
    
    protected override void Start() {
        base.Start();
        hitPoints = 10;
        currentHealth = hitPoints;
        aggroRange = 5f;
        blindAggroRange = 1.5f;
    }
    
    protected override void Update() {
        base.Update();
        Walk();
        if (damagedDuration < Mathf.Epsilon && attackCooldown < Mathf.Epsilon && state == State.Attacking)
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
        enemyAnimator.SetBool(IsWalking, bringerIsWalking);
    }

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
}
