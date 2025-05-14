using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Assign the player transform in the inspector or via script
    public float chaseRange = 10f;
    public float patrolRange = 5f;
    public float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private float patrolTimer;
    private bool isChasing = false;
    private float lostPlayerTimer = 0f;
    private bool playerWasInRange = false;
    public GameObject EnemyCanvasLockOn;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewPatrolTarget();
    }

    void Update()
    {
        Animator anim = GetComponentInChildren<Animator>();
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            playerWasInRange = true;
            lostPlayerTimer = 0f;
            agent.SetDestination(player.position);

            if (anim != null)
            {
                anim.SetBool("isRunning", true);
            }
        }
        else
        {
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
                    }
                }
                else
                {
                    // Keep chasing for 3 seconds after losing player
                    agent.SetDestination(player.position);
                    if (anim != null)
                    {
                        anim.SetBool("isRunning", true);
                    }
                    if (EnemyCanvasLockOn != null)
                    {
                        EnemyCanvasLockOn.SetActive(true);
                    }
                }
            }
            else
            {
                // Set animator isRunning to false when not chasing
                if (anim != null)
                {
                    anim.SetBool("isRunning", false);
                }
                if (EnemyCanvasLockOn != null)
                {
                    EnemyCanvasLockOn.SetActive(false);
                }

                // Patrol logic
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
}
