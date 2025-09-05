using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nube_Manager : MonoBehaviour
{
    public GameObject nubePrefab;
    public Vector2 rangoX = new Vector2(-7f, 7f);
    public Vector2 rangoY = new Vector2(2f, 4f);
    [Range(0f, 1f)] public float probabilidadSpawn = 0.5f;
    public float fallSpeed = 2f;
    public float chequeoCadaSegundos = 2f;

    private readonly List<Nube> nubes = new List<Nube>();

    void Start()
    {
        if (nubePrefab == null) { Debug.LogError("Asigna nubePrefab"); return; }
        StartCoroutine(Spawner());
    }

    // Spawner ocasional
    private IEnumerator Spawner()
    {
        var wait = new WaitForSeconds(chequeoCadaSegundos);
        while (true)
        {
            yield return wait;

            if (Random.value < probabilidadSpawn)
                CrearNube(PosAleatoria());
        }
    }

    private Vector3 PosAleatoria()
    {
        return new Vector3(Random.Range(rangoX.x, rangoX.y), Random.Range(rangoY.x, rangoY.y), 0f);
    }

    private void CrearNube(Vector3 pos)
    {
        var go = Instantiate(nubePrefab, pos, Quaternion.identity);
        var nube = go.GetComponent<Nube>() ?? go.AddComponent<Nube>();
        nube.manager = this;

        var rb = go.GetComponent<Rigidbody2D>() ?? go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.simulated = true;
        rb.freezeRotation = true;

        var col = go.GetComponent<Collider2D>() ?? go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        nubes.Add(nube);

        // Activar caída al instante
        nube.ActivarCaida(fallSpeed);
    }

    // Reemplazar nube destruida (respawn inmediato)
    public void NubeEliminada(Nube nube)
    {
        nubes.Remove(nube);
        // no se crea otra automáticamente, el spawner ocasional generará nuevas
    }
}
