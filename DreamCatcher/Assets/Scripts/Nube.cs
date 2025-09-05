using UnityEngine;

public class Nube : MonoBehaviour
{
    [HideInInspector] public Nube_Manager manager;

    private bool cayendo = false;
    private bool bloqueando = false; // bloquea colador al caer
    private bool deshaciendo = false; // se destruye poco a poco
    private float fallSpeed;
    private Rigidbody2D rb;
    private float shrinkSpeed = 3f;

    public bool Destruida { get; private set; } = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // quieta por defecto
    }

    void Update()
    {
        // Movimiento hacia abajo si está cayendo y no bloquea ni se destruye
        if (cayendo && !bloqueando && !deshaciendo)
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // Animación de deshacer/destruir
        if (deshaciendo)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);

            // Cuando casi desaparece, notificar manager y liberar colador
            if (transform.localScale.magnitude < 0.05f)
            {
                Destruida = true;
                NotificarEliminacion();
            }
        }
    }

    public void ActivarCaida(float speed)
    {
        fallSpeed = speed;
        cayendo = true;
    }

    public void Resetear(Vector3 nuevaPos)
    {
        transform.position = nuevaPos;
        transform.localScale = Vector3.one;
        cayendo = false;
        bloqueando = false;
        deshaciendo = false;
        Destruida = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.SetActive(true);
    }

    // Bloquea colador y ocupa un slot
    public void BloquearColador(Colador colador)
    {
        cayendo = false;
        bloqueando = true;
        rb.bodyType = RigidbodyType2D.Static;

        if (colador != null)
            colador.BloquearPorNube(this);
    }

    // Empieza a deshacerse y libera el colador
    public void Deshacer()
    {
        if (!deshaciendo)
        {
            deshaciendo = true;
            bloqueando = false;
        }
    }

    // Notifica al manager antes de destruir el objeto
    private void NotificarEliminacion()
    {
        // Liberar colador antes de destruir
        Colador col = FindAnyObjectByType<Colador>();
        if (col != null && bloqueando == false)
            col.DestruirNube();

        // Notificar al manager
        if (manager != null)
            manager.NubeEliminada(this);

        Destroy(gameObject);
    }

    public bool EstaCayendo { get { return cayendo; } }
    public bool EstaAtrapada { get { return bloqueando; } }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Colador") && !bloqueando)
        {
            Colador col = other.GetComponent<Colador>();
            if (col != null)
                BloquearColador(col);
        }
    }
}
