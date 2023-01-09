using UnityEngine;

public class BringerOfDeathMechanics : EnemyMechanics {
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Death = Animator.StringToHash("Death");
    
    private Animator bringerAnimator;

    protected override void Awake() {
        base.Awake();
        bringerAnimator = GetComponent<Animator>();
    }
    
    protected override void Update() {
        base.Update();
        Walk();
    }
    
    void Walk() {
        bool bringerIsWalking = Mathf.Abs(this.enemyRigidbody.velocity.x) > Mathf.Epsilon;
        bringerAnimator.SetBool(IsWalking, bringerIsWalking);
    }
}
