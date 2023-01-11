using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int WasHurt = Animator.StringToHash("wasHurt");
    private static readonly int Death = Animator.StringToHash("Death");
    
    public static PlayerMechanics Instance { get; private set; }
    
    [Header("Stats")]
    
    [Header("Movement Speed")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    
    [SerializeField] Vector2 deathKick = new Vector2(0f, 10f);
    [SerializeField] Vector2 damagedKick = new Vector2(-3f, 5f);
    [SerializeField] Vector2 hitKick = new Vector2(-5f, 2f);
    
    Vector2 moveInput;
    
    [Header("Health")]
    [SerializeField] int hitPoints = 1;
    private float invulnerabilityTime = 1f;

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
    public bool unlockedTimeTraversal = true;
    private float timeTraversalDelay = 5f;

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
    }
    
    void Update() {
        if (isAlive) {
            Run();
            Jump();
            
            DeductTimers();
        }
    }

    private void DeductTimers() {
        invulnerabilityTime -= Time.deltaTime;
        timeTraversalDelay -= Time.deltaTime;
    }

    public Vector2 GetPosition() {
        return this.transform.position;
    }

    void OnMove(InputValue value) {
        if (isAlive) {
            moveInput = value.Get<Vector2>();
        }
    }

    void OnJump(InputValue value) {
        if (isAlive) {
            bool playerCanJump = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Hazards"));
            if (value.isPressed && playerCanJump) {
                audioSource.PlayOneShot(jumpSFX);
                playerRigidbody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    void OnTraverseTime() {
        if (isAlive && timeTraversalDelay <= Mathf.Epsilon) {
            audioSource.PlayOneShot(timeTravelSFX);
            timeTraversalDelay = 3f;
            
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
        Vector2 runVelocity = new Vector2(moveInput.x * runSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = runVelocity;

        bool playerIsRunning = Mathf.Abs(runVelocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool(IsRunning, playerIsRunning);

        FlipSprite(playerIsRunning, runVelocity.x);
    }

    void Jump() {
        bool playerIsJumping = Mathf.Abs(playerRigidbody.velocity.y) > 0.01f;
        playerAnimator.SetBool(IsJumping, playerIsJumping);
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if ((other.CompareTag("Enemy") || other.CompareTag("Hazard")) && isAlive && invulnerabilityTime <= Mathf.Epsilon) {
            TakeDamage(other);
            invulnerabilityTime = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        Collider2D other = col.collider;
        if ((other.CompareTag("Enemy") || other.CompareTag("Hazard")) && isAlive && invulnerabilityTime <= Mathf.Epsilon) {
            TakeDamage(other);
            invulnerabilityTime = 1f;
        }
    }

    void TakeDamage(Collider2D other) {
        hitPoints--;
        audioSource.PlayOneShot(hurtSFX);
        playerAnimator.SetTrigger(WasHurt);
        playerRigidbody.velocity = other.CompareTag("Hazard") ? damagedKick : hitKick;
        if(hitPoints <= 0)
            Die();
    }

    void Die() {
        // isAlive = false;
        // audioSource.PlayOneShot(deathSFX);
        // playerAnimator.SetTrigger(Death);
        // playerRigidbody.velocity = deathKick;
        // FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
}
