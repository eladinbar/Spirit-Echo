using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int Dashing = Animator.StringToHash("dashing");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int DoubleJump = Animator.StringToHash("doubleJump");
    private static readonly int WasHurt = Animator.StringToHash("wasHurt");
    private static readonly int Death = Animator.StringToHash("Death");

    private const float DAMAGED_INVULNERABILITY_TIME = 0.5f;
    private const float KNOCKBACK_TIME = 0.25f;
    private const float TIME_TRAVERSAL_COOLDOWN = 3f;
    private const float DASH_COOLDOWN = 1f;
    public static PlayerMechanics Instance { get; private set; }
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
    [SerializeField] int hitPoints = 1;
    private float invulnerabilityTime = 0.5f;
    private float knockbackTime = 0f;

    [Header("SFX")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip hurtSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip timeTravelSFX;

    AudioSource audioSource;

    [Header("Tilemaps")]
    [SerializeField] GameObject presentTilemap;
    [SerializeField] GameObject pastTilemap;

    PastHandler pastTilemapHandler;
    PresentHandler presentTilemapHandler;

    // Physics
    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    BoxCollider2D feetCollider;
    CapsuleCollider2D bodyCollider;

    // States
    bool isAlive = true;
    bool isFlipped = false;
    
    public bool unlockedTimeTraversal = true;
    private float timeTraversalDelay = 0f;

    public bool unlockedDoubleJump = true;
    private bool canDoubleJump = false;

    public bool unlockedDash = true;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashCooldown = 1f;

    void Awake() {
        Instance = this;
    }

    void Start()    {
        audioSource = GetComponent<AudioSource>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        pastTilemapHandler = pastTilemap.GetComponent<PastHandler>();
        presentTilemapHandler = presentTilemap.GetComponent<PresentHandler>();
        damagedKickReverse = new Vector2(damagedKick.x, -damagedKick.y);
    }

    void Update() {
        if (isAlive) {
            DeductTimers();
            
            if (isDashing)
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
    }

    public Vector3 GetPosition() {
        return this.transform.position;
    }

    void OnMove(InputValue value) {
        if (isAlive) {
            moveInput = value.Get<Vector2>();
        }
    }

    void OnJump(InputValue value) {
        if (isAlive) {
            bool playerCanJump = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
            canDoubleJump = playerAnimator.GetBool(IsJumping) && canDoubleJump;
            if (value.isPressed && playerCanJump) {
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

    void OnDash() {
        canDash = canDash || feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (isAlive && dashCooldown <= Mathf.Epsilon && canDash) {
            StartCoroutine(Dash());
            canDash = false;
            dashCooldown = DASH_COOLDOWN;
        }
    }

    void OnTraverseTime() {
        if (isAlive && timeTraversalDelay <= Mathf.Epsilon) {
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
    }

    void FlipSprite(bool playerIsRunning, float moveSpeed) {
        if (playerIsRunning)
            transform.localScale = new Vector2(Mathf.Sign(moveSpeed), transform.localScale.y);
    }

    void Run() {
        Vector2 runVelocity = playerRigidbody.velocity;
        
        if (knockbackTime <= Mathf.Epsilon) {
            runVelocity = new Vector2(moveInput.x * runSpeed, playerRigidbody.velocity.y);
            playerRigidbody.velocity = runVelocity;
        }

        bool playerIsRunning = Mathf.Abs(runVelocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool(IsRunning, playerIsRunning);

        FlipSprite(playerIsRunning, runVelocity.x);
    }

    private IEnumerator Dash() {
        isDashing = true;
        playerAnimator.SetTrigger(Dashing);
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


    void OnFlip()
    {
        if (isFlipped)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
            playerRigidbody.gravityScale = 1;
        }
        else 
        {
            transform.localScale = new Vector3(transform.localScale.x, -1, transform.localScale.z);
            playerRigidbody.gravityScale = -1;
        }
        isFlipped = !isFlipped;
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
        hitPoints -= damage;
        audioSource.PlayOneShot(hurtSFX);
        playerAnimator.SetTrigger(WasHurt);
        if(hitPoints <= 0)
            Die();
        Knockback(kick);
        canDoubleJump = true;
        invulnerabilityTime = DAMAGED_INVULNERABILITY_TIME;
    }

    void Die() {
        // isAlive = false;
        // audioSource.PlayOneShot(deathSFX);
        // playerAnimator.SetTrigger(Death);
        // playerRigidbody.velocity = deathKick;
        // FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
}
   