using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Fases")]
    public int[] puntosMetaPorFase = { 5, 8, 10 };
    public float[] tiempoPorFase = { 30f, 25f, 20f };
    public ParallaxGroup parallaxGroup;

    [Header("Valores de Puntaje")]
    public int puntosPorOvejaBuena = 1;
    public int penalizacionOvejaMala = 1;
    public int bonoPorColarNube = 1;

    [Header("UI")]
    public SpriteRenderer barraPuntaje;
    public SpriteRenderer barraTiempo;

    private Vector3 escalaInicialPuntaje;
    private Vector3 escalaInicialTiempo;

    public int puntajeActual = 0;
    public float tiempoRestante;
    public int faseActual = 0;
    public bool NivelTerminado { get; private set; }

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

        // Suscribirse al cambio de fase
        if (parallaxGroup != null)
            ParallaxGroup.OnCambioEtapa += ReiniciarFase;

        ReiniciarFase(faseActual);
    }

    void Update()
    {
        if (NivelTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinNivel(false);
        }

        ActualizarUI();
    }

    private void ReiniciarFase(int fase)
    {
        faseActual = fase;
        puntajeActual = 0;
        tiempoRestante = tiempoPorFase[faseActual];
        NivelTerminado = false;
    }

    public void ModificarPuntaje(int delta)
    {
        if (NivelTerminado) return;

        puntajeActual += delta;
        if (puntajeActual < 0) puntajeActual = 0;

        if (puntajeActual >= puntosMetaPorFase[faseActual])
        {
            // Siguiente fase manual si se desea
            int siguienteFase = faseActual + 1;
            if (siguienteFase >= puntosMetaPorFase.Length)
                FinNivel(true);
            else
                ReiniciarFase(siguienteFase);
        }

        ActualizarUI();
    }

    public void OvejaBuena() => ModificarPuntaje(puntosPorOvejaBuena);
    public void OvejaMala() => ModificarPuntaje(-penalizacionOvejaMala);
    public void NubeColada() => ModificarPuntaje(bonoPorColarNube);

    private void FinNivel(bool victoria)
    {
        NivelTerminado = true;
        if (victoria)
            SceneManager.LoadScene("VictoryScene");
        else
            SceneManager.LoadScene("DefeatScene");

        Debug.Log(victoria ? "¡Juego completado!" : "Tiempo agotado. Derrota.");
    }

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
}
