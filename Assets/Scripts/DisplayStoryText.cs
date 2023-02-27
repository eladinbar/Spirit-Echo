using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DisplayStoryText : MonoBehaviour {
    private PlayerInput playerInput;
    private int triggerID;

    [SerializeField] Canvas canvas;
    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] TextMeshProUGUI tutorialText;

    private int currentIndex = 0;
    private bool triggered;
    private bool traversed;
    private bool isPresent = true;

    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        triggerID = this.gameObject.GetComponent<BoxCollider2D>().GetInstanceID();
        PlayerMechanics.Instance.onTraverseTime.AddListener(OnTraverseTime);
    }

    private void OnEnable() {
        foreach (TextMeshProUGUI text in textList) {
            text.gameObject.SetActive(false);
        }
        if (currentIndex < textList.Count)
            tutorialText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Specific check, can be expanded upon
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
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
        if (currentIndex >= textList.Count) {
            textList[currentIndex - 1].gameObject.SetActive(false);
            playerInput.ActivateInput();
            if(tutorialText)
                tutorialText.gameObject.SetActive(true);
            triggered = false;
        }
        else {
            playerInput.DeactivateInput();
            if (currentIndex > 0) {
                textList[currentIndex - 1].gameObject.SetActive(false);
            }
            textList[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
    }
}