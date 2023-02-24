using UnityEngine;

public class TrashMonsterMechanics : RoamingAttackingEnemyMechanics {
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int HitRun = Animator.StringToHash("Hit & Run");
    
    private const float HITRUN_DURATION = 2f;

    protected override void Awake() {
        base.Awake();
        DEATH_ANIMATION_LENGTH = 0.917f;
        ATTACK_COOLDOWN = 5f;
        ATTACK_DURATION = 1.5f;
    }
    
    protected override void Start() {
        base.Start();
        hitPoints = 2;
        currentHealth = hitPoints;
        aggroRange = 3f;
        blindAggroRange = 1f;
        attackRange = 1.5f;
        attackDamage = 1;
        attackKnockback = 3f;
    }

    protected override void Move() {
        bool bringerIsWalking = Mathf.Abs(this.enemyRigidbody.velocity.x) > Mathf.Epsilon;
        enemyAnimator.SetBool(IsRunning, bringerIsWalking);
    }
}

