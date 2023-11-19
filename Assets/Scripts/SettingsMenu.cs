#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    const string MASTER_VOLUME_NAME = "masterVolume";
    const string VOICE_OVER_VOLUME_NAME = "voiceVolume";
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] AudioMixer voiceMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider voiceVolumeSlider;
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

        SetVolume(PlayerPrefs.GetFloat(MASTER_VOLUME_NAME));
        SetVoiceVolume(PlayerPrefs.GetFloat(VOICE_OVER_VOLUME_NAME));

        volumeSlider.value = GetVolume();
        voiceVolumeSlider.value = GetVoiceVolume();
    }

    public float GetVolume() {
        mainMixer.GetFloat(MASTER_VOLUME_NAME, out float volume);
        return volume;
    }

    public float GetVoiceVolume() {
        voiceMixer.GetFloat(VOICE_OVER_VOLUME_NAME, out float volume);
        return volume;
    }

    public void SetVolume(float volume) {
        mainMixer.SetFloat(MASTER_VOLUME_NAME, volume);
        PlayerPrefs.SetFloat(MASTER_VOLUME_NAME, volume);
    }

    public void SetVoiceVolume(float volume) {
        voiceMixer.SetFloat(VOICE_OVER_VOLUME_NAME, volume);
        PlayerPrefs.SetFloat(VOICE_OVER_VOLUME_NAME, volume);
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
