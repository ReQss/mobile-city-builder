using UnityEngine;

public class BulletNavigateEnemy : MonoBehaviour
{
    public float movementSpeed = 35f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && GetComponent<BoxCollider>().bounds.Contains(other.transform.position))
        {

            Debug.Log("Player is staying in the loot item trigger area.");
              transform.position = Vector3.MoveTowards(
            transform.position,
            other.transform.position,
            movementSpeed * Time.deltaTime
              );
            // Optional: Add logic for when the player stays in the trigger area
        }
    }
}
