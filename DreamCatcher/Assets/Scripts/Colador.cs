using UnityEngine;
using System.Collections;
using TMPro;

public class Colador : MonoBehaviour
{
    [Header("Movimiento")]
    public Rigidbody2D rb;
    public float moveSpeed = 10f;

    [Header("Puntaje")]
    public int puntaje = 0;
    public TMP_Text textoPuntaje;

    [Header("Slots")]
    public Transform[] slots;
    private Transform[] ocupados;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite invertidoSprite;
    private SpriteRenderer spriteRenderer;

    private float inputValue;
    private bool invertido = false;


    void Start()
    {
        ocupados = new Transform[slots.Length];
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && normalSprite != null)
            spriteRenderer.sprite = normalSprite;

        ActualizarTexto();
    }

    void Update()
    {
        // Movimiento horizontal
        inputValue = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputValue * moveSpeed, 0f);

        // Invertir colador y cambiar sprite mientras se mantiene click izquierdo
        if (Input.GetMouseButton(0))
        {
            if (!invertido)
            {
                invertido = true;
                if (spriteRenderer != null && invertidoSprite != null)
                    spriteRenderer.sprite = invertidoSprite;
            }
        }
        else
        {
            if (invertido)
            {
                invertido = false;
                if (spriteRenderer != null && normalSprite != null)
                    spriteRenderer.sprite = normalSprite;
            }
        }
    }

    private void ActualizarTexto()
    {
        if (textoPuntaje != null)
            textoPuntaje.text = "Puntaje: " + puntaje;
    }

    // Método para recolectar o expulsar oveja
    public bool AgregarOveja(Transform oveja)
    {
        // Si el colador está invertido, expulsar oveja hacia arriba
        if (invertido)
        {
            Rigidbody2D rbOveja = oveja.GetComponent<Rigidbody2D>();
            if (rbOveja != null)
            {
                rbOveja.simulated = false; // desactivamos física
            }

            // Movimiento manual hacia arriba y destrucción
            StartCoroutine(ExpulsarOveja(oveja, 2f));
            return false;
        }

        // Recolección normal de ovejas en slots
        int slotIndex = -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (ocupados[i] == null)
            {
                slotIndex = i;
                break;
            }
        }

        if (slotIndex == -1) return false;

        oveja.SetParent(slots[slotIndex]);
        oveja.localPosition = Vector3.zero;
        ocupados[slotIndex] = oveja;

        Rigidbody2D rbOveja2 = oveja.GetComponent<Rigidbody2D>();
        if (rbOveja2 != null) rbOveja2.simulated = false;

        Oveja scriptOveja = oveja.GetComponent<Oveja>();
        if (scriptOveja != null) scriptOveja.enabled = false;

        StartCoroutine(RecompensarOveja(oveja, slotIndex, 3f));

        return true;
    }

    // Corutina para recompensar oveja recolectada
    private IEnumerator RecompensarOveja(Transform oveja, int slotIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (oveja != null)
        {
            puntaje += 1;
            ActualizarTexto();
            Destroy(oveja.gameObject);
        }

        ocupados[slotIndex] = null;
    }

    // Corutina para expulsar oveja hacia arriba sin colisiones
    private IEnumerator ExpulsarOveja(Transform oveja, float duracion)
    {
        float timer = 0f;
        while (timer < duracion && oveja != null)
        {
            oveja.Translate(Vector2.up * 8f * Time.deltaTime); // velocidad hacia arriba
            timer += Time.deltaTime;
            yield return null;
        }

        if (oveja != null)
            Destroy(oveja.gameObject);
    }


    // Reiniciar colador (opcional)
    public void ResetPlayer()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        invertido = false;
        if (spriteRenderer != null && normalSprite != null)
            spriteRenderer.sprite = normalSprite;

        for (int i = 0; i < ocupados.Length; i++)
        {
            if (ocupados[i] != null)
                Destroy(ocupados[i].gameObject);
            ocupados[i] = null;
        }

        puntaje = 0;
        ActualizarTexto();
    }

    public bool Invertido { get { return invertido; } }

    public void ModificarPuntaje(int cantidad)
    {
        puntaje += cantidad;
        if (puntaje < 0) puntaje = 0;
        ActualizarTexto();
    }

}
