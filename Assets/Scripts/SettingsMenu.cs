#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] AudioMixer audioMixer;
    private Toggle fullscreenToggle;

    public Toggle FullscreenToggle => fullscreenToggle;

#if UNITY_EDITOR
    private EditorWindow window;
    private bool initialToggle = true;
    #endif

    private void Awake() {
        fullscreenToggle = FindObjectOfType<Toggle>();
        
        #if UNITY_EDITOR
        window = EditorWindow.focusedWindow;
        fullscreenToggle.isOn = window.maximized;
        initialToggle = false;
        #endif
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("masterVolume", volume);
    }

    public void SetQuality(int qualityLevel) {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void ToggleFullscreen(bool toFullscreen) {
        #if UNITY_EDITOR
            if (!initialToggle)
                window.maximized = !window.maximized;
        #else
            Screen.fullScreen = toFullscreen;
        #endif
        
    }
}
