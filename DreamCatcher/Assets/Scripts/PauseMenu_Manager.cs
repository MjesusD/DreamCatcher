using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu_Manager : MonoBehaviour
{
    public GameObject pauseMenuUI;
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

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Reactiva el tiempo
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Detiene el tiempo
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reactiva antes de cambiar escena
        SceneManager.LoadScene("MainMenu");
    }
}
