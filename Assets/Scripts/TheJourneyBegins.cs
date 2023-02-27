using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TheJourneyBegins : MonoBehaviour {
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private PlayerInput playerInput;
    private Animator playerAnimator;
    
    private bool finishedDisplaying;
    int currentIndex = 0;
    
    // Input checks
    private int lastInput = 0;
    private int currentInput = 0;
    int keyPressCount = 0;
    [SerializeField] int keyPressesRequired = 3;

    
    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] TextMeshProUGUI tutorialText;

    void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        playerAnimator = PlayerMechanics.Instance.GetComponent<Animator>();
        // Initialize all text elements
        foreach (TextMeshProUGUI text in textList) {
            text.gameObject.SetActive(false);
        }
        tutorialText.gameObject.SetActive(false);
        // Fade in from black
        // Display initial capsule canvas
        ShowNextText();
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart() {
        // Ensure all other start functions have finished executing
        yield return new WaitForSeconds(Mathf.Epsilon);
        // Revert initial animation sprite to 'Idle'
        PlayerMechanics.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerMechanics.Instance.GetComponent<Animator>().SetBool(IsJumping, false);
        // Stop time and input
        Time.timeScale = 0f;
        playerInput.DeactivateInput();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !finishedDisplaying) {
            ShowNextText();
        }

        else if (finishedDisplaying && keyPressCount < keyPressesRequired) {
            // Check for alternating left/right key presses
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                // Handle left arrow key press
                currentInput = -1;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                // Handle right arrow key press
                currentInput = 1;
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
            PlayerMechanics.Instance.GetComponent<Rigidbody2D>().isKinematic = false;
            Time.timeScale = 1f;
            playerInput.ActivateInput();
            Destroy(this.gameObject);
        }
    }
    
    private void ShowNextText() {
        if (currentIndex >= textList.Count) {
            textList[currentIndex - 1].gameObject.SetActive(false);
            if(tutorialText)
                tutorialText.gameObject.SetActive(true);
            finishedDisplaying = true;
        }
        else {
            if (currentIndex > 0) {
                textList[currentIndex - 1].gameObject.SetActive(false);
            }
            textList[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
    }
}
