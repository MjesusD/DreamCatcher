using UnityEngine;
using System.Collections;


public class Fondo_Manager : MonoBehaviour
{
    [Header("Etapas de fondo (arrastrar en orden)")]
    public GameObject[] etapas;       // Cada elemento es un grupo (d�a, tarde, noche)
    public float changeInterval = 10f; // Tiempo en segundos para cambiar de fondo

    private int currentIndex = 0;

    void Start()
    {
        // Activar solo la primera etapa al inicio
        for (int i = 0; i < etapas.Length; i++)
            etapas[i].SetActive(i == 0);

        // Iniciar el ciclo de cambios
        StartCoroutine(ChangeBackground());
    }

    IEnumerator ChangeBackground()
    {
        while (currentIndex < etapas.Length - 1) // Se detiene en la �ltima etapa
        {
            yield return new WaitForSeconds(changeInterval);

            // Desactivar la etapa actual
            etapas[currentIndex].SetActive(false);

            // Avanzar al siguiente �ndice
            currentIndex++;

            // Activar la nueva etapa
            etapas[currentIndex].SetActive(true);
        }
        // Al llegar a la �ltima etapa, se queda activa y no vuelve al inicio
    }
}
