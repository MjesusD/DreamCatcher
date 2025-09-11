using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    // Carga una escena por nombre
    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Reintentar juego
    public void ReintentarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Salir del juego
    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Salir del juego"); 
    }

    
}
