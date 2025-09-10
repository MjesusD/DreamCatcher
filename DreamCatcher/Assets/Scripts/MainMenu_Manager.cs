using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Manager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;   // Menú principal completo
    public GameObject musicPanel;      // Panel de música
    public GameObject controlsPanel;   // Panel de controles
    public GameObject overlay;         // Fondo

    void Start()
    {
        // Estados iniciales
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (musicPanel != null) musicPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (overlay != null) overlay.SetActive(false);
    }

    // Botones del menú principal 
    public void PlayGame() => SceneManager.LoadScene("GameScene");

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    // Panel de música 
    public void AbrirPanelMusica()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (musicPanel != null) musicPanel.SetActive(true);
        if (overlay != null) overlay.SetActive(true);
    }

    public void CerrarPanelMusica()
    {
        if (musicPanel != null) musicPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (overlay != null) overlay.SetActive(false);
    }

    // Panel de controles
    public void AbrirPanelControles()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
        if (overlay != null) overlay.SetActive(true);
    }

    public void CerrarPanelControles()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (overlay != null) overlay.SetActive(false);
    }
}
