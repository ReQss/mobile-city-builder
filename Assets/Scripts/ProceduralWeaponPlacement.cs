using UnityEngine;
using System.Collections.Generic;

public class ProceduralWeaponPlacement : MonoBehaviour
{
    public List<GameObject> objectsToSpawn; // List of prefabs to spawn
    public int spawnCount = 10;
    public float spawnRange = 20f;
    public Vector3 center = Vector3.zero;
    public Transform centerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnObjects()
    {
        Vector3 spawnCenter = centerTransform != null ? centerTransform.position : center;

        if (objectsToSpawn == null || objectsToSpawn.Count == 0 || GameManager.Instance.weaponLevel <= 0)
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
            Instantiate(prefab, randomPos, Quaternion.identity);
        }
    }
}
