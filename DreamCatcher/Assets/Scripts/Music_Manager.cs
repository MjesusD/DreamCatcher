using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Music_Manager : MonoBehaviour
{
    public static Music_Manager Instance;

    private AudioSource audioSource;

    [Header("Clips de Música")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        // Restaurar configuración guardada
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        int enabled = PlayerPrefs.GetInt("MusicEnabled", 1);

        audioSource.volume = volume;
        if (enabled == 1) audioSource.Play();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenuScene":
                CambiarMusica(mainMenuMusic);
                break;
            case "GameScene":
                CambiarMusica(gameSceneMusic);
                break;
            case "VictoryScene":
                CambiarMusica(victoryMusic);
                break;
            case "DefeatScene":
                CambiarMusica(defeatMusic);
                break;
        }
    }

    private void CambiarMusica(AudioClip clip)
    {
        if (clip == null || audioSource.clip == clip) return;

        audioSource.clip = clip;

        // Solo reproducir si la música está habilitada
        int enabled = PlayerPrefs.GetInt("MusicEnabled", 1);
        if (enabled == 1)
        {
            audioSource.Play();
        }
    }

    // Métodos accesibles desde Music_Settings
    public void SetVolume(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            if (!audioSource.isPlaying) audioSource.Play();
            PlayerPrefs.SetInt("MusicEnabled", 1);
        }
        else
        {
            audioSource.Pause();
            PlayerPrefs.SetInt("MusicEnabled", 0);
        }
    }
}
