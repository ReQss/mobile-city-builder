using UnityEngine;

public class RoadPTGGenerator : MonoBehaviour
{
    public Terrain terrain;
    public int roadTextureIndex = 1; // indeks tekstury drogi (0 - trawa, 1 - droga)
    public Vector3 startWorldPos = new Vector3(20, 0, 20);
    public Vector3 endWorldPos = new Vector3(180, 0, 180);
    public float roadWidth = 5f;

    void Start()
    {
        if (terrain == null) terrain = Terrain.activeTerrain;
        PaintRoad();
    }

    void PaintRoad()
    {
        TerrainData data = terrain.terrainData;
        int mapWidth = data.alphamapWidth;
        int mapHeight = data.alphamapHeight;
        int numLayers = data.alphamapLayers;

        float[,,] alphamaps = data.GetAlphamaps(0, 0, mapWidth, mapHeight);

        // Zamień pozycje świata na współrzędne na splatmapie
        Vector3 terrainPos = terrain.transform.position;
        Vector3 start = startWorldPos - terrainPos;
        Vector3 end = endWorldPos - terrainPos;

        int steps = 100; // dokładność linii
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 pos = Vector3.Lerp(start, end, t);
            int x = Mathf.RoundToInt((pos.x / data.size.x) * mapWidth);
            int y = Mathf.RoundToInt((pos.z / data.size.z) * mapHeight);

            // promień w pikselach mapy
            int radius = Mathf.RoundToInt((roadWidth / data.size.x) * mapWidth);

            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            {
                for (int offsetX = -radius; offsetX <= radius; offsetX++)
                {
                    int px = x + offsetX;
                    int py = y + offsetY;

                    if (px >= 0 && px < mapWidth && py >= 0 && py < mapHeight)
                    {
                        float dist = Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY);
                        if (dist <= radius)
                        {
                            for (int l = 0; l < numLayers; l++)
                                alphamaps[py, px, l] = (l == roadTextureIndex) ? 1f : 0f;
                        }
                    }
                }
            }
        }

        data.SetAlphamaps(0, 0, alphamaps);
    }
}
