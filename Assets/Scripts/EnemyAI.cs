using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Assign the player transform in the inspector or via script
    public float chaseRange = 10f;
    public float patrolRange = 5f;
    public float attackRange = 1f;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private float patrolTimer;
    private bool isChasing = false;
    private float lostPlayerTimer = 0f;
    private bool playerWasInRange = false;
    public bool EnemyCanvasLockOnIsEnabled = false;
    public GameObject EnemyCanvasLockOn;
    public int health = 100;
    public GameObject DamageDealtPrefab;
    public Transform DamageSpawnPoint;
    public int coinsAmount = 50;
    public bool isMele = false;
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                GameManager.Instance.coinsCollected += coinsAmount;
                if (QuestManager.Instance.currentQuest != null)
                {
                    if (QuestManager.Instance.currentQuest.questType == QuestType.KillEnemies)
                    {
                        QuestManager.Instance.currentQuest.currentAmount++;
                        QuestManager.Instance.CheckQuestProgress(QuestManager.Instance.currentQuest);
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewPatrolTarget();
    }

    void Update()
    {
        
        EnemyMovementLogicAndAnimations();

        if (EnemyCanvasLockOn != null)
        {
            EnemyCanvasLockOn.SetActive(EnemyCanvasLockOnIsEnabled);
        }
    }
    private void EnemyMovementLogicAndAnimations()
    {
         Animator anim = GetComponentInChildren<Animator>();
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && isMele)
        {
            // Stop moving and attack
            agent.isStopped = true;
            if (anim != null)
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isAttacking", true);
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            playerWasInRange = true;
            lostPlayerTimer = 0f;
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (anim != null)
            {
                anim.SetBool("isRunning", true);
                anim.SetBool("isAttacking", false); // <-- Reset attacking when chasing
            }
        }
        else
        {
            agent.isStopped = false; // Resume movement if not attacking
            if (isChasing && playerWasInRange)
            {
                lostPlayerTimer += Time.deltaTime;
                if (lostPlayerTimer >= 3f)
                {
                    isChasing = false;
                    playerWasInRange = false;
                    SetNewPatrolTarget();

                    if (anim != null)
                    {
                        anim.SetBool("isRunning", false);
                        anim.SetBool("isAttacking", false);
                    }
                }
                else
                {
                    agent.SetDestination(player.position);
                    if (anim != null)
                    {
                        anim.SetBool("isRunning", true);
                        anim.SetBool("isAttacking", false); // <-- Reset attacking when chasing
                    }
                }
            }
            else
            {
                if (anim != null)
                {
                    
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isAttacking", false); // <-- Reset attacking when patrolling
                }

                if (Vector3.Distance(transform.position, patrolTarget) <= 0.5f)
                {
                    patrolTimer += Time.deltaTime;
                    if (patrolTimer >= patrolWaitTime)
                    {
                        SetNewPatrolTarget();
                        patrolTimer = 0f;
                    }
                }
                else
                {
                    agent.SetDestination(patrolTarget);
                }
            }
        }
    }
    void SetNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection.y = 0;
        patrolTarget = transform.position + randomDirection;
        agent.SetDestination(patrolTarget);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        // ApplySlow(2f, 1f);
    }

    private void ApplySlow(float slowFactor, float duration)
    {
        float originalSpeed = agent.speed;
        agent.speed = originalSpeed / slowFactor;

        StartCoroutine(RevertSpeedAfterDelay(originalSpeed, duration));
    }

    private System.Collections.IEnumerator RevertSpeedAfterDelay(float originalSpeed, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null)
        {
            agent.speed = originalSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            int damageAmount = 20;
            TakeDamage(damageAmount); 
            GameObject damageDealt = Instantiate(DamageDealtPrefab, new Vector3(DamageSpawnPoint.position.x,3.2f,DamageSpawnPoint.position.z), Quaternion.identity);

            var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = damageAmount.ToString();
            }

            Vector3 randomDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)).normalized;
            float knockbackForce = 2f; 
            Rigidbody rb = damageDealt.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(randomDir * knockbackForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 50f, ForceMode.Impulse);
            }

            Destroy(other.gameObject);
            Destroy(damageDealt, 0.5f); 
        }
        else if (other.CompareTag("SwordHitbox"))
        {
            int damageAmount = 35;
            TakeDamage(damageAmount);
            GameObject damageDealt = Instantiate(DamageDealtPrefab, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);

            var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = damageAmount.ToString();
            }

            Destroy(damageDealt, 0.5f);

            Debug.Log("Enemy hit by sword!");
        }
    }
}
