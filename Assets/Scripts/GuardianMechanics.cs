using UnityEngine;

public class GuardianMechanics : AttackingEnemyMechanics
{
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
        attackRange = 4f;
        attackDamage = 1;
        attackKnockback = 3f;
    }

    protected override void Attack() {
        damagedDuration = ATTACK_DURATION;
        Vector2 distanceFromTarget = PlayerMechanics.Instance.GetPosition() - this.transform.position;
        enemyAnimator.SetTrigger(Attacking);
        audioSource.PlayOneShot(attackSFX);
        if (distanceFromTarget.magnitude <= attackRange) {
            Vector2 knockbackKick = distanceFromTarget.normalized * attackKnockback;
            knockbackKick.y = 0f;
            PlayerMechanics.Instance.TakeDamage(this, knockbackKick, attackDamage);
        }
        attackCooldown = Random.Range(MIN_WAIT_TIME+2, MAX_WAIT_TIME);
    }
}
