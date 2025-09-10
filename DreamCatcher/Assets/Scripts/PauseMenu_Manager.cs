using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu_Manager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;   // Menú de pausa
    public GameObject gameplayUI;    // UI del nivel (puntos, tiempo, etc.)

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // Botón Continuar
    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);

        Time.timeScale = 1f; // Reactiva el tiempo
        isPaused = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);

        Time.timeScale = 0f; // Detiene el tiempo
        isPaused = true;
    }

   
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reactiva el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu"); 
    }
}
