using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {
    private PlayerInput playerInput;
    
    [SerializeField] float levelLoadDelay = 1.5f;

    private void Start() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
            StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel() {
        GetComponent<AudioSource>().Play();
        playerInput.DeactivateInput();
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        playerInput.ActivateInput();
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}