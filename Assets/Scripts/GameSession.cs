using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour {
    // [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;
    [SerializeField] float levelLoadDelay = 1.5f;
    
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    
    
    void Awake() {
        int gameSessionCount = FindObjectsOfType<GameSession>().Length;
        if(gameSessionCount > 1)
            Destroy(this.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        // livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
    }

    public void ProcessPlayerDeath() {
        // if (playerLives > 1)
        //     StartCoroutine(TakeLife());
        // else
            StartCoroutine(ResetGameSession());
    }

    public void ReturnToMainMenu() {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }

    private IEnumerator ResetGameSession() {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        ReturnToMainMenu();
    }

    private IEnumerator TakeLife() {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        // playerLives--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // livesText.text = playerLives.ToString();
    }

    public void IncreaseScore(int points) {
        score += points;
        scoreText.text = score.ToString();
    }
}