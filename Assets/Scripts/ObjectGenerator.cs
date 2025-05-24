using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public List<GameObject> prefabs;
    // [SerializeField]
    public Vector2Int xSize;
    // public int ySize;
    public Vector2Int zSize;
    public Vector2Int ySize;
    public LayerMask layerToExclude;
    public float radius = 0;
    public bool positionClear = true;
    public int numberOfObjects = 0;
    public GameObject instantiatedObjectsFolder;
    public bool spawnObjects = true;

    void Start()
    {
        if (spawnObjects == false) return;
        for (int i = 0; i < numberOfObjects; i++)
        {
            Generate();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = positionClear ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public GameObject getRandomObject()
    {
        int randomValue = Random.Range(0, prefabs.Count);
        return prefabs[randomValue];
    }
    public virtual void Generate()
    {
        int layersToExclude = (1 << gameObject.layer) | (1 << layerToExclude.value);
        bool positionFound = false;
        int maxAttempts = 100;
        int attempts = 0;

        while (!positionFound && attempts < maxAttempts)
        {
            int randomX = Random.Range(xSize.x, xSize.y);
            int randomZ = Random.Range(zSize.x, zSize.y);
            int randomY = 10; // zawsze nad terenem
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            RaycastHit hit;
            if (!Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity))
            {
                attempts++;
                continue;
            }
            if (hit.collider.tag != "Ground")
            {
                attempts++;
                continue;
            }
            if (Physics.CheckSphere(hit.point, radius + 5f, layersToExclude))
            {
                positionClear = false;
                attempts++;
                continue;
            }
            positionClear = true;

            GameObject randomPrefab = getRandomObject();
            GameObject spawnedObject = Instantiate(randomPrefab, hit.point, Quaternion.identity);
            spawnedObject.transform.SetParent(instantiatedObjectsFolder.transform);
            positionFound = true;
            break;
        }
        if (!positionFound)
        {
            Debug.LogWarning("Nie znaleziono pozycji dla drzewa po " + maxAttempts + " próbach.");
        }
    }



}