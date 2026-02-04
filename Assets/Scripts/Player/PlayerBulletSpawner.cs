using System.Threading.Tasks;
using UnityEngine;

public class PlayerBulletSpawner : MonoBehaviour
{
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InstantiateMultipleBullets()
    {
        int numberOfBullets = PlayerMovement.playerMovementInstance.currentBulletAmount;
        float angleStep = 15f; // Kąt między pociskami
        float startingAngle = -angleStep * (numberOfBullets - 1) / 2; // Początkowy kąt
        for (int i = 0; i < numberOfBullets; i++)
        {
            float currentAngle = startingAngle + angleStep * i;
            // Debug.Log(currentAngle);
            InstantiateAngleBullet(currentAngle);
        }
    }
     public void InstantiateAngleBullet(float angle)
    {
        GameObject projectile = BulletPool.Instance.GetBullet();
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        projectile.transform.position = new Vector3(
        PlayerMovement.playerMovementInstance.bulletSpawnPos.position.x,
        PlayerMovement.playerMovementInstance.bulletSpawnYPos,
        PlayerMovement.playerMovementInstance.bulletSpawnPos.position.z);
            Vector3 leftDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
    projectile.transform.rotation = Quaternion.LookRotation(leftDirection, Vector3.up); // Ustaw rotację na kierunek lotu
    rb.linearVelocity = leftDirection * PlayerMovement.playerMovementInstance.projectileSpeed;
    }

    public void InstantiateMagicBullet()
    {
        GameObject projectile = BulletPool.Instance.GetMagicalBullet();
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        projectile.transform.position = new Vector3(
        PlayerMovement.playerMovementInstance.bulletSpawnPos.position.x,
        PlayerMovement.playerMovementInstance.bulletSpawnYPos,
        PlayerMovement.playerMovementInstance.bulletSpawnPos.position.z);
        projectile.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up); // Ustaw rotację na kierunek lotu
        rb.linearVelocity = transform.forward * PlayerMovement.playerMovementInstance.projectileSpeed;
    }
}
