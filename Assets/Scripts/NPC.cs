using UnityEngine;

public class NPC : MonoBehaviour
{
    public float detectionRadius = 5f;
    public LayerMask playerLayer;      
    public GameObject bubblePrefab; 

        public static bool anyNPCDetectsPlayer = false;

    void Start()
    {
        
    }
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
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
        }
    }
}
