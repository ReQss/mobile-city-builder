using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnExplosions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private GameObject explosionAreaPrefab;
    async void Start()
    {
        while (true)
        {

            if (gameObject.activeSelf == false) return;
            await Explode(3); // liczba wybuchów
            await Task.Delay(5000); // 5 sekund
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private bool lookExplosion;
    public async Task Explode(int explosionCount)
    {
        if(PlayerMovement.playerMovementInstance == null) return;
        if (lookExplosion) return;
        // if (gameObject.activeSelf == false) return;
        lookExplosion = true;
        List<GameObject> explosionAreas = new List<GameObject>();
        List<Vector3> spawnPositions = new List<Vector3>();
        for (int i = 0; i < explosionCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            Vector3 spawnPosition = PlayerMovement.playerMovementInstance.transform.position + randomOffset;
            GameObject explosionarea = Instantiate(explosionAreaPrefab, spawnPosition, Quaternion.identity);
            spawnPositions.Add(spawnPosition);
            explosionAreas.Add(explosionarea);
        }
        await Task.Delay(2800);
        //delete explosion area
        foreach (var area in explosionAreas)
        {
            Destroy(area);
        }
        //spawn explosions
        foreach (Vector3 pos in spawnPositions)
        {
            Instantiate(explosionPrefab, pos, Quaternion.identity);
        }
        spawnPositions.Clear();
        explosionAreas.Clear();
        await Task.Yield();
        lookExplosion = false;
    }
}
