using UnityEngine;

public class Nube : MonoBehaviour
{
    [HideInInspector] public Nube_Manager manager;

    private bool cayendo = false;
    private bool bloqueando = false;
    private bool deshaciendo = false;
    private float fallSpeed;
    private Rigidbody2D rb;
    private float shrinkSpeed = 3f;

    private Colador coladorActual;

    public bool Destruida { get; private set; } = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // Caída libre
        if (cayendo && !bloqueando && !deshaciendo)
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // Animación de deshacer
        if (deshaciendo)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);

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
        coladorActual = null;
        gameObject.SetActive(true);
    }

    // Bloquea colador y ocupa un slot
    public void BloquearColador(Colador colador)
    {
        if (bloqueando) return;

        cayendo = false;
        bloqueando = true;
        rb.bodyType = RigidbodyType2D.Static;

        coladorActual = colador;
        colador?.BloquearPorNube(this);
    }

    // Empieza a deshacerse y libera colador inmediatamente
    public void Deshacer()
    {
        if (deshaciendo) return;

        deshaciendo = true;
        bloqueando = false;

        if (coladorActual != null)
        {
            coladorActual.DestruirNube();
            coladorActual = null;
        }
    }

    private void NotificarEliminacion()
    {
        manager?.NubeEliminada(this);
        Destroy(gameObject);
    }

    public bool EstaCayendo => cayendo;
    public bool EstaAtrapada => bloqueando;

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
