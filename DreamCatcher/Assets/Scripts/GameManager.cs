using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Fases")]
    public int[] puntosMetaPorFase = { 5, 8, 10 };
    public float[] tiempoPorFase = { 30f, 25f, 20f };

    [Header("Valores de Puntaje")]
    public int puntosPorOvejaBuena = 1;
    public int penalizacionOvejaMala = 1;
    public int bonoPorColarNube = 1;

    [Header("Bonos de tiempo")]
    public float tiempoExtraPorNube = 5f; // segundos que se suman al tamizar una nube

    [Header("UI")]
    public SpriteRenderer barraPuntaje;
    public SpriteRenderer barraTiempo;

    [Header("Panel de Fase")]
    public CanvasGroup panelFase;
    public TextMeshProUGUI textoFase;
    public float fadeDuration = 1f;
    public float tiempoMostrarPanel = 2f;

    [Header("Fade de Escena")]
    public CanvasGroup fadePanel;
    public float fadeEscenaDuracion = 1f;

    private Vector3 escalaInicialPuntaje;
    private Vector3 escalaInicialTiempo;

    public int puntajeActual = 0;
    public float tiempoRestante;
    public int faseActual = 0;
    public bool NivelTerminado { get; private set; }

    private bool inicioDesdeInstrucciones = false;
    private bool faseIniciada = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (barraPuntaje) escalaInicialPuntaje = barraPuntaje.transform.localScale;
        if (barraTiempo) escalaInicialTiempo = barraTiempo.transform.localScale;

        NivelTerminado = false;
        faseActual = 0;

        // Solo iniciar automáticamente si no hay instrucciones
        if (!inicioDesdeInstrucciones)
        {
            faseIniciada = true;
            ReiniciarFase(0);
        }
    }

    void Update()
    {
        if (NivelTerminado || !faseIniciada) return;

        // Tiempo por fase
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinFase(false); // pierde si no alcanza puntos en la fase
        }

        ActualizarUI();
    }

    // ---------------- Fases ----------------
    private void ReiniciarFase(int fase)
    {
        faseActual = fase;
        puntajeActual = 0;
        tiempoRestante = tiempoPorFase[faseActual];
        NivelTerminado = false;
        faseIniciada = true;
    }

    public void ModificarPuntaje(int delta)
    {
        if (NivelTerminado || !faseIniciada) return;

        puntajeActual += delta;
        if (puntajeActual < 0) puntajeActual = 0;

        if (puntajeActual >= puntosMetaPorFase[faseActual])
        {
            int siguienteFase = faseActual + 1;
            if (siguienteFase >= puntosMetaPorFase.Length)
            {
                FinFase(true); // victoria al completar última fase
            }
            else
            {
                StartCoroutine(TransicionarAFase(siguienteFase));
            }
        }

        ActualizarUI();
    }

    public void OvejaBuena() => ModificarPuntaje(puntosPorOvejaBuena);
    public void OvejaMala() => ModificarPuntaje(-penalizacionOvejaMala);
    public void NubeColada()
    {
        if (NivelTerminado || !faseIniciada) return;

        tiempoRestante += tiempoExtraPorNube;

        // No exceder el tiempo máximo de la fase
        if (tiempoRestante > tiempoPorFase[faseActual])
            tiempoRestante = tiempoPorFase[faseActual];

        ActualizarUI();
        Debug.Log($"Nube colada: +{tiempoExtraPorNube} seg, tiempo restante {tiempoRestante}");
    }
    private void FinFase(bool victoria)
    {
        NivelTerminado = true;
        faseIniciada = false;

        if (victoria)
            StartCoroutine(CambiarEscenaConFade("VictoryScene"));
        else
            StartCoroutine(CambiarEscenaConFade("DefeatScene"));

        Debug.Log(victoria ? "¡Juego completado!" : $"Derrota en Fase {faseActual + 1}");
    }

    // ---------------- UI ----------------
    private void ActualizarUI()
    {
        if (barraPuntaje)
        {
            float fill = (float)puntajeActual / puntosMetaPorFase[faseActual];
            barraPuntaje.transform.localScale = new Vector3(
                escalaInicialPuntaje.x * Mathf.Clamp01(fill),
                escalaInicialPuntaje.y,
                escalaInicialPuntaje.z
            );
        }

        if (barraTiempo)
        {
            float fill = tiempoRestante / tiempoPorFase[faseActual];
            barraTiempo.transform.localScale = new Vector3(
                escalaInicialTiempo.x * Mathf.Clamp01(fill),
                escalaInicialTiempo.y,
                escalaInicialTiempo.z
            );
        }
    }

    // ---------------- Panel de Fase ----------------
    private IEnumerator MostrarPanelFase(int fase)
    {
        panelFase.gameObject.SetActive(true);
        textoFase.text = $"Fase {fase + 1}";

        float timer = 0f;
        while (timer < fadeDuration)
        {
            panelFase.alpha = timer / fadeDuration;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        panelFase.alpha = 1f;

        yield return new WaitForSecondsRealtime(tiempoMostrarPanel);

        timer = 0f;
        while (timer < fadeDuration)
        {
            panelFase.alpha = 1f - (timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        panelFase.alpha = 0f;
        panelFase.gameObject.SetActive(false);
    }

    private IEnumerator TransicionarAFase(int nuevaFase)
    {
        faseIniciada = false;
        NivelTerminado = true;

        yield return MostrarPanelFase(nuevaFase);

        ReiniciarFase(nuevaFase);
    }

    // ---------------- Fade de escena ----------------
    private IEnumerator CambiarEscenaConFade(string nombreEscena)
    {
        if (fadePanel == null) yield break;

        fadePanel.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeEscenaDuracion)
        {
            fadePanel.alpha = Mathf.SmoothStep(0f, 1f, timer / fadeEscenaDuracion);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        fadePanel.alpha = 1f;

        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(nombreEscena);
    }

    // ---------------- Inicio desde instrucciones ----------------
    public void ComenzarJuegoDesdeInstrucciones()
    {
        inicioDesdeInstrucciones = true;
        StartCoroutine(TransicionarAFase(0));
    }
}
