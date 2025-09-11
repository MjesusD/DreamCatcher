using UnityEngine;

public class Parallax_Manager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;       // Objeto de fondo
        [Range(0f, 10f)]
        public float speed = 1f;      // Velocidad de movimiento
        [HideInInspector] public float width; // Ancho del sprite, se calcula en Start
    }

    public ParallaxLayer[] layers;

    void Start()
    {
        // Calcula el ancho de cada capa según su SpriteRenderer
        foreach (ParallaxLayer layer in layers)
        {
            if (layer.layer != null)
            {
                SpriteRenderer sr = layer.layer.GetComponent<SpriteRenderer>();
                if (sr != null)
                    layer.width = sr.bounds.size.x;
            }
        }
    }

    void Update()
    {
        foreach (ParallaxLayer layer in layers)
        {
            if (layer.layer == null) continue;

            // Mueve la capa en X
            layer.layer.position += new Vector3(layer.speed * Time.deltaTime, 0, 0);

            // Si salió completamente de la cámara a la izquierda, reposición a la derecha
            if (layer.layer.position.x > layer.width)
            {
                layer.layer.position -= new Vector3(layer.width * 2f, 0, 0);
            }
            else if (layer.layer.position.x < -layer.width)
            {
                layer.layer.position += new Vector3(layer.width * 2f, 0, 0);
            }
        }
    }
}
