using UnityEngine;

public class ExplodeDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            playerMovement.TakeDamage(20);
            _ = playerMovement.KnockbackEffect(transform.position, 5, 0.3f);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")&& !other.CompareTag("Boss"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(300);
                _ = enemyAI.KnockbackEffectPublic(transform);
            }
        }
    }
}
