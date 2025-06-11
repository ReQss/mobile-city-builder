using UnityEngine;
using System.Collections.Generic;

public class ProceduralWeaponPlacement : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; // List of prefabs to spawn
    public int spawnCount = 10;
    public float spawnRange = 20f;
    public Vector3 center = Vector3.zero;
    public Transform centerTransform;
    public GameObject player;
    public Transform parentFolder; // Assign this in the inspector to act as the folder
    public float enableDistance = 15f; // Distance to enable weapon

    private List<GameObject> spawnedWeapons = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        foreach (var weapon in spawnedWeapons)
        {
            if (weapon == null) continue;
            float dist = Vector3.Distance(playerPos, weapon.transform.position);
            bool shouldEnable = dist <= enableDistance;
            if (weapon.activeSelf != shouldEnable)
                weapon.SetActive(shouldEnable);
        }
    }

    public void SpawnObjects()
    {
        Vector3 spawnCenter = centerTransform != null ? centerTransform.position : center;

        if (objectsToSpawn == null || objectsToSpawn.Count == 0)
            return;

        int maxIndex = Mathf.Min(GameManager.Instance.weaponLevel, objectsToSpawn.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = objectsToSpawn[Random.Range(0, maxIndex)];
            Vector3 randomPos = spawnCenter + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );
            GameObject weapon = Instantiate(prefab, randomPos, Quaternion.identity, parentFolder);
            weapon.SetActive(false); // Start inactive
            spawnedWeapons.Add(weapon);
        }
    }
    public void SpawnObjectsNumber(int count)
    {
        Vector3 spawnCenter = centerTransform != null ? centerTransform.position : center;

        if (objectsToSpawn == null || objectsToSpawn.Count == 0)
            return;

        int maxIndex = Mathf.Min(GameManager.Instance.weaponLevel, objectsToSpawn.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = objectsToSpawn[Random.Range(0, maxIndex)];
            Vector3 randomPos = spawnCenter + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );
            GameObject objectSpawned = Instantiate(prefab, randomPos, Quaternion.identity, parentFolder); // Set parent
            objectSpawned.SetActive(true);
        }
    }
    public void SpawnObjectsNumberNearbyPlayer(int count)
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is null. Cannot spawn objects nearby player.");
            return;
        }

        if (objectsToSpawn == null || objectsToSpawn.Count == 0)
            return;

        int maxIndex = Mathf.Min(GameManager.Instance.weaponLevel, objectsToSpawn.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = objectsToSpawn[Random.Range(0, maxIndex)];
            // Spawn within a radius around the player
            float radius = spawnRange * 0.5f;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(2f, radius); // min distance from player
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;
            Vector3 randomPos = player.transform.position + offset;

            GameObject objectSpawned = Instantiate(prefab, randomPos, Quaternion.identity, parentFolder); // Set parent
            objectSpawned.SetActive(true);
        }
    }
     public void SpawnEnemiesNumberNearbyPlayer(int count)
    {
         if (player == null)
    {
        Debug.LogWarning("Player reference is null. Cannot spawn objects nearby player.");
        return;
    }

    if (objectsToSpawn == null || objectsToSpawn.Count == 0)
        return;

    int maxIndex = Mathf.Min(objectsToSpawn.Count, objectsToSpawn.Count);

    for (int i = 0; i < count; i++)
    {
        GameObject prefab = objectsToSpawn[Random.Range(0, maxIndex)];
        // Spawn within a radius around the player
        float radius = spawnRange * 0.5f;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(2f, radius); // min distance from player
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;
        Vector3 randomPos = player.transform.position + offset;

        GameObject objectSpawned = Instantiate(prefab, randomPos, Quaternion.identity, parentFolder); // Set parent
        objectSpawned.SetActive(true);
    }
    }
}
