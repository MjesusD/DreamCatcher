using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Meta del Nivel")]
    public int puntosMeta = 10;
    public float tiempoNivel = 60f;

    [Header("Valores de Puntaje")]
    public int puntosPorOvejaBuena = 1;
    public int penalizacionOvejaMala = 1;
    public int bonoPorColarNube = 1; // opcional

    [Header("UI")]
    public TMP_Text textoPuntaje; // "Puntaje: X / Meta"
    public TMP_Text textoTiempo;  // "Tiempo: mm:ss"
    public GameObject pantallaVictoria;
    public GameObject pantallaDerrota;

    [Header("Estado")]
    public int puntajeActual = 0;
    public float tiempoRestante;
    public bool NivelTerminado { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        NivelTerminado = false;
        tiempoRestante = tiempoNivel;
        ActualizarUI();
    }

    void Update()
    {
        if (NivelTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            VerificarCondicionFinal();
        }
        ActualizarUI();
    }

    // === API de Puntaje ===
    public void ModificarPuntaje(int delta)
    {
        if (NivelTerminado) return;

        puntajeActual += delta;
        if (puntajeActual < 0) puntajeActual = 0;

        VerificarCondicionFinal();
        ActualizarUI();
    }

    public void OvejaBuena() => ModificarPuntaje(puntosPorOvejaBuena);
    public void OvejaMala() => ModificarPuntaje(-penalizacionOvejaMala);
    public void NubeColada() => ModificarPuntaje(bonoPorColarNube);

    // Fin de nivel
    private void VerificarCondicionFinal()
    {
        if (puntajeActual >= puntosMeta)
        {
            FinNivel(true);
        }
        else if (tiempoRestante <= 0f)
        {
            FinNivel(false);
        }
    }

    private void FinNivel(bool victoria)
    {
        NivelTerminado = true;
        if (victoria && pantallaVictoria) pantallaVictoria.SetActive(true);
        if (!victoria && pantallaDerrota) pantallaDerrota.SetActive(true);

        Debug.Log(victoria ? "¡Nivel superado!" : "Tiempo agotado. Derrota.");
    }

    private void ActualizarUI()
    {
        if (textoPuntaje)
            textoPuntaje.text = $"Puntaje: {puntajeActual} / {puntosMeta}";

        if (textoTiempo)
        {
            int t = Mathf.CeilToInt(tiempoRestante);
            int mm = t / 60;
            int ss = t % 60;
            textoTiempo.text = $"Tiempo: {mm:00}:{ss:00}";
        }
    }
}
