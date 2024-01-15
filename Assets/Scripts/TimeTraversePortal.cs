using UnityEngine;
using UnityEngine.InputSystem;

public class TimeTraversePortal : MonoBehaviour {
    private GameObject instructionCanvas;
    private bool canUse;
    
    private PlayerInput playerInput;
    private Vector2 moveInput;

    private void Awake() {
        //TimeTraversePortal is expected to have 1 child only
        instructionCanvas = this.gameObject.transform.GetChild(0).gameObject;
        instructionCanvas.SetActive(false);
    }

    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        PlayerMechanics.Instance.onMove.AddListener(OnMove);
    }
    
    private void Update() {
        if (canUse && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || moveInput is { y: > 0 }))
            TraverseTime();
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player") && playerInput.inputIsActive) {
            instructionCanvas.SetActive(true);
            canUse = true;
        } else {
            instructionCanvas.SetActive(false);
            canUse = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            instructionCanvas.SetActive(false);
            canUse = false;
        }
    }
    
    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void TraverseTime() {
        PlayerMechanics.Instance.unlockedTimeTraversal = true;
        PlayerMechanics.Instance.OnTraverseTime();
    }
}
