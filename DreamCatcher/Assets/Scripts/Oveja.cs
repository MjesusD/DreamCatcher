using UnityEngine;
using System.Collections;

public class Oveja : MonoBehaviour
{
    public float fallSpeed = 3f; // velocidad de caída
    private bool recolectada = false;

    void Update()
    {
        if (!recolectada)
        {
            // Caída constante hacia abajo
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

            // Destruir si sale de la pantalla
            if (transform.position.y < -6f)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!recolectada && collision.CompareTag("Colador"))
        {
            StartCoroutine(Recolectar(collision.transform));
        }
    }

    private IEnumerator Recolectar(Transform colador)
    {
        recolectada = true;

        Colador scriptColador = colador.GetComponent<Colador>();
        if (scriptColador != null)
        {
            bool aceptada = scriptColador.AgregarOveja(transform);

            // Si colador está invertido o lleno, la oveja sigue cayendo
            if (!aceptada)
            {
                recolectada = false;
                yield break;
            }
        }

        // Esperar a que el colador maneje la oveja (recompensa o expulsión)
        yield return null;
    }
}
