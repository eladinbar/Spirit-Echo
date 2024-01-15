using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private GameObject instructionCanvas;
    private bool canExit;
    
    [SerializeField] float levelLoadDelay = 1.5f;

    private void Awake() {
        //LevelExit is expected to have 1 child only
        instructionCanvas = this.gameObject.transform.GetChild(0).gameObject;
        instructionCanvas.SetActive(false);
    }

    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        PlayerMechanics.Instance.onMove.AddListener(OnMove);
    }

    private void Update() {
        if (canExit && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || moveInput is { y: > 0 }))
            StartCoroutine(LoadNextLevel());
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            instructionCanvas.SetActive(true);
            canExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            instructionCanvas.SetActive(false);
            canExit = false;
        }
    }

    private IEnumerator LoadNextLevel() {
        GetComponent<AudioSource>().Play();
        playerInput.DeactivateInput();
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        playerInput.ActivateInput();
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }
}