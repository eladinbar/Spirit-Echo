using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplayStoryText : MonoBehaviour {
    private PlayerInput playerInput;
    
    public UnityEvent trigger;

    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] Image spaceImage;
    [SerializeField] TextMeshProUGUI continueText;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] List<Image> tutorialImages;

    private int currentIndex = 0;
    private bool triggered;
    private bool traversed;
    private bool isPresent = true;

    // Specific check
    private int sceneIndex;
    Level3 _level3;
    
    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        PlayerMechanics.Instance.onTraverseTime.AddListener(OnTraverseTime);
        // Specific check
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        _level3 = FindObjectOfType<Level3>();
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
        // Specific check, can be expanded upon
        if (sceneIndex == 1 && this.transform.position.x == -5.079998f) {
            float xPosition = PlayerMechanics.Instance.GetPosition().x;
            bool inPosition = isPresent && traversed && xPosition is >= -7f and <= 2.5f;
            if (other.CompareTag("Player") && inPosition && !triggered) {
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

    private void Update() {
        if (triggered && Input.GetKeyDown(KeyCode.Space)) {
            ShowNextText();
        }
    }

    private void ShowNextText() {
        // Specific check
        // Omer's level
        if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= -10f and <= -2f && currentIndex == 6) {
            _level3.trigger();
        }
        
        else if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= 50f and <= 65f && currentIndex == 8) {
            _level3.trigger();
        }

        else if (sceneIndex == 3 && PlayerMechanics.Instance.GetPosition().x is >= 110f and <= 130f && currentIndex >= textList.Count) {
            _level3.trigger();
        }
        // Specific check
        
        if (currentIndex >= textList.Count) {
            textList[currentIndex - 1].gameObject.SetActive(false);
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

            textList[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
    }
}