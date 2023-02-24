using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour {
    private static bool gameIsPaused = false;
    public static bool GameIsPaused => gameIsPaused;

    private GameSession gameSession;
    private PlayerInput playerInput;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject settingsPanel;
    private SettingsMenu settingsMenu;

    void Start() {
        gameSession = FindObjectOfType<GameSession>();
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
        settingsMenu = settingsPanel.GetComponent<SettingsMenu>();
        
        //Initialize settings menu
        Pause();
        settingsPanel.SetActive(true);
        settingsMenu.SetVolume(-10f);
        Resume();
        settingsPanel.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F11))
            settingsMenu.ToggleFullscreen(!settingsMenu.FullscreenToggle.isOn);

        if(Input.GetKeyDown(KeyCode.Escape))
            if (gameIsPaused)
                Resume();
            else
                Pause();
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1f;
        playerInput.ActivateInput();
        gameIsPaused = false;
    }

    void Pause() {
        settingsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0f;
        playerInput.DeactivateInput();
        gameIsPaused = true;
    }

    public void LoadMenu() {
        Time.timeScale = 1f;
        gameSession.ReturnToMainMenu();
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