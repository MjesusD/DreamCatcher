using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParallaxGroup : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;      // Capa (cielo, nubes, montañas)
        public float speed = 0.5f;   // Velocidad de movimiento
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
    public float changeInterval = 10f;
    public float fadeDuration = 2f;

    private int currentIndex = 0;

    void Start()
    {
        // Inicializar etapas y duplicar capas para loop
        foreach (var etapa in etapas)
        {
            etapa.root.SetActive(false);
            foreach (var layer in etapa.layers)
            {
                SpriteRenderer sr = layer.layer.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    layer.width = sr.bounds.size.x;

                    // Duplicar sprite a la derecha
                    Transform clone = Instantiate(layer.layer, layer.layer.position + Vector3.right * layer.width, Quaternion.identity, layer.layer.parent);
                    layer.clones.Add(clone);
                }
            }
        }

        etapas[0].root.SetActive(true);
        SetAlphaEtapa(etapas[0], 1f);

        StartCoroutine(MoverCapas());
        StartCoroutine(CambiarEtapas());
    }

    IEnumerator MoverCapas()
    {
        while (true)
        {
            foreach (var layer in etapas[currentIndex].layers)
            {
                if (layer.layer == null) continue;

                // Mover principal
                layer.layer.Translate(Vector2.left * layer.speed * Time.deltaTime, Space.World);
                // Mover clones
                foreach (var c in layer.clones)
                    c.Translate(Vector2.left * layer.speed * Time.deltaTime, Space.World);

                // Loop infinito
                if (layer.layer.position.x <= -layer.width)
                {
                    layer.layer.position += Vector3.right * layer.width * (layer.clones.Count + 1);
                }
                foreach (var c in layer.clones)
                {
                    if (c.position.x <= -layer.width)
                        c.position += Vector3.right * layer.width * (layer.clones.Count + 1);
                }
            }
            yield return null;
        }
    }

    IEnumerator CambiarEtapas()
    {
        while (currentIndex < etapas.Length - 1)
        {
            yield return new WaitForSeconds(changeInterval);

            int nextIndex = currentIndex + 1;
            etapas[nextIndex].root.SetActive(true);

            float timer = 0f;
            while (timer < fadeDuration)
            {
                float alphaCurrent = 1f - (timer / fadeDuration);
                float alphaNext = timer / fadeDuration;

                SetAlphaEtapa(etapas[currentIndex], alphaCurrent);
                SetAlphaEtapa(etapas[nextIndex], alphaNext);

                timer += Time.deltaTime;
                yield return null;
            }

            SetAlphaEtapa(etapas[currentIndex], 0f);
            SetAlphaEtapa(etapas[nextIndex], 1f);

            etapas[currentIndex].root.SetActive(false);
            currentIndex = nextIndex;
        }
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
