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
    private Coroutine spawnerCoroutine;

    void OnEnable()
    {
        ParallaxGroup.OnCambioEtapa += CambiarFase;
    }

    void OnDisable()
    {
        ParallaxGroup.OnCambioEtapa -= CambiarFase;
    }

    void Start()
    {
        if (nubePrefab == null) { Debug.LogError("Asigna nubePrefab"); return; }
        spawnerCoroutine = StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(chequeoCadaSegundos);

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

    public void NubeEliminada(Nube nube)
    {
        nubes.Remove(nube);
    }

    // Ajustar dificultad según la fase
    private void CambiarFase(int fase)
    {
        // Ajuste de parámetros
        switch (fase)
        {
            case 0:
                fallSpeed = 2f; chequeoCadaSegundos = 2f; probabilidadSpawn = 0.4f; break;
            case 1:
                fallSpeed = 3f; chequeoCadaSegundos = 1.5f; probabilidadSpawn = 0.6f; break;
            case 2:
                fallSpeed = 4f; chequeoCadaSegundos = 1f; probabilidadSpawn = 0.8f; break;
        }

        // Reiniciar coroutine para aplicar chequeoCadaSegundos inmediatamente
        if (spawnerCoroutine != null) StopCoroutine(spawnerCoroutine);
        spawnerCoroutine = StartCoroutine(Spawner());

        Debug.Log($"Cambio a fase {fase} -> Nubes ajustadas (vel={fallSpeed}, intervalo={chequeoCadaSegundos}, prob={probabilidadSpawn})");
    }

}
