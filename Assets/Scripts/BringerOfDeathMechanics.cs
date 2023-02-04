using System;
using UnityEngine;

public class BringerOfDeathMechanics : EnemyMechanics {
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Death = Animator.StringToHash("Death");
    
    private Animator bringerAnimator;

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
        if (state == State.Attacking) {
            // this.transform.position = new Vector2(0f, 0f);
        }
        
        Walk();
    }

    // protected override void FindTarget() {
    //     base.FindTarget();
    //     float distanceFromTarget = this.transform.position.x - PlayerMechanics.Instance.GetPosition().x;
    //     bool canSeeTarget = CanSeeTarget(aggroRange);
    //     float attackDistance = 3f;
    //
    //     if (distanceFromTarget <= attackDistance && canSeeTarget)
    //         state = State.Attacking;
    // }

    void Walk() {
        bool bringerIsWalking = Mathf.Abs(this.enemyRigidbody.velocity.x) > Mathf.Epsilon;
        bringerAnimator.SetBool(IsWalking, bringerIsWalking);
    }
}
