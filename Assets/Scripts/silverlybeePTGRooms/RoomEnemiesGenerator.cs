using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float spawnChance = 1f; // 1 = 100%, 0.5 = 50%
}

public class RoomEnemiesGenerator : MonoBehaviour
{
    public List<SpawnableObject> objectsToSpawn; 
    public int spawnCount = 10;
    public float spawnRange = 20f;
    public Vector3 center = Vector3.zero;
    public Transform centerTransform;
    public Transform parentFolder;
    private bool isSpawningEnemies = false;

    public async Task SpawnObjectsNumber(int count)
    {
        if (isSpawningEnemies)
            return;
        isSpawningEnemies = true;
        Vector3 spawnCenter = centerTransform != null ? centerTransform.position : center;

        if (objectsToSpawn == null || objectsToSpawn.Count == 0)
            return;
        int playerLevelExtraEnemies = GameManager.Instance.playerLevel / 4;
        int spawned = 0;
        int tries = 0;
        int maxTries = (count + playerLevelExtraEnemies) * 10;

        // Przygotuj sumę szans
        float totalChance = 0f;
        foreach (var obj in objectsToSpawn)
            totalChance += obj.spawnChance;

        while (spawned < count + playerLevelExtraEnemies && tries < maxTries)
        {
            tries++;
            // Losuj typ przeciwnika z wagą
            float rand = Random.value * totalChance;
            float cumulative = 0f;
            SpawnableObject candidate = null;
            foreach (var obj in objectsToSpawn)
            {
                cumulative += obj.spawnChance;
                if (rand <= cumulative)
                {
                    candidate = obj;
                    break;
                }
            }
            if (candidate == null)
                continue;

            Vector3 randomPos = spawnCenter + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );
            GameObject objectSpawned = Instantiate(candidate.prefab, randomPos, Quaternion.identity, parentFolder); 
            objectSpawned.SetActive(true);
            spawned++;
        }

        // Jeśli nie udało się stworzyć żadnego przeciwnika, stwórz pierwszego z listy
        if (spawned == 0 && objectsToSpawn.Count > 0)
        {
            Vector3 randomPos = spawnCenter + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );
            GameObject objectSpawned = Instantiate(objectsToSpawn[0].prefab, randomPos, Quaternion.identity, parentFolder); 
            objectSpawned.SetActive(true);
            spawned = 1;
        }

        await Task.CompletedTask;
    }
}
