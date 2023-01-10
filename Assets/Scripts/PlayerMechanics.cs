using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int Death = Animator.StringToHash("Death");
    public bool jumpEnabled = true;
    
    
    [Header("Movement Speed")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    
    Vector2 moveInput;

    [Header("SFX")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip timeTravelSFX;
    
    AudioSource audioSource;

    [Header("Tilemaps")]
    [SerializeField] GameObject presentTilemap;
    [SerializeField] GameObject pastTilemap;

    PastHandler pastTilemapHandler;
    PresentHandler presentTilemapHandler;

    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    BoxCollider2D feetCollider;
    CapsuleCollider2D bodyCollider;
    bool isAlive = true;
    
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
        }
    }

    void OnMove(InputValue value) {
        if (isAlive) {
            moveInput = value.Get<Vector2>();
        }
    }

    void OnJump(InputValue value) {
        if (isAlive) {
            bool playerCanJump = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground") );
            if (value.isPressed && playerCanJump && jumpEnabled) {
                audioSource.PlayOneShot(jumpSFX);
                playerRigidbody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    void OnTraverseTime() {
        if (isAlive) {
            audioSource.PlayOneShot(timeTravelSFX);
            
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
        bool playerIsJumping = Mathf.Abs(playerRigidbody.velocity.y) > Mathf.Epsilon;
        playerAnimator.SetBool(IsJumping, playerIsJumping);
    }
}
