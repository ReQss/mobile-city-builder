using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
public class GrassObject
{
    public Mesh highPolyMesh;
    public Mesh lowPolyMesh;
    public Material material;
    public float probability;
    public Vector3 scale = Vector3.one;
}
[System.Serializable]
public class TerrainHeight
{
    public float grassHeight;
    public float rockHeight;
    public float sandHeight;
}
public class GrassGenerator : MonoBehaviour
{
    public TerrainHeight terrainHeightLevel;
    public Vector3 terrainCenter;
    public Camera playerCamera;
    public Transform terrain;
    public float chunkSize = 10f;
    // Dodaj strukturę pomocniczą na dane chunków
    private class GrassChunkData
    {
        public List<Matrix4x4> matrices = new();
        public List<Vector3> positions = new();
        public GrassObject[] grassObjects;
        public float minY = Mathf.Infinity;
        public float maxY = -Mathf.Infinity;
    }
    private Dictionary<Vector2Int, GrassChunkData> grassChunks = new();
    private Dictionary<Vector2Int, GrassObject[]> grassObjectsByChunk = new Dictionary<Vector2Int, GrassObject[]>();
    public List<GrassObject> grassObjects;
    public int grassCount = 10000;
    public float areaSize = 100f;
    public LayerMask groundLayer;
    public Transform player;
    public float renderRadius = 50f;
    public float switchDistance = 30f;
    [SerializeField]
    private float adjustGrassHeight = 0.6f;
    private ComputeBuffer matrixBuffer;

    public float spacingFactor = 0.5f;
    public int chanceForGrassSpawn = 4;
    public List<string> tagsToAvoid = new List<string> { "Building", "Water" };

    private Vector3 lastPlayerPosition;
    private Quaternion lastCameraRotation;
    private Dictionary<GrassObject, List<Matrix4x4>> visibleHighPolyGrass = new();
    private Dictionary<GrassObject, List<Matrix4x4>> visibleLowPolyGrass = new();

    void Start()
    {
        NormalizeProbabilities();
        GenerateGrass();
        lastPlayerPosition = player.position;
        lastCameraRotation = playerCamera.transform.rotation;
        UpdateVisibleGrass();
    }

    void NormalizeProbabilities()
    {
        float totalProbability = 0f;
        foreach (var grassObject in grassObjects)
        {
            totalProbability += grassObject.probability;
        }

        for (int i = 0; i < grassObjects.Count; i++)
        {
            grassObjects[i].probability /= totalProbability;
        }
    }

