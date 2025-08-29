using UnityEngine;
using System.Collections;
using TMPro;

public class Colador : MonoBehaviour
{
    public Rigidbody2D rb; 
    public float moveSpeed = 10f;
    public int puntaje = 0;

    public TMP_Text textoPuntaje;

    private float inputValue;
    public Transform[] slots;
    private Transform[] ocupados;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite invertidoSprite;
    private SpriteRenderer spriteRenderer;

    private bool invertido = false;

    void Start()
    {
        ocupados = new Transform[slots.Length];
        ActualizarTexto();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && normalSprite != null)
            spriteRenderer.sprite = normalSprite;
    }

    void Update()
    {
        // Movimiento horizontal
        inputValue = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputValue * moveSpeed, 0f);

        // Invertir colador con click izquierdo
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

    // Método para agregar oveja al colador
    public bool AgregarOveja(Transform oveja)
    {
        // Si está invertido, expulsa en lugar de recolectar
        if (invertido)
        {
            Rigidbody2D rbOveja = oveja.GetComponent<Rigidbody2D>();
            if (rbOveja != null)
            {
                rbOveja.simulated = true;
                rbOveja.linearVelocity = new Vector2(0f, 8f); // expulsión hacia arriba
            }

            // Destruir la oveja después de 2 segundos
            Destroy(oveja.gameObject, 2f);

            return false; // no se guarda en el colador
        }

        // Código normal de guardado
        
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

        // Colocar oveja en slot
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


    private IEnumerator RecompensarOveja(Transform oveja, int slotIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Comprobar si la oveja aún existe
        if (oveja != null)
        {
            puntaje += 1;
            ActualizarTexto();

            ocupados[slotIndex] = null;
            Destroy(oveja.gameObject);
        }
        else
        {
            // Si ya fue expulsada, liberamos el slot
            ocupados[slotIndex] = null;
        }
    }

}
