using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour {
    private readonly string prefix = ":";
    
    [SerializeField] float levelLoadDelay = 5f;
    
    [SerializeField] TextMeshProUGUI livesCount;

    
    void Awake() {
        Cursor.visible = false;
        int gameSessionCount = FindObjectsOfType<GameSession>().Length;
        if(gameSessionCount > 1)
            Destroy(this.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        livesCount.text = prefix + PlayerMechanics.Instance.MaxHitPoints.ToString();
    }

    public void UpdateLives(int count) {
        livesCount.text = prefix + count.ToString();
    }
    
    public void ProcessPlayerDeath() {
        StartCoroutine(RestartLevel());
    }
    
    private IEnumerator RestartLevel() {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        livesCount.text = prefix + PlayerMechanics.Instance.MaxHitPoints.ToString();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu() {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }
}