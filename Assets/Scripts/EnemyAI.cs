using System.Collections;
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
    public GameObject DamageDealtPrefabMagic2;
    public Transform DamageSpawnPoint;
    public int coinsAmount = 50;
    public bool isMele = false;
    public bool isRanged = false;

    public GameObject bulletPrefab; // Assign in inspector
    public Transform bulletSpawnPoint; // Assign in inspector
    public float bulletSpeed = 15f;
    public float rangedAttackCooldown = 2f;
    private float rangedAttackTimer = 0f;
    public GameObject enemyBulletFolder;

    private Coroutine magicDotCoroutine; // Add this field to prevent overlapping DoTs
    public bool isShoting = false;

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

        // Prevent movement while shooting
        if (isShoting)
        {
            agent.isStopped = true;
            if (anim != null)
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isAttacking", true);
            }
            return;
        }

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
                anim.SetBool("isAttacking", false);
            }

            // Ranged attack logic
            if (isRanged && distanceToPlayer <= attackRange * 3f) // Ranged enemies attack from further away
            {
                agent.isStopped = true;
                rangedAttackTimer -= Time.deltaTime;

                // Always face the player when attacking
                Vector3 lookDirection = (player.position - transform.position).normalized;
                lookDirection.y = 0; // Keep only horizontal rotation
                if (lookDirection != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(lookDirection);

                if (rangedAttackTimer <= 0f)
                {
                    // ShootAtPlayer(); // Call from animation event instead
                    rangedAttackTimer = rangedAttackCooldown;
                }
                if (anim != null)
                {
                    anim.SetBool("isAttacking", true);
                }
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
                        anim.SetBool("isAttacking", false); 
                    }
                }
            }
            else
            {
                if (anim != null)
                {

                    anim.SetBool("isRunning", false);
                    anim.SetBool("isAttacking", false); 
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
            int damageAmount = GameManager.Instance.playerAttack*2;
            TakeDamage(damageAmount);
            GameObject damageDealt = Instantiate(DamageDealtPrefab, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);

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

            Destroy(other.gameObject,0.1f);
            Destroy(damageDealt, 0.5f);
        }
        else if (other.CompareTag("Magic"))
        {
            int damageAmount = GameManager.Instance.playerAttack*6;
            TakeDamage(damageAmount);
            GameObject damageDealt = Instantiate(DamageDealtPrefabMagic2, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);

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

            Destroy(other.gameObject,0.4f);
            Destroy(damageDealt, 0.5f);

            // Start DoT effect (cancel previous if running)
            if (magicDotCoroutine != null)
                StopCoroutine(magicDotCoroutine);
            magicDotCoroutine = StartCoroutine(MagicDotEffect());
        }
        else if (other.CompareTag("SwordHitbox"))
        {
            int damageAmount = GameManager.Instance.playerAttack * 3;
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

    // Add this coroutine at the end of the class
    private IEnumerator MagicDotEffect()
    {
        float duration = 2f;
        float tickInterval = 0.1f;
        int tickCount = Mathf.FloorToInt(duration / tickInterval);
        int tickDamage = 3;

        for (int i = 0; i < tickCount; i++)
        {
            TakeDamage(tickDamage);

            GameObject damageDealt = Instantiate(DamageDealtPrefabMagic2, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);
            var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = tickDamage.ToString();
            }

            Rigidbody rb = damageDealt.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)).normalized;
                float knockbackForce = 1f;
                rb.AddForce(randomDir * knockbackForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 25f, ForceMode.Impulse);
            }

            Destroy(damageDealt, 0.5f);

            yield return new WaitForSeconds(tickInterval);
        }
        magicDotCoroutine = null;
    }

    // Add this method to the class
    public void ShootAtPlayer()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null && player != null)
        {
            Transform parentFolder = null;
            if (enemyBulletFolder != null)
            {
                parentFolder = enemyBulletFolder.transform;
            }

            Vector3 dir = (player.position - bulletSpawnPoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            GameObject bullet=null;
            if (parentFolder != null)
                bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, lookRotation, parentFolder);
            else bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, lookRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = dir * bulletSpeed;
            }
            Destroy(bullet, 3f);
        }
    }
}
