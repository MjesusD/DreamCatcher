using UnityEngine;
using System.Collections;

public class Oveja : MonoBehaviour
{
    public float fallSpeed = 3f;
    private bool recolectada = false;

    void Update()
    {
        if (!recolectada)
        {
            // Caída constante
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

            // Destruir si sale de pantalla
            if (transform.position.y < -6f)
            {
                Destroy(gameObject);
            }
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

            if (!aceptada)
            {
                recolectada = false; // sigue cayendo
                yield break;
            }
        }

        yield return null; // la oveja queda en el colador
    }

}
