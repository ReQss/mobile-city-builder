using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RoomEnemiesGenerator : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; 
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

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
            Vector3 randomPos = spawnCenter + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );
            GameObject objectSpawned = Instantiate(prefab, randomPos, Quaternion.identity, parentFolder); 
            objectSpawned.SetActive(true);
        }
        await Task.CompletedTask;
    }
    
 

}
