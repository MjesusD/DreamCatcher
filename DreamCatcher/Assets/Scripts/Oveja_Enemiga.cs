using UnityEngine;
using System.Collections;

public class Oveja_Enemiga : MonoBehaviour
{
    public float fallSpeed = 3f;
    private bool procesada = false;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        if (transform.position.y < -6f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (procesada) return;

        if (collision.CompareTag("Colador"))
        {
            Colador colador = collision.GetComponent<Colador>();
            if (colador != null)
            {
                procesada = true;
                if (colador.Invertido)
                {
                    // Colador invertido - sumar puntos
                    StartCoroutine(Expulsar(colador, 1));
                }
                else
                {
                    // Colador normal - restar puntos
                    colador.ModificarPuntaje(-1);
                    Destroy(gameObject);
                }
            }
        }
    }

    private IEnumerator Expulsar(Colador colador, int puntos)
    {
        float timer = 0f;
        while (timer < 1.5f)
        {
            transform.Translate(Vector2.up * 8f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        colador.ModificarPuntaje(puntos);
        Destroy(gameObject);
    }
}
