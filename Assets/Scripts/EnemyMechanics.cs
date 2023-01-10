using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMechanics : MonoBehaviour {
    private enum State {
        Roaming,
        ChaseTarget
    }
    
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected int pointsPerKill = 50;
    [SerializeField] protected int hitPoints = 1;
    [SerializeField] protected float aggroRange = 5f;
    
    protected Rigidbody2D enemyRigidbody;
    protected BoxCollider2D wallDetector;
    
    protected Vector2 currentPosition;
    protected Vector2 roamPosition;
    private State state;
    
    private float waitTimeInPosition = 0f;

    public int PointsPerKill => pointsPerKill;

    public int HitPoints {
        get => hitPoints;
        set => hitPoints = value;
    }

    protected virtual void Awake() {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        wallDetector = GetComponent<BoxCollider2D>();
        state = State.Roaming;
    }

    protected virtual void Start() {
        currentPosition = this.transform.position;
        roamPosition = GetRoamingPosition();
    }
    
    protected virtual void Update() {
        switch (state) {
            case State.Roaming:
                if (waitTimeInPosition <= Mathf.Epsilon || enemyRigidbody.velocity.x > Mathf.Epsilon) {
                    MoveTo(roamPosition);
                    const float reachedPositionDistance = 1f;
                    if (Vector2.Distance(this.transform.position, roamPosition) < reachedPositionDistance) {
                        // Reached Roam Position
                        waitTimeInPosition = Random.Range(1f, 5f);
                        enemyRigidbody.velocity = new Vector2(0, 0f);
                        roamPosition = GetRoamingPosition(); // Get a new roaming position
                    }
                    currentPosition = this.transform.position;
                }
                else
                    waitTimeInPosition -= Time.deltaTime;
                
                FindTarget();
                break;
            
            case State.ChaseTarget:
                MoveTo(PlayerMechanics.Instance.GetPosition());
                break;
        }
        
    }

    void MoveTo(Vector2 roamPosition) {
        if (this.transform.position.x - roamPosition.x > Mathf.Epsilon)
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

    void FindTarget() {
        Vector2 enemyPosition = this.transform.position;
        bool canSeeTarget = Math.Abs(enemyPosition.x - PlayerMechanics.Instance.GetPosition().x) < aggroRange &&
                            Math.Abs(enemyPosition.y - PlayerMechanics.Instance.GetPosition().y) < 0.01f;  
        if (canSeeTarget) {
            // Player within target range
            state = State.ChaseTarget;
        }
    }

    void CanSeeTarget(float distance) {
        Vector2 endOfSight = this.transform.position + Vector3.right * distance; 
        
        RaycastHit2D raycastHit = Physics2D.Linecast(this.transform.position, endOfSight, 1 << LayerMask.NameToLayer("Player"));
    }
    
    void OnTriggerEnter2D(Collider2D collider2d) {
        if (wallDetector.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            moveSpeed = -moveSpeed;
            FlipSprite();
            // Get new roaming position in direction of movement
            roamPosition = Mathf.Sign(moveSpeed) < 0 ? GetLeftRoamingPosition() : GetRightRoamingPosition();
        }
    }

    void FlipSprite() {
        transform.localScale = new Vector2(-Mathf.Sign(moveSpeed), transform.localScale.y);
    }
}
