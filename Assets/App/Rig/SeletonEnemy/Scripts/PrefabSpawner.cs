using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Assign the prefab in the Inspector
    public Transform spawnPoint;     // Assign a spawn position (or leave empty to use this object's position)
    public float spawnInterval = 2f; // Time between spawns
    public int totalSpawns = 5;      // Total number of spawns

    private int currentSpawnCount = 0; // Tracks how many have been spawned

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (currentSpawnCount < totalSpawns)
        {
            SpawnPrefab();
            currentSpawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPrefab()
    {
        if (prefabToSpawn != null)
        {
            Vector3 spawnPosition = spawnPoint ? spawnPoint.position : transform.position;
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab not assigned!");
        }
    }
}
