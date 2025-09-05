using UnityEngine;
using System.Collections;

public class Cambio_Tiempo : MonoBehaviour
{
    public SpriteRenderer background;   // El fondo a cambiar 
    public Sprite[] backgrounds;        // Imágenes: día, tarde, noche
    public float changeInterval = 10f;  // Tiempo en segundos para cambiar fondo

    private int currentIndex = 0;

    void Start()
    {
        if (backgrounds.Length > 0)
            background.sprite = backgrounds[0]; // Fondo inicial

        StartCoroutine(ChangeBackground());
    }

    IEnumerator ChangeBackground()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);

            currentIndex = (currentIndex + 1) % backgrounds.Length;
            background.sprite = backgrounds[currentIndex];
        }
    }
}
