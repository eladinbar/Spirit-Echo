using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplayStoryText : MonoBehaviour {
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private bool isPressed;

    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] Image spaceImage;
    [SerializeField] TextMeshProUGUI continueText;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] List<Image> tutorialImages;
    [SerializeField] List<AudioClip> voiceOvers;
    [SerializeField] GameObject _gameObject;
    
    AudioSource audioSource;

    private int currentIndex = 0;
    private bool triggered;
    private bool traversed;
    private bool isPresent = true;

    // Specific check
    private int sceneIndex;
    
    // Revelation level
    static bool keyFlag;
    static bool doubleJumpFlag;
    static bool farLedge;
    
    // Wall jump level
    [SerializeField] Level3 _level3;
    
    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        PlayerMechanics.Instance.onTraverseTime.AddListener(OnTraverseTime);
        audioSource = this.GetComponent<AudioSource>();
        // Specific check
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    private void OnEnable() {
        foreach (TextMeshProUGUI text in textList) {
            text.gameObject.SetActive(false);
        }

        if (currentIndex < textList.Count) {
            tutorialText.gameObject.SetActive(false);
            spaceImage.gameObject.SetActive(false);
            continueText.gameObject.SetActive(false);
            foreach (Image image in tutorialImages)
                image.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Specific check
        // The Journey Begins
        if (sceneIndex == 1 && this.transform.position.x == -5.079998f) {
            float xPosition = PlayerMechanics.Instance.GetPosition().x;
            bool inPosition = isPresent && traversed && xPosition is >= -7f and <= 2.5f;
            if (other.CompareTag("Player") && inPosition && !triggered) {
                triggered = true;
                ShowNextText();
            }
        }
        
        // Revelation
        // Ability Panel
        else if (sceneIndex == 2 && this.transform.position.x is >= -30f and <= -25f) {
            if (other.CompareTag("Player") && keyFlag) {
                doubleJumpFlag = true;
                PlayerMechanics.Instance.unlockedDoubleJump = true;
                triggered = true;
                ShowNextText();
            }
        }
        
        // Double Jump/Far Ledge
        else if (sceneIndex == 2 && this.transform.position.x is >= -1f and <= 3f) {
            if (textList.Count == 2 && !doubleJumpFlag) {
                farLedge = true;
                triggered = true;
                ShowNextText();
            } else if(textList.Count == 1 && doubleJumpFlag) {
                triggered = true;
                ShowNextText();
            }
        }
        // End of specific check
        else if (other.CompareTag("Player") && !triggered) {
            triggered = true;
            ShowNextText();
        }
    }

    private void OnTraverseTime() {
        traversed = true;
        isPresent = !isPresent;
    }
    
    private void OnNavigate(InputValue value) {
        moveInput = value.Get<Vector2>();
        isPressed = true;
    }

    private void Update() {
        if (triggered && (Input.GetKeyDown(KeyCode.Z) || (isPressed && moveInput is { y: > 0 }))) {
            isPressed = false;
            ShowNextText();
        }
    }

    private void ShowNextText() {
        // Specific check
        // Revelation level
        // Key
        if (sceneIndex == 2 && this.transform.position.x is >= -45f and <= -40f && currentIndex >= textList.Count) {
            keyFlag = true;
        }
        
        // Ability Panel
        else if (sceneIndex == 2 && this.transform.position.x is >= -30f and <= -25f && !farLedge && currentIndex == 5) {
            textList[currentIndex - 1].gameObject.SetActive(false);
            currentIndex = textList.Count;
        }

        // Wall jump level
        // Donny
        else if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= -10f and <= -2f && currentIndex == 6) {
            _level3.trigger();
        }
        
        // Viver 2nd meeting
        else if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= 110f and <= 130f && currentIndex >= textList.Count) {
            _level3.trigger();
        }
        
        // Viver 1st meeting
        else if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= 50f and <= 65f && currentIndex == 8) {
            _level3.trigger();
        }
        
        if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= 50f and <= 65f && currentIndex >= textList.Count) {
            _level3.trigger();
        }

        
        // End specific check
        
        if (currentIndex >= textList.Count) {
            textList[currentIndex - 1].gameObject.SetActive(false);
            if(_gameObject)
                Destroy(_gameObject);
            spaceImage.gameObject.SetActive(false);
            continueText.gameObject.SetActive(false);
            playerInput.ActivateInput();
            if (tutorialText) {
                tutorialText.gameObject.SetActive(true);
                foreach (Image image in tutorialImages)
                    image.gameObject.SetActive(true);
            }

            triggered = false;
        }
        else {
            playerInput.DeactivateInput();
            if (currentIndex > 0) {
                textList[currentIndex - 1].gameObject.SetActive(false);
            }
            else {
                spaceImage.gameObject.SetActive(true);
                continueText.gameObject.SetActive(true);
            }

            if (currentIndex < voiceOvers.Count) {
                audioSource.clip = voiceOvers[currentIndex];
                audioSource.Play();
            }
            textList[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
    }
}