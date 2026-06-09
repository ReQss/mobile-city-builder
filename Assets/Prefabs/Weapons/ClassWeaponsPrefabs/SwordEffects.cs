using UnityEngine;

public class SwordEffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject vfxSparks;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private bool hasCollided = false;
    async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") && !hasCollided)
        {
            hasCollided = true; 
            //and destroy after 1s
            //wait for 0.5 s async to let the particle system play
            //spark position is middle distance between other position and current position
            Vector3 sparkPosition = (other.transform.position + transform.position) / 2;
            GameObject sparks = Instantiate(vfxSparks, sparkPosition, Quaternion.identity);
            sparks.GetComponent<ParticleSystem>().Play();
            Destroy(sparks, 0.5f);
            await System.Threading.Tasks.Task.Delay(300);
            Debug.Log("sparks");
            hasCollided = false;
            // Apply damage or effects to the enemy
        }
    }
}
