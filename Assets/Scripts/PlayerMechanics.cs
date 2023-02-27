using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour {
    // Player Instance
    public static PlayerMechanics Instance { get; private set; }

    // Animation state hashes
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
    private static readonly int Dashing = Animator.StringToHash("Dash");
    private static readonly int Attacking = Animator.StringToHash("Attack");
    private static readonly int Using = Animator.StringToHash("Use");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Death = Animator.StringToHash("Death");
    
    // Constants
    private const float DAMAGED_INVULNERABILITY_TIME = 0.5f;
    private const float KNOCKBACK_TIME = 0.25f;
    private const float TIME_TRAVERSAL_COOLDOWN = 3f;
    private const float DASH_COOLDOWN = 1f;
    private const float ATTACK_COOLDOWN = 0.667f;
    
    [Header("Stats")]
    
    [Header("Movement Speed")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float doubleJumpSpeed = 4f;
    [SerializeField] float dashSpeed = 24f;
    
    [SerializeField] Vector2 deathKick = new Vector2(0f, 10f);
    [SerializeField] Vector2 damagedKick = new Vector2(0f, 5f);
    private Vector2 damagedKickReverse;
    [SerializeField] Vector2 hitKick = new Vector2(-5f, 0f);
    
    Vector2 moveInput;
    
    [Header("Health")]
    [SerializeField] int maxHitPoints = 5;
    [SerializeField] int currentHitPoints;
    private float invulnerabilityTime = 0.5f;
    private float knockbackTime = 0f;

    public int MaxHitPoints => maxHitPoints;
    public int CurrentHitPoints => currentHitPoints;

    [Header("SFX")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip hurtSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip timeTravelSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip flipGravitySFX;
    [SerializeField] AudioClip attackSFX;
    [SerializeField] AudioClip errorSFX;

    AudioSource audioSource;

    [Header("Tilemaps")]
    [SerializeField] GameObject presentTilemap;
    [SerializeField] GameObject pastTilemap;

    PastHandler pastTilemapHandler;
    PresentHandler presentTilemapHandler;

    // Physics
    Rigidbody2D playerRigidbody;
    BoxCollider2D feetCollider;
    CapsuleCollider2D bodyCollider;
    
    Animator playerAnimator;

    // States
    bool isAlive = true;
    
    //// Move
    public bool moveEnabled = true;
    
    //// Jump
    public UnityEvent onJump;
    public bool jumpEnabled = true;

    //// Time Traversal
    public UnityEvent onTraverseTime;
    public bool unlockedTimeTraversal = false;
    public bool timeTraverseEnabled = true;
    private float timeTraversalDelay = 0f;

    //// Double Jump
    public bool unlockedDoubleJump = false;
    private bool canDoubleJump = false;

    //// Dash
    public bool unlockedDash = false;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashCooldown = 1f;

    //// Attack
    public bool unlockedAttack = false;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask enemyLayers;
    private float attackCooldown = 0f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] int attackDamage = 1;

    //// Gravity Shift
    public bool unlockedGravityShift = false;
    private bool isFlipped = false;
    
    //// Wall CLimb
    public bool unlockedWallClimb = false;

    void Awake() {
        Instance = this;
    }

    void Start() {
        
        #if UNITY_EDITOR
            unlockedTimeTraversal = true;
         /*   
            unlockedDoubleJump = true;
            unlockedDash = true;
            unlockedAttack = true;
          
            unlockedGravityShift = true;
            unlockedWallClimb = true;
         */     
        #endif
        
        audioSource = GetComponent<AudioSource>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        pastTilemapHandler = pastTilemap.GetComponent<PastHandler>();
        presentTilemapHandler = presentTilemap.GetComponent<PresentHandler>();
        damagedKickReverse = new Vector2(damagedKick.x, -damagedKick.y);
        enemyLayers = LayerMask.GetMask("Enemies");
        currentHitPoints = maxHitPoints;
    }

    void Update() {
        if (isAlive) {
            DeductTimers();
            
            if (isDashing || knockbackTime > Mathf.Epsilon)
                return;
            
            Run();
            Jump();
        }
    }

    private void DeductTimers() {
        invulnerabilityTime -= Time.deltaTime;
        knockbackTime -= Time.deltaTime;
        timeTraversalDelay -= Time.deltaTime;
        dashCooldown -= Time.deltaTime;
        attackCooldown -= Time.deltaTime;
    }

    public Vector3 GetPosition() {
        return this.transform.position;
    }

    void OnMove(InputValue value) {
        if (isAlive && moveEnabled) {
            moveInput = value.Get<Vector2>();
        }
    }

    void OnJump(InputValue value) {
        if (isAlive) {
            bool playerCanJump = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))&& jumpEnabled;
            canDoubleJump = playerAnimator.GetBool(IsJumping) && canDoubleJump && unlockedDoubleJump;
            if (value.isPressed && playerCanJump) {
                onJump.Invoke();
                audioSource.PlayOneShot(jumpSFX);
                if(!isFlipped){
                    playerRigidbody.velocity += new Vector2(0f, jumpSpeed);
                }
                else{
                    playerRigidbody.velocity += new Vector2(0f, -1 * jumpSpeed);
                }
                canDoubleJump = true;
            } else if (value.isPressed && canDoubleJump) {
                audioSource.PlayOneShot(jumpSFX);
                if(!isFlipped){
                    playerRigidbody.velocity += new Vector2(0f, doubleJumpSpeed);
                }else{
                    playerRigidbody.velocity += new Vector2(0f, -1 * doubleJumpSpeed);
                }
                playerAnimator.SetTrigger(DoubleJump);
                canDoubleJump = false;
            }
        }
    }
    
    internal void OnTraverseTime() {
        if (isAlive && unlockedTimeTraversal && timeTraverseEnabled && timeTraversalDelay <= Mathf.Epsilon) {
            onTraverseTime.Invoke();
            audioSource.PlayOneShot(timeTravelSFX);
            timeTraversalDelay = TIME_TRAVERSAL_COOLDOWN;

            bool toPresent = pastTilemap.activeSelf;
            if (toPresent) {
                presentTilemap.SetActive(true);
                pastTilemapHandler.StartFading();
            }
            else {
                pastTilemap.SetActive(true);
                presentTilemapHandler.StartFading();
            }
        }
        if(isAlive && !timeTraverseEnabled){
            audioSource.PlayOneShot(errorSFX);
        }
    }

    void OnDash() {
        canDash = (canDash || feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) && unlockedDash;
        if (isAlive && dashCooldown <= Mathf.Epsilon && canDash) {
            StartCoroutine(Dash());
            canDash = false;
            dashCooldown = DASH_COOLDOWN;
        }
    }

    void OnAttack() {
        if (isAlive && knockbackTime <= Mathf.Epsilon && attackCooldown <= Mathf.Epsilon && unlockedAttack) {
            playerAnimator.SetTrigger(Attacking);
            audioSource.PlayOneShot(attackSFX);
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            
            foreach (Collider2D enemy in enemiesHit) {
                enemy.GetComponent<EnemyMechanics>().TakeDamage(Vector2.zero, attackDamage);
            }
            attackCooldown = ATTACK_COOLDOWN;
        }
    }

    // Show attack sphere in editor while Gizmos are enabled
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnFlip() {
        if (unlockedGravityShift) {
            audioSource.PlayOneShot(flipGravitySFX);
            if (isFlipped) {
                transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
                playerRigidbody.gravityScale = 1;
            }
            else {
                transform.localScale = new Vector3(transform.localScale.x, -1, transform.localScale.z);
                playerRigidbody.gravityScale = -1;
            }

            isFlipped = !isFlipped;
        }
    }

    void FlipSprite(bool playerIsRunning, float moveSpeed) {
        if (playerIsRunning && attackCooldown <= Mathf.Epsilon)
            transform.localScale = new Vector2(Mathf.Sign(moveSpeed), transform.localScale.y);
    }

    void Run() {
        Vector2 runVelocity = new Vector2(moveInput.x * runSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = runVelocity;

        bool playerIsRunning = Mathf.Abs(runVelocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool(IsRunning, playerIsRunning);

        FlipSprite(playerIsRunning, runVelocity.x);
    }

    private IEnumerator Dash() {
        isDashing = true;
        playerAnimator.SetTrigger(Dashing);
        audioSource.PlayOneShot(dashSFX);
        float originalGravity = playerRigidbody.gravityScale;
        playerRigidbody.gravityScale = 0f;
        playerRigidbody.velocity = new Vector2(this.transform.localScale.x * dashSpeed, 0f);
        yield return new WaitForSeconds(dashingTime);
        playerRigidbody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
    }

    void Jump() {
        bool playerIsJumping = Mathf.Abs(playerRigidbody.velocity.y) > 0.01f;
        playerAnimator.SetBool(IsJumping, playerIsJumping);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Tilemap"))
            canDash = true;
        if ((other.CompareTag("Enemy") || other.CompareTag("Hazard")) && isAlive && invulnerabilityTime <= Mathf.Epsilon) {
            Vector2 kick = !isFlipped ? damagedKick : damagedKickReverse;
            TakeDamage(other, kick);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if ((other.CompareTag("Enemy") || other.CompareTag("Hazard")) && isAlive && invulnerabilityTime <= Mathf.Epsilon) {
            Vector2 kick = !isFlipped ? damagedKick : damagedKickReverse;
            TakeDamage(other, kick);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Tilemap"))
            canDoubleJump = true;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        if (other.CompareTag("Tilemap"))
            canDash = true;
        if ((other.CompareTag("Enemy") || other.CompareTag("Hazard")) && isAlive && invulnerabilityTime <= Mathf.Epsilon) {
            Vector2 kick = !isFlipped ? damagedKick : damagedKickReverse;
            TakeDamage(other, kick);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        Collider2D other = collision.collider;
        if ((other.CompareTag("Enemy") || other.CompareTag("Hazard")) && isAlive && invulnerabilityTime <= Mathf.Epsilon) {
            Vector2 kick = !isFlipped ? damagedKick : damagedKickReverse;
            TakeDamage(other, kick);
        }
    }

    private void Knockback(Vector2 kick) {
        knockbackTime = KNOCKBACK_TIME;
        playerRigidbody.velocity = kick;
    }

    public void TakeDamage(Behaviour other, Vector2 kick, int damage=1) {
        currentHitPoints -= damage;
        FindObjectOfType<GameSession>().UpdateLives(currentHitPoints);
        audioSource.PlayOneShot(hurtSFX);
        playerAnimator.SetTrigger(Hurt);
        if(currentHitPoints <= 0)
            Die();
        Knockback(kick);
        canDoubleJump = true;
        invulnerabilityTime = DAMAGED_INVULNERABILITY_TIME;
    }

    void Die() {
        audioSource.PlayOneShot(deathSFX);

        isAlive = false;
        playerAnimator.SetTrigger(Death);
        playerRigidbody.velocity = deathKick;
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
}
   