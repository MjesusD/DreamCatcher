using UnityEngine;

public class OvejaSpawner : MonoBehaviour
{
    public GameObject ovejaBuenaPrefab;
    public GameObject ovejaEnemigaPrefab;
    public float spawnInterval = 2f;
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;
    [Range(0, 1)]
    public float probabilidadEnemiga = 0.5f; // % enemigos

    private float timer;

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
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
