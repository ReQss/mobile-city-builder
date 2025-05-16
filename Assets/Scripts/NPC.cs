using UnityEngine;

public class NPC : MonoBehaviour
{
    public float detectionRadius = 5f; // Set this in the Inspector
    public LayerMask playerLayer;      // Assign the Player layer in the Inspector
    public GameObject bubblePrefab; // Assign the bubble prefab in the Inspector

        public static bool anyNPCDetectsPlayer = false;

    void Start()
    {
        
    }//create function
    // This function is called when the script instance is being loaded

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
            Debug.Log("Player is nearby (layer check)!");
            if (bubblePrefab != null)
            {
                bubblePrefab.SetActive(true);
            }
            anyNPCDetectsPlayer = true;
        }
        else
        {
            if (bubblePrefab != null)
            {
                bubblePrefab.SetActive(false);
            }
            // Only set to false if no other NPCs are detecting the player
            // This will be handled globally in PlayerMovement
        }
    }
}
