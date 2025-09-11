using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CambioEtapaHandler(int nuevaEtapa);

public class ParallaxGroup : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;
        public float speed = 0.5f;
        [HideInInspector] public List<Transform> clones = new List<Transform>();
        [HideInInspector] public float width;
    }

    [System.Serializable]
    public class Etapa
    {
        public GameObject root;       // Mañana, Tarde, Noche
        public ParallaxLayer[] layers;
    }

    [Header("Configuración de etapas")]
    public Etapa[] etapas;
    public float changeInterval = 10f;   // Tiempo hasta siguiente fase
    public float fadeDuration = 2f;

    private int currentIndex = 0;

    public static event CambioEtapaHandler OnCambioEtapa;

    void Start()
    {
        // Inicializar capas y clones
        foreach (var etapa in etapas)
        {
            etapa.root.SetActive(false);
            foreach (var layer in etapa.layers)
            {
                SpriteRenderer sr = layer.layer.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    layer.width = sr.bounds.size.x;
                    Transform clone = Instantiate(layer.layer, layer.layer.position + Vector3.right * layer.width, Quaternion.identity, layer.layer.parent);
                    layer.clones.Add(clone);
                }
            }
        }

        etapas[0].root.SetActive(true);
        SetAlphaEtapa(etapas[0], 1f);

        StartCoroutine(MoverCapas());
        StartCoroutine(CambiarEtapasAutomatico());
    }

    IEnumerator MoverCapas()
    {
        while (true)
        {
            foreach (var layer in etapas[currentIndex].layers)
            {
                if (layer.layer == null) continue;

                layer.layer.Translate(Vector2.left * layer.speed * Time.deltaTime, Space.World);
                foreach (var c in layer.clones)
                    c.Translate(Vector2.left * layer.speed * Time.deltaTime, Space.World);

                if (layer.layer.position.x <= -layer.width)
                    layer.layer.position += Vector3.right * layer.width * (layer.clones.Count + 1);

                foreach (var c in layer.clones)
                    if (c.position.x <= -layer.width)
                        c.position += Vector3.right * layer.width * (layer.clones.Count + 1);
            }
            yield return null;
        }
    }

    // Cambiar fase automáticamente según tiempo
    IEnumerator CambiarEtapasAutomatico()
    {
        while (currentIndex < etapas.Length - 1)
        {
            yield return new WaitForSeconds(changeInterval);

            int nextIndex = currentIndex + 1;
            StartCoroutine(FadeEtapas(currentIndex, nextIndex));
            currentIndex = nextIndex;
        }
    }

    public void CambiarEtapaManual(int nuevaEtapa)
    {
        if (nuevaEtapa < 0 || nuevaEtapa >= etapas.Length) return;
        StartCoroutine(FadeEtapas(currentIndex, nuevaEtapa));
        currentIndex = nuevaEtapa;
    }

    private IEnumerator FadeEtapas(int from, int to)
    {
        etapas[to].root.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alphaFrom = 1f - (timer / fadeDuration);
            float alphaTo = timer / fadeDuration;

            SetAlphaEtapa(etapas[from], alphaFrom);
            SetAlphaEtapa(etapas[to], alphaTo);

            timer += Time.deltaTime;
            yield return null;
        }

        SetAlphaEtapa(etapas[from], 0f);
        SetAlphaEtapa(etapas[to], 1f);
        etapas[from].root.SetActive(false);

        OnCambioEtapa?.Invoke(to);
    }

    void SetAlphaEtapa(Etapa etapa, float alpha)
    {
        foreach (var layer in etapa.layers)
        {
            SetAlpha(layer.layer, alpha);
            foreach (var c in layer.clones)
                SetAlpha(c, alpha);
        }
    }

    void SetAlpha(Transform t, float alpha)
    {
        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}
