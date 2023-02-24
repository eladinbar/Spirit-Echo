using UnityEngine;

public class BringerOfDeathMechanics : RoamingAttackingEnemyMechanics {
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Casting = Animator.StringToHash("Cast");

    protected override void Awake() {
        base.Awake();
        DEATH_ANIMATION_LENGTH = 0.833f;
        ATTACK_COOLDOWN = 3f;
        ATTACK_DURATION = 1f;
    }
    
    protected override void Start() {
        base.Start();
        hitPoints = 10;
        currentHealth = hitPoints;
        aggroRange = 5f;
        blindAggroRange = 1.5f;
        attackRange = 2.5f;
        attackDamage = 5;
        attackKnockback = 5f;
    }

    protected override void Move() {
        bool bringerIsWalking = Mathf.Abs(this.enemyRigidbody.velocity.x) > Mathf.Epsilon;
        enemyAnimator.SetBool(IsWalking, bringerIsWalking);
    }
}
