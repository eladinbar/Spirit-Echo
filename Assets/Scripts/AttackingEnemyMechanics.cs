using UnityEngine;

public abstract class AttackingEnemyMechanics : EnemyMechanics {
    protected static readonly int Attacking = Animator.StringToHash("Attack");

    protected float ATTACK_COOLDOWN;
    protected float ATTACK_DURATION;

    [SerializeField] protected AudioClip attackSFX;

    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float attackKnockback;
    protected float attackCooldown = 0f;

    protected override void Update() {
        base.Update();
        if (damagedDuration < Mathf.Epsilon && attackCooldown < Mathf.Epsilon && state == State.Attacking)
            Attack();
    }
    
    protected override void DeductTimers() {
        base.DeductTimers();
        attackCooldown -= Time.deltaTime;
    }

    protected virtual void Attack() {
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
