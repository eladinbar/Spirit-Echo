using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] AudioMixer audioMixer;
    private Toggle fullscreenToggle;
    private EditorWindow window;
    private bool initialToggle = true;

    private void Awake() {
        fullscreenToggle = FindObjectOfType<Toggle>();
        window = EditorWindow.focusedWindow;
    }

    private void Start() {
        fullscreenToggle.isOn = window.maximized;
        initialToggle = false;
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("masterVolume", volume);
    }

    public void SetQuality(int qualityLevel) {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void FullscreenToggle(bool toFullscreen) {
        #if UNITY_EDITOR
            if (!initialToggle)
                window.maximized = !window.maximized;
        #else
            Screen.fullScreen = toFullscreen;
        #endif
    }
}
