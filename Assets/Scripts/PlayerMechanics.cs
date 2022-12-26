using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMechanics : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int Death = Animator.StringToHash("Death");
    
    [Header("Movement Speed")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    
    Vector2 moveInput;

    [Header("SFX")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip deathSFX;
    
    AudioSource audioSource;

    [Header("Tilemaps")]
    [SerializeField] GameObject presentTilemap;
    [SerializeField] GameObject presentBackgroundTilemap;
    [SerializeField] GameObject pastTilemap;
    [SerializeField] GameObject pastBackgroundTilemap;
    
    [SerializeField] PastHandler[] pastHandlers;
    [SerializeField] PresentHandler[] presentHandlers;
    
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
            bool playerCanJump = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

            if (value.isPressed && playerCanJump) {
                //audioSource.PlayOneShot(jumpSFX);
                playerRigidbody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    void OnTraverseTime() {
        if (isAlive) {
            bool toPresent = pastTilemap.activeSelf;
            if (toPresent) {
                presentTilemap.SetActive(true);
                presentBackgroundTilemap.SetActive(true);
            }
            else {
                pastTilemap.SetActive(true);
                pastBackgroundTilemap.SetActive(true);
            }

            pastHandlers[0].StartFading(toPresent);
            pastHandlers[1].StartFading(toPresent);
            presentHandlers[0].StartFading(toPresent);
            presentHandlers[1].StartFading(toPresent);
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
