using UnityEngine;

public class WallTransparency : MonoBehaviour
{
    public Transform player;
    public Material transparentMaterial;
    public float heightDifference = 0.0f; // 0 = dokładnie poniżej, >0 = z marginesem

    private Material originalMat;
    private Renderer rend;
    private bool isTransparent = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMat = rend.material;
        }
    }

    void Update()
    {
        if (player == null || rend == null) return;

        // Sprawdź, czy gracz jest niżej niż ściana (z marginesem heightDifference)
        if (player.position.y < transform.position.y - heightDifference)
        {
            if (!isTransparent)
            {
                rend.material = transparentMaterial;
                isTransparent = true;
            }
        }
        else
        {
            if (isTransparent)
            {
                rend.material = originalMat;
                isTransparent = false;
            }
        }
    }
}
