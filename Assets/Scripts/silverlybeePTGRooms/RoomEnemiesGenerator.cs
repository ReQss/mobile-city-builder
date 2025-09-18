using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AI;

[System.Serializable]
public class SpawnableObject
{
    public string name;
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

        float totalChance = 0f;
        foreach (var obj in objectsToSpawn)
            totalChance += obj.spawnChance;

        while (spawned < count + playerLevelExtraEnemies && tries < maxTries)
        {
            tries++;
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
            // randomPos.y = Mathf.Max(1.0f, Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(randomPos) : randomPos.y);

            GameObject objectSpawned = null;
            switch (candidate.name)
            {
                case "BossKnight":
                    objectSpawned = EnemyPool.Instance.GetBossKnightEnemy();
                    break;
                case "BossWidow":
                    objectSpawned = EnemyPool.Instance.GetBossWidowEnemy();
                    break;
                case "Archer":
                    objectSpawned = EnemyPool.Instance.GetArcherEnemy();
                    break;
                case "Thug":
                    objectSpawned = EnemyPool.Instance.GetThugEnemy();
                    break;
                case "BlackWidow":
                    objectSpawned = EnemyPool.Instance.GetBlackWidowEnemy();
                    break;
                case "RedWidow":
                    objectSpawned = EnemyPool.Instance.GetRedWidowEnemy();
                    break;
                case "GrayWidow":
                    objectSpawned = EnemyPool.Instance.GetGrayWidowEnemy();
                    break;
                default:
                    objectSpawned = Instantiate(candidate.prefab, randomPos, Quaternion.identity, parentFolder);
                    break;
            }

            if (objectSpawned == null)
                continue;

            objectSpawned.transform.position = randomPos;
            objectSpawned.transform.rotation = Quaternion.identity;
            objectSpawned.transform.SetParent(parentFolder);
            objectSpawned.SetActive(true);
            objectSpawned.GetComponent<NavMeshAgent>().enabled = true;
            spawned++;
        }

        if (spawned == 0 && objectsToSpawn.Count > 0)
        {
            Vector3 randomPos = spawnCenter + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );
            GameObject objectSpawned = null;
            var candidate = objectsToSpawn[0];
            switch (candidate.name)
            {
                case "BossKnight":
                    objectSpawned = EnemyPool.Instance.GetBossKnightEnemy();
                    break;
                case "BossWidow":
                    objectSpawned = EnemyPool.Instance.GetBossWidowEnemy();
                    break;
                case "Archer":
                    objectSpawned = EnemyPool.Instance.GetArcherEnemy();
                    break;
                case "Thug":
                    objectSpawned = EnemyPool.Instance.GetThugEnemy();
                    break;
                case "BlackWidow":
                    objectSpawned = EnemyPool.Instance.GetBlackWidowEnemy();
                    break;
                case "RedWidow":
                    objectSpawned = EnemyPool.Instance.GetRedWidowEnemy();
                    break;
                case "GrayWidow":
                    objectSpawned = EnemyPool.Instance.GetGrayWidowEnemy();
                    break;
                default:
                    objectSpawned = Instantiate(candidate.prefab, randomPos, Quaternion.identity, parentFolder);
                    break;
            }
            if (objectSpawned != null)
            {
                objectSpawned.transform.position = randomPos;
                objectSpawned.transform.rotation = Quaternion.identity;
                objectSpawned.transform.SetParent(parentFolder);
                objectSpawned.SetActive(true);
                var agent = objectSpawned.GetComponent<NavMeshAgent>();
                if (agent != null) agent.enabled = true;
                spawned = 1;
            }
        }

        await Task.CompletedTask;
    }
}
