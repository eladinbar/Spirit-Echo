using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour {
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

    }
    public void livesUpdate(int count){
        print(count);
        livesCount.text=count.ToString();
    }

    public void ReturnToMainMenu() {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }
}