using UnityEngine;
using System.Collections;

public class Colador : MonoBehaviour
{
    [Header("Movimiento")]
    public Rigidbody2D rb;
    public float moveSpeed = 10f;

    [Header("Slots compartidos")]
    public Transform[] slots; // Slots compartidos para ovejas y nubes

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite invertidoSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Parámetros de juego")]
    public float tiempoTransformarOveja = 3f;
    public float fuerzaExpulsion = 8f;
    public bool colarDaPuntos = true;

    private bool invertido = false;
    public bool Invertido => invertido;

    private bool bloqueadoPorNube = false;
    private Nube nubeActual;
    private int slotActual = -1;

    private Transform[] ocupados; // Slots ocupados

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer && normalSprite)
            spriteRenderer.sprite = normalSprite;

        ocupados = new Transform[slots.Length];
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.NivelTerminado)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Movimiento horizontal
        rb.linearVelocity = bloqueadoPorNube ? Vector2.zero : new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, 0f);

        // Sprite invertido y expulsión de ovejas
        if (Input.GetMouseButton(0))
        {
            if (!invertido)
            {
                invertido = true;
                if (spriteRenderer && invertidoSprite) spriteRenderer.sprite = invertidoSprite;
                ExpulsarOvejas(); // expulsar todas las ovejas en slots al invertir
            }
        }
        else if (invertido)
        {
            invertido = false;
            if (spriteRenderer && normalSprite) spriteRenderer.sprite = normalSprite;
        }

        // Tamizar nube con click derecho + mover mouse
        if (bloqueadoPorNube && nubeActual != null && Input.GetMouseButton(1) && Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f)
        {
            nubeActual.Deshacer();
            DestruirNube();
        }

        // Wrap horizontal
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
        if (invertido || bloqueadoPorNube) // si está invertido o hay nube
        {
            Rigidbody2D rbOveja = oveja.GetComponent<Rigidbody2D>();
            if (rbOveja != null)
            {
                rbOveja.simulated = true;
                rbOveja.linearVelocity = Vector2.zero;
                rbOveja.AddForce(Vector2.up * fuerzaExpulsion, ForceMode2D.Impulse);
            }
            return false;
        }

        int slotIndex = -1;
        for (int i = 0; i < slots.Length; i++)
            if (ocupados[i] == null) { slotIndex = i; break; }

        if (slotIndex == -1) return false;

        // Colocar oveja en slot
        oveja.SetParent(slots[slotIndex]);
        oveja.localPosition = Vector3.zero;
        ocupados[slotIndex] = oveja;

        Rigidbody2D rbOveja2 = oveja.GetComponent<Rigidbody2D>();
        if (rbOveja2 != null) rbOveja2.simulated = false;

        MonoBehaviour scriptOveja = oveja.GetComponent<MonoBehaviour>();
        if (scriptOveja != null) scriptOveja.enabled = false;

        StartCoroutine(RecompensarOveja(oveja, slotIndex, tiempoTransformarOveja));
        return true;
    }

    private IEnumerator RecompensarOveja(Transform oveja, int slotIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (oveja != null && GameManager.Instance != null)
            GameManager.Instance.OvejaBuena();
        if (oveja != null) Destroy(oveja.gameObject);
        ocupados[slotIndex] = null;
    }

    private void ExpulsarOvejas()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Transform t = ocupados[i];
            if (t != null && t.GetComponent<Oveja>() != null)
            {
                Rigidbody2D rbOveja = t.GetComponent<Rigidbody2D>();
                if (rbOveja != null)
                {
                    rbOveja.simulated = true;
                    rbOveja.linearVelocity = Vector2.zero;
                    rbOveja.AddForce(Vector2.up * fuerzaExpulsion, ForceMode2D.Impulse);
                }

                // Activar script de Oveja para que no recompense
                MonoBehaviour scriptOveja = t.GetComponent<MonoBehaviour>();
                if (scriptOveja != null) scriptOveja.enabled = true;

                ocupados[i] = null;
            }
        }
    }

    public void ModificarPuntaje(int cantidad)
{
    if (GameManager.Instance != null)
        GameManager.Instance.ModificarPuntaje(cantidad);
}
    public void BloquearPorNube(Nube nube)
    {
        bloqueadoPorNube = true;
        nubeActual = nube;

        slotActual = -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (ocupados[i] == null)
            {
                nube.transform.SetParent(slots[i]);
                nube.transform.localPosition = Vector3.zero;
                ocupados[i] = nube.transform;
                slotActual = i;
                break;
            }
        }
    }

    public void DestruirNube()
    {
        bloqueadoPorNube = false;

        if (slotActual >= 0)
        {
            slots[slotActual].DetachChildren();
            ocupados[slotActual] = null;
            slotActual = -1;
        }

        if (colarDaPuntos && GameManager.Instance != null)
            GameManager.Instance.NubeColada();

        nubeActual = null;
    }
}
