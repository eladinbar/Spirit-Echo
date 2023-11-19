using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    const string MASTER_VOLUME_NAME = "masterVolume";
    const string VOICE_OVER_VOLUME_NAME = "voiceVolume";

    [SerializeField] GameObject settingsPanel;

    void Start() {
        Cursor.visible = true;
        
        SettingsMenu menu = settingsPanel.GetComponent<SettingsMenu>();
        
        if(!PlayerPrefs.HasKey("masterVolume"))
            PlayerPrefs.SetFloat("masterVolume", -25f);
        if(!PlayerPrefs.HasKey("voiceVolume"))
            PlayerPrefs.SetFloat("voiceVolume", 0f);

        settingsPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Cursor.visible = false;
    }

    public void Credits() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 6);
    }

    public void QuitGame() {
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