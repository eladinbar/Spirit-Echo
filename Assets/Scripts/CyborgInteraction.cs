using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CyborgInteraction : MonoBehaviour {
    private PlayerInput playerInput;

    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] Image spaceImage;
    [SerializeField] TextMeshProUGUI continueText;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] List<Image> tutorialImages;

    private int currentIndex = 0;

    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
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
        playerInput.DeactivateInput();
        ShowNextText();
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShowNextText();
        }
    }

    private void ShowNextText() {
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
            Destroy(this.gameObject);
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