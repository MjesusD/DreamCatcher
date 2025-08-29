using UnityEngine;

public class OvejaSpawner : MonoBehaviour
{
    public GameObject ovejaPrefab;   // Prefab de la oveja
    public float spawnInterval = 4f; // tiempo entre spawns
    public float spawnRangeX = 8f;   // rango en el eje X donde aparecen
    public float spawnHeight = 6f;   // altura en la que aparecen (arriba de la pantalla)

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
        // posición aleatoria en X dentro del rango
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);

        // posición de aparición
        Vector2 spawnPos = new Vector2(randomX, spawnHeight);

        // crear oveja
        Instantiate(ovejaPrefab, spawnPos, Quaternion.identity);
    }
}
