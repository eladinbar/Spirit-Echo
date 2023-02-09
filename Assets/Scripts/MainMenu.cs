using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    void Start() {
        Cursor.visible = true;
    }
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        // save any game data here
        #if UNITY_EDITOR
            // 'UnityEditor.EditorApplication.isPlaying' needs to be set to false to end the game
            // Since 'Application.Quit()' does not work in the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}