    GrassObject SelectGrassObject()
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var grassObject in grassObjects)
        {
            cumulativeProbability += grassObject.probability;
            if (randomValue < cumulativeProbability)
            {
                return grassObject;
            }
        }

        return grassObjects[grassObjects.Count - 1];
    }

    void GenerateGrass()
    {
        int rowCount = Mathf.CeilToInt(Mathf.Sqrt(grassCount));
        float cellSize = areaSize / rowCount * spacingFactor;
        int index = 0;

        for (int x = 0; x < rowCount; x++)
        {
            for (int z = 0; z < rowCount; z++)
            {
                if (index >= grassCount)
                    return;
                int chanceForGrass = Random.Range(1, chanceForGrassSpawn);
                if (chanceForGrass == 2) continue;

                float randomPositionOffset = Random.Range(0f, 1f);
                Vector3 position = new Vector3(
                    (x * cellSize) - (areaSize / 2f) + terrainCenter.x + randomPositionOffset,
                    100f,
                    randomPositionOffset + (z * cellSize) - (areaSize / 2f) + terrainCenter.z
                );

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
                {
                    bool skipTag = false;
                    for (int i = 0; i < tagsToAvoid.Count; i++)
                    {
                        if (hit.collider.CompareTag(tagsToAvoid[i]))
                        {
                            skipTag = true;
                            break;
                        }
                    }
                    position = hit.point;
                    if (position.y > terrainHeightLevel.rockHeight || position.y < terrainHeightLevel.sandHeight)
                        continue;
                    if (skipTag) continue;
                    position.y += adjustGrassHeight;

                    float randomScaleFactor = Random.Range(0.8f, 1.2f);
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    Vector2Int chunkCoord = new Vector2Int(
                        Mathf.FloorToInt(position.x / chunkSize),
                        Mathf.FloorToInt(position.z / chunkSize)
                    );

                    GrassObject selectedGrassObject = SelectGrassObject();
                    Vector3 finalScale = selectedGrassObject.scale * randomScaleFactor;
                    Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, finalScale);

                    if (!grassChunks.ContainsKey(chunkCoord))
                    {
                        grassChunks[chunkCoord] = new GrassChunkData
                        {
                            grassObjects = new GrassObject[grassCount]
                        };
                    }
                    var chunk = grassChunks[chunkCoord];
                    chunk.matrices.Add(matrix);
                    chunk.positions.Add(position);
                    chunk.grassObjects[chunk.matrices.Count - 1] = selectedGrassObject;
                    if (position.y < chunk.minY) chunk.minY = position.y;
                    if (position.y > chunk.maxY) chunk.maxY = position.y;

                    index++;
                }
            }
        }
        matrixBuffer = new ComputeBuffer(grassCount, 64);
    }

    void Update()
    {
        // Sprawdzaj tylko, czy gracz się ruszył lub kamera się obróciła
        if ((player.position - lastPlayerPosition).sqrMagnitude > 1f || Quaternion.Angle(playerCamera.transform.rotation, lastCameraRotation) > 2f)
        {
            lastPlayerPosition = player.position;
            lastCameraRotation = playerCamera.transform.rotation;
            UpdateVisibleGrass();
        }

        // Rysuj trawę (samo rysowanie jest szybkie)
        foreach (var kvp in visibleHighPolyGrass)
        {
            if (kvp.Value.Count > 0)
            {
                matrixBuffer.SetData(kvp.Value.ToArray());
                Graphics.DrawMeshInstanced(kvp.Key.highPolyMesh, 0, kvp.Key.material, kvp.Value.ToArray(), kvp.Value.Count, null, ShadowCastingMode.Off, receiveShadows: true);
            }
        }
        foreach (var kvp in visibleLowPolyGrass)
        {
            if (kvp.Value.Count > 0)
            {
                matrixBuffer.SetData(kvp.Value.ToArray());
                Graphics.DrawMeshInstanced(kvp.Key.lowPolyMesh, 0, kvp.Key.material, kvp.Value.ToArray(), kvp.Value.Count, null, ShadowCastingMode.Off, receiveShadows: true);
            }
        }
    }


    // Przeniesiona logika widoczności do osobnej funkcji
    void UpdateVisibleGrass()
    {
        visibleHighPolyGrass.Clear();
        visibleLowPolyGrass.Clear();

        Vector3 playerPosition = player.position;
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        foreach (var kvp in grassChunks)
        {
            Vector2Int chunkCoord = kvp.Key;
            var chunk = kvp.Value;
            Vector3 chunkCenter = new Vector3(
                chunkCoord.x * chunkSize + chunkSize / 2,
                (chunk.minY + chunk.maxY) / 2,
                chunkCoord.y * chunkSize + chunkSize / 2
            );
            float chunkHeight = chunk.maxY - chunk.minY;
            Bounds chunkBounds = new Bounds(
                chunkCenter,
                new Vector3(chunkSize, chunkHeight, chunkSize)
            );

            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, chunkBounds))
                continue;
            if (Vector3.Distance(playerPosition, chunkCenter) > renderRadius)
                continue;

            for (int i = 0; i < chunk.matrices.Count; i++)
            {
                float distanceToPlayer = Vector3.Distance(playerPosition, chunk.positions[i]);
                GrassObject grassObject = chunk.grassObjects[i];

                if (distanceToPlayer <= switchDistance)
                {
                    if (!visibleHighPolyGrass.ContainsKey(grassObject))
                        visibleHighPolyGrass[grassObject] = new List<Matrix4x4>();
                    visibleHighPolyGrass[grassObject].Add(chunk.matrices[i]);
                }
                else
                {
                    if (!visibleLowPolyGrass.ContainsKey(grassObject))
                        visibleLowPolyGrass[grassObject] = new List<Matrix4x4>();
                    visibleLowPolyGrass[grassObject].Add(chunk.matrices[i]);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (matrixBuffer != null)
            matrixBuffer.Release();
    }
}
