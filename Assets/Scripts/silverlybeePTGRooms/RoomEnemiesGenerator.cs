using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System;
public enum EnemyType
{
    Prefab,
    BossKnight,
    BossWidow,
    Archer,
    Thug,
    BlackWidow,
    RedWidow,
    GrayWidow
}

[System.Serializable]
public class SpawnableObject
{
    public EnemyType type;
    public GameObject prefab;
    [Range(0f, 1f)] public float spawnChance = 1f;
}

public class RoomEnemiesGenerator : MonoBehaviour
{
    public List<SpawnableObject> objectsToSpawn;
    public float spawnRange = 20f;
    public Vector3 center = Vector3.zero;
    public Transform centerTransform;
    public Transform parentFolder;
    public int numberOfEnemies = 1;
    private bool isSpawning;

    public void SpawnObjectsNumber()
    {
        if (isSpawning || objectsToSpawn.Count == 0) return;
        isSpawning = true;
        int count = numberOfEnemies;
        //take player level too into account, every 2 levels add 1 extra enemy
        int level = GameManager.Instance.playerLevel;
        count += (int)MathF.Ceiling((level - 1) / 2f);
        Vector3 spawnCenter = centerTransform ? centerTransform.position : center;


        for (int i = 0; i < count; i++)
        {
            var candidate = GetRandomObject();
            Vector3 pos = GetRandomPosition(spawnCenter);
            Spawn(candidate, pos);
        }

        isSpawning = false;
    }

    SpawnableObject GetRandomObject()
    {
        float total = 0;
        foreach (var o in objectsToSpawn) total += o.spawnChance;

        float rand = UnityEngine.Random.value * total;
        float sum = 0;

        foreach (var o in objectsToSpawn)
        {
            sum += o.spawnChance;
            if (rand <= sum) return o;
        }

        return objectsToSpawn[0];
    }

    Vector3 GetRandomPosition(Vector3 center)
    {
        return center + new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),
            0,
            UnityEngine.Random.Range(-spawnRange, spawnRange)
        );
    }

    void Spawn(SpawnableObject obj, Vector3 pos)
    {
        GameObject go = GetFromPool(obj);

        if (go == null && obj.prefab != null)
            go = Instantiate(obj.prefab);

        if (go == null) return;

        go.transform.SetParent(parentFolder);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        var agent = go.GetComponent<NavMeshAgent>();
        if (agent) agent.enabled = true;
    }
    private GameObject GetFromPool(SpawnableObject obj)
{
    switch (obj.type)
    {
        // case EnemyType.BossKnight: return EnemyPool.Instance.GetBossKnightEnemy();
        // case EnemyType.BossWidow: return EnemyPool.Instance.GetBossWidowEnemy();
        // case EnemyType.Archer: return EnemyPool.Instance.GetArcherEnemy();
        // case EnemyType.Thug: return EnemyPool.Instance.GetThugEnemy();
        // case EnemyType.BlackWidow: return EnemyPool.Instance.GetBlackWidowEnemy();
        // case EnemyType.RedWidow: return EnemyPool.Instance.GetRedWidowEnemy();
        // case EnemyType.GrayWidow: return EnemyPool.Instance.GetGrayWidowEnemy();
        default:
            // Sprawdzamy listę enemyInstances po prefabie
            if (obj.prefab != null)
            {
                EnemyInstance instance = EnemyPool.Instance.enemyInstances
                    .Find(x => x.enemyPrefab == obj.prefab);

                if (instance != null)
                {
                    return EnemyPool.Instance.GetEnemyFromPool(instance.enemyPrefab, instance.enemyPool);
                }
                else
                {
                    Debug.LogWarning("Prefab not found in enemyInstances: " + obj.prefab.name);
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("SpawnableObject prefab is null for type: " + obj.type);
                return null;
            }
    }
}
}               