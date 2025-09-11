using UnityEngine;
using System.Collections;

public class Oveja : MonoBehaviour
{
    public float fallSpeed = 3f;
    private bool recolectada = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Start()
    {
        rb.linearVelocity = Vector2.down * fallSpeed;
    }

    void Update()
    {
        // Movimiento manual si no está recolectada
        if (!recolectada)
        {
            rb.linearVelocity = Vector2.down * fallSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!recolectada && collision.CompareTag("Colador"))
        {
            Colador colador = collision.GetComponent<Colador>();
            if (colador != null)
                StartCoroutine(Recolectar(colador));
        }
    }

    private IEnumerator Recolectar(Colador scriptColador)
    {
        if (scriptColador == null) yield break;

        recolectada = true;

        // Intentamos agregar la oveja al colador
        bool aceptada = scriptColador.AgregarOveja(transform);

        // Si no fue aceptada (colador invertido o lleno), vuelve a caer
        if (!aceptada)
        {
            recolectada = false;
            rb.linearVelocity = Vector2.down * fallSpeed;
            yield break;
        }

        // Si fue aceptada, la física y destrucción la maneja el colador
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return null;
    }
}
