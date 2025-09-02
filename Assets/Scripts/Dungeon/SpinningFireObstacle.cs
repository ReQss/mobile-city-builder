using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpinningFireObstacle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int damageIntervalTime = 100;
    private bool isTakingDamage = false;
    [SerializeField]
    private List<FireCollisionDetector> fireCollisions = new List<FireCollisionDetector>(); // 4 on each side
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _= DamagePlayer();
    }
    public async Task DamagePlayer()
    {
        if(isTakingDamage) return;
        foreach (FireCollisionDetector fireCollision in fireCollisions)
        {
            if (fireCollision.isCollidingWithPlayer)
            {
                isTakingDamage = true;
                PlayerMovement.playerMovementInstance.TakeDamage(1);
                await Task.Delay(damageIntervalTime);
                isTakingDamage = false;
            }
        }
    }
    
}
