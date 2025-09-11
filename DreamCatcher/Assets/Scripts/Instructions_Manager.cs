using UnityEngine;

public class Instructions_Manager : MonoBehaviour
{
    public GameObject instruccionesPanel;

    void Start()
    {
        if (instruccionesPanel != null)
            instruccionesPanel.SetActive(true);

        // Pausar el juego mientras est�n las instrucciones
        Time.timeScale = 0f;
    }

    // Bot�n de continuar
    public void ContinuarJuego()
    {
        if (instruccionesPanel != null)
            instruccionesPanel.SetActive(false);

        // Reanudar el juego
        Time.timeScale = 1f;
    }
}
