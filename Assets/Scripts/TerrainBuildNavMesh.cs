using Unity.AI.Navigation;
using UnityEngine;

public class TerrainBuildNavMesh : MonoBehaviour
{
     private NavMeshSurface navMeshSurface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public void BakeNavMeshAfterGeneration()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface != null)
        {
            Debug.Log("Baking NavMesh after procedural generation...");
            navMeshSurface.BuildNavMesh();
        }
    }
}

