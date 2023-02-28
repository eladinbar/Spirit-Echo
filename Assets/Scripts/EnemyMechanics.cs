using System.Collections;
using UnityEngine;

public abstract class EnemyMechanics : MonoBehaviour {
    protected enum State {
        Idling,
        Roaming,
        ChasingTarget,
        Attacking,
        Teleporting
    }
    
    protected static readonly int Hurt = Animator.StringToHash("Hurt");
    protected static readonly int Death = Animator.StringToHash("Death");
    
    protected const float MIN_WAIT_TIME = 1f;
    protected const float MAX_WAIT_TIME = 5f;
    protected const float KNOCKBACK_TIME = 1f;
    protected float DEATH_ANIMATION_LENGTH;
    
    [SerializeField] protected float moveSpeed = 0f;
    [SerializeField] protected int hitPoints;
    protected int currentHealth;

    [SerializeField] protected AudioClip hurtSFX;
    [SerializeField] protected AudioClip deathSFX;
    
    AudioManager audioManager;
    AudioSource audioManagerSource;
    protected AudioSource audioSource;
    protected Animator enemyAnimator;
    
    protected Rigidbody2D enemyRigidbody;

    protected Vector2 currentPosition;
    protected State state;
    
    protected float waitTimeInPosition = 0f;
    protected float damagedDuration = 0f;

    public int HitPoints {
        get => hitPoints;
        set => hitPoints = value;
    }
    
    public float MoveSpeed => moveSpeed;

    protected virtual void Awake() {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        enemyAnimator = GetComponent<Animator>();
    }

    protected virtual void Start() {
        audioManager = FindObjectOfType<AudioManager>();
        audioManagerSource = audioManager.GetComponent<AudioSource>();
        currentPosition = this.transform.position;
    }
    
    protected virtual void Update() {
        DeductTimers();
    }

    protected virtual void DeductTimers() {
        waitTimeInPosition -= Time.deltaTime;
        damagedDuration -= Time.deltaTime;
    }

    protected virtual void FlipSprite() {
        this.transform.localScale = new Vector2(-Mathf.Sign(moveSpeed), transform.localScale.y);
    }
    
    protected void Knockback(Vector2 kick) {
        waitTimeInPosition = KNOCKBACK_TIME;
        // playerRigidbody.velocity = kick;
    }

    public virtual void TakeDamage(Vector2 kick, int damage=1) {
        currentHealth -= damage;
        if(hurtSFX && currentHealth > 0)
            audioSource.PlayOneShot(hurtSFX);
        if(enemyAnimator.HasState(0, Hurt))
            enemyAnimator.SetTrigger(Hurt);
        
        if(currentHealth <= 0)
            Die();
        Knockback(kick);
    }
    
    protected virtual void Die() {
        if(deathSFX)
            audioManagerSource.PlayOneShot(deathSFX);

        enemyAnimator.SetTrigger(Death);
        StartCoroutine(ProcessDeath());
    }

    private IEnumerator ProcessDeath() {
        enemyAnimator.SetTrigger(Death);
        yield return new WaitForSeconds(DEATH_ANIMATION_LENGTH);
        Destroy(this.gameObject);
    }
}
