using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TheJourneyBegins : MonoBehaviour {
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int Punching = Animator.StringToHash("Punch");
    private static readonly int Kicking = Animator.StringToHash("Kick");
    
    private PlayerInput playerInput;
    private Animator playerAnimator;
    private Rigidbody2D playerRigidbody;
    
    private bool finishedDisplaying;
    int currentIndex = 0;
    
    // Input checks
    private int lastInput = 0;
    private int currentInput = 0;
    int keyPressCount = 0;
    [SerializeField] int keyPressesRequired = 2;
    private Vector2 moveInput;
    private bool isPressed;

    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] Image spaceImage;
    [SerializeField] TextMeshProUGUI continueText;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] List<Image> tutorialImages;
    [SerializeField] List<AudioClip> voiceOvers;
    
    AudioSource audioSource;

    void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        playerAnimator = PlayerMechanics.Instance.GetComponent<Animator>();
        playerRigidbody = PlayerMechanics.Instance.GetComponent<Rigidbody2D>();
        audioSource = this.GetComponent<AudioSource>();
        // Initialize all text elements
        foreach (TextMeshProUGUI text in textList)
            text.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(false);
        foreach (Image image in tutorialImages)
            image.gameObject.SetActive(false);
        // Fade in from black
        // Display initial capsule canvas
        ShowNextText();
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart() {
        // Ensure all other start functions have finished executing
        yield return new WaitForSeconds(Mathf.Epsilon);
        // Revert initial animation sprite to 'Idle'
        playerRigidbody.velocity = Vector2.zero;
        playerAnimator.SetBool(IsJumping, false);
        // Stop physics and input
        PlayerMechanics.Instance.moveEnabled = false;
        PlayerMechanics.Instance.jumpEnabled = false;
        playerRigidbody.isKinematic = true;
        playerInput.DeactivateInput();
    }

    private void Update() {
        if ((Input.GetKeyDown(KeyCode.Z) || (isPressed && moveInput is { y: > 0 })) && !finishedDisplaying) {
            isPressed = false;
            playerRigidbody.velocity = Vector2.zero;
            ShowNextText();
        }

        else if (finishedDisplaying && keyPressCount < keyPressesRequired) {
            // Reactivate input for animation
            playerInput.ActivateInput();
            playerRigidbody.velocity = Vector2.zero;
            
            int animatorState = keyPressCount == 0 ? Punching : Kicking;

            // Check for alternating left/right key presses
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || moveInput is { x: < 0 }) {
                // Handle left arrow key press
                currentInput = -1;
                PlayerMechanics.Instance.transform.localScale = new Vector3(-1, 1 ,1);
                playerAnimator.SetTrigger(animatorState);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || moveInput is { x: > 0 }) {
                // Handle right arrow key press
                currentInput = 1;
                PlayerMechanics.Instance.transform.localScale = Vector3.one;
                playerAnimator.SetTrigger(animatorState);
            }

            // Check for alternating key presses
            if (currentInput != 0 && currentInput == lastInput * -1) {
                keyPressCount++;
            }

            // Update last input
            lastInput = currentInput;
        }

        if (finishedDisplaying && keyPressCount >= keyPressesRequired) {
            // Wait for left right input
            playerRigidbody.isKinematic = false;
            PlayerMechanics.Instance.moveEnabled = true;
            PlayerMechanics.Instance.jumpEnabled = true;
            Destroy(this.gameObject);
        }
    }
    
    private void ShowNextText() {
        if (currentIndex >= textList.Count) {
            textList[currentIndex - 1].gameObject.SetActive(false);
            spaceImage.gameObject.SetActive(false);
            continueText.gameObject.SetActive(false);
            if(tutorialText)
                tutorialText.gameObject.SetActive(true);
            foreach (Image image in tutorialImages)
                image.gameObject.SetActive(true);
            finishedDisplaying = true;
        }
        else {
            if (currentIndex > 0) {
                textList[currentIndex - 1].gameObject.SetActive(false);
            }
            
            if (currentIndex < voiceOvers.Count) {
                audioSource.clip = voiceOvers[currentIndex];
                audioSource.Play();
            }
            textList[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
    }

    private void OnNavigate(InputValue value) {
        moveInput = value.Get<Vector2>();
        isPressed = true;
    }
}
