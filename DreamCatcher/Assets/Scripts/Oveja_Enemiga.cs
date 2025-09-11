using UnityEngine;

public class Oveja_Enemiga : MonoBehaviour
{
    public float fallSpeed = 3f;
    private bool procesada = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;         // caen controladas, no por gravedad
        rb.freezeRotation = true;
    }

    void Start()
    {
        rb.linearVelocity = Vector2.down * fallSpeed;
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
                    // Rebote con AddForce
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);

                    colador.ModificarPuntaje(1);
                    Destroy(gameObject, 2f); // se destruye tras el rebote
                }
                else
                {
                    // Colador normal, resta puntos
                    colador.ModificarPuntaje(-1);
                    Destroy(gameObject);
                }
            }
        }
    }
}
