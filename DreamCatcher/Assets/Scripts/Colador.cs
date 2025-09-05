using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Colador : MonoBehaviour
{
    [Header("Movimiento")]
    public Rigidbody2D rb;
    public float moveSpeed = 10f;

    [Header("Slots de captura")]
    public Transform[] slots;
    private Transform[] ocupados;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite invertidoSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Parámetros de juego")]
    public float tiempoTransformarOveja = 3f; // tiempo hasta convertir en "energía de sueño"
    public float fuerzaExpulsion = 8f;        // velocidad al expulsar
    public bool colarDaPuntos = true;         // si quieres sumar puntos al colar nubes

    private float inputValue;
    private bool invertido = false;
    public bool Invertido => invertido;

    private bool bloqueadoPorNube = false;
    private Nube nubeActual;
    private int slotNube = -1; // slot ocupado por la nube

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer && normalSprite) spriteRenderer.sprite = normalSprite;

        ocupados = new Transform[slots.Length];
    }

    void Update()
    {
        // si el nivel terminó, no mover
        if (GameManager.Instance != null && GameManager.Instance.NivelTerminado)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Movimiento horizontal solo si no hay nube bloqueando
        if (!bloqueadoPorNube)
        {
            inputValue = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(inputValue * moveSpeed, 0f);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Sprite invertido con click izquierdo
        if (Input.GetMouseButton(0))
        {
            if (!invertido) { invertido = true; if (spriteRenderer && invertidoSprite) spriteRenderer.sprite = invertidoSprite; }
        }
        else
        {
            if (invertido) { invertido = false; if (spriteRenderer && normalSprite) spriteRenderer.sprite = normalSprite; }
        }

        // Deshacer nube con click derecho + movimiento de mouse
        if (bloqueadoPorNube && nubeActual != null)
        {
            if (Input.GetMouseButton(1) && Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f)
            {
                nubeActual.Deshacer();
                if (nubeActual.Destruida)
                {
                    DestruirNube(); // libera slot y permite mover colador
                }
            }
        }

        // Wrap eje X (pantalla)
        Camera cam = Camera.main;
        if (cam != null)
        {
            float screenHalfWidth = cam.orthographicSize * cam.aspect;
            Vector3 pos = transform.position;

            if (pos.x < -screenHalfWidth) pos.x = screenHalfWidth;
            else if (pos.x > screenHalfWidth) pos.x = -screenHalfWidth;

            transform.position = pos;
        }
    }

    public bool AgregarOveja(Transform oveja)
    {
        if (bloqueadoPorNube) return false;

        // Si está invertido, expulsar (rebote)
        if (invertido)
        {
            Rigidbody2D rbOveja = oveja.GetComponent<Rigidbody2D>();
            if (rbOveja != null) rbOveja.simulated = false;
            StartCoroutine(ExpulsarOveja(oveja, 2f));
            return false;
        }

        // Buscar slot libre
        int slotIndex = -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (ocupados[i] == null) { slotIndex = i; break; }
        }
        if (slotIndex == -1) return false;

        // Colocar oveja en slot
        oveja.SetParent(slots[slotIndex]);
        oveja.localPosition = Vector3.zero;
        ocupados[slotIndex] = oveja;

        // Congelar física y comportamiento de la oveja
        Rigidbody2D rbOveja2 = oveja.GetComponent<Rigidbody2D>();
        if (rbOveja2 != null) rbOveja2.simulated = false;
        var scriptOveja = oveja.GetComponent<MonoBehaviour>();
        if (scriptOveja != null) scriptOveja.enabled = false;

        // Tras delay, convertir en "energía de sueño"
        StartCoroutine(RecompensarOveja(oveja, slotIndex, tiempoTransformarOveja));
        return true;
    }

    private IEnumerator RecompensarOveja(Transform oveja, int slotIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (oveja != null)
        {
            // Avisar al GameManager que fue una oveja buena
            if (GameManager.Instance != null)
                GameManager.Instance.OvejaBuena();

            Destroy(oveja.gameObject);
        }
        ocupados[slotIndex] = null;
    }

    private IEnumerator ExpulsarOveja(Transform oveja, float duracion)
    {
        float timer = 0f;
        while (timer < duracion && oveja != null)
        {
            oveja.Translate(Vector2.up * fuerzaExpulsion * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        if (oveja != null) Destroy(oveja.gameObject);
    }

    // Reenvío para scripts existentes que ya llamaban al Colador
    public void ModificarPuntaje(int cantidad)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ModificarPuntaje(cantidad);
    }

    // Llamado desde la nube al caer sobre colador
    public void BloquearPorNube(Nube nube)
    {
        // buscar slot vacío para la nube
        for (int i = 0; i < slots.Length; i++)
        {
            if (ocupados[i] == null)
            {
                ocupados[i] = nube.transform;
                slotNube = i;
                break;
            }
        }
        bloqueadoPorNube = true;
        nubeActual = nube;

        if (slotNube >= 0)
        {
            nube.transform.SetParent(slots[slotNube]);
            nube.transform.localPosition = Vector3.zero;
        }
    }

    // Liberar slot de la nube y permitir movimiento
    public void DestruirNube()
    {
        bloqueadoPorNube = false;
        if (slotNube >= 0)
        {
            ocupados[slotNube] = null;
            slotNube = -1;
        }

        // (Opcional) dar puntos por colar nube
        if (colarDaPuntos && GameManager.Instance != null)
            GameManager.Instance.NubeColada();

        nubeActual = null;
    }
}
