using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject ovejaBuenaPrefab;
    public GameObject ovejaEnemigaPrefab;

    [Header("Spawn Config")]
    public float spawnInterval = 2f;
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;
    [Range(0, 1)] public float probabilidadEnemiga = 0.5f;

    private float timer;
    private int faseActual = 0;

    void OnEnable() => ParallaxGroup.OnCambioEtapa += CambiarFaseVisual;
    void OnDisable() => ParallaxGroup.OnCambioEtapa -= CambiarFaseVisual;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnOveja();
            timer = 0f;
        }
    }

    void SpawnOveja()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector2 spawnPos = new Vector2(randomX, spawnHeight);

        GameObject prefab = (Random.value < probabilidadEnemiga) ? ovejaEnemigaPrefab : ovejaBuenaPrefab;
        GameObject oveja = Instantiate(prefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = oveja.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float velocidadExtra = faseActual * 1.5f;
            rb.linearVelocity = Vector2.down * (2f + velocidadExtra);
        }
    }

    void CambiarFaseVisual(int nuevaFase)
    {
        faseActual = nuevaFase;

        switch (faseActual)
        {
            case 0: spawnInterval = 2f; probabilidadEnemiga = 0.5f; break;
            case 1: spawnInterval = 1.5f; probabilidadEnemiga = 0.65f; break;
            case 2: spawnInterval = 1f; probabilidadEnemiga = 0.8f; break;
        }
    }
}
