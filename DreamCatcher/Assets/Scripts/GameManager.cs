using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Meta del Nivel")]
    public int puntosMeta = 10;
    public float tiempoNivel = 60f;

    [Header("Valores de Puntaje")]
    public int puntosPorOvejaBuena = 1;
    public int penalizacionOvejaMala = 1;
    public int bonoPorColarNube = 1;

    [Header("UI (SpriteRenderer)")]
    public SpriteRenderer barraPuntaje;  
    public SpriteRenderer barraTiempo;  

    private Vector3 escalaInicialPuntaje;
    private Vector3 escalaInicialTiempo;

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

        // Guardamos escalas originales de los rellenos
        if (barraPuntaje) escalaInicialPuntaje = barraPuntaje.transform.localScale;
        if (barraTiempo) escalaInicialTiempo = barraTiempo.transform.localScale;

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

    // Puntaje
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
        if (victoria)
            UnityEngine.SceneManagement.SceneManager.LoadScene("VictoryScene");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("DefeatScene");

        Debug.Log(victoria ? "¡Nivel superado!" : "Tiempo agotado. Derrota.");
    }

    // Actualización de barras 
    private void ActualizarUI()
    {
        if (barraPuntaje)
        {
            float fill = (float)puntajeActual / puntosMeta;
            barraPuntaje.transform.localScale = new Vector3(
                escalaInicialPuntaje.x * Mathf.Clamp01(fill),
                escalaInicialPuntaje.y,
                escalaInicialPuntaje.z
            );
        }

        if (barraTiempo)
        {
            float fill = tiempoRestante / tiempoNivel;
            barraTiempo.transform.localScale = new Vector3(
                escalaInicialTiempo.x * Mathf.Clamp01(fill),
                escalaInicialTiempo.y,
                escalaInicialTiempo.z
            );
        }
    }
}
