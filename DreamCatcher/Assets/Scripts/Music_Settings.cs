using UnityEngine;
using UnityEngine.UI;

public class Music_Settings : MonoBehaviour
{
    public Toggle toggleMusic;
    public Slider sliderVolume;

    void OnEnable()
    {
        // Al abrir el panel sincronizar con el estado actual
        if (Music_Manager.Instance != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            int enabled = PlayerPrefs.GetInt("MusicEnabled", 1);

            sliderVolume.value = savedVolume;
            toggleMusic.isOn = (enabled == 1);
        }
    }

    void Start()
    {
        // Listeners
        toggleMusic.onValueChanged.AddListener(OnToggleMusic);
        sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnToggleMusic(bool value)
    {
        if (Music_Manager.Instance != null)
            Music_Manager.Instance.SetEnabled(value);
    }

    private void OnVolumeChanged(float value)
    {
        if (Music_Manager.Instance != null)
            Music_Manager.Instance.SetVolume(value);
    }
}
