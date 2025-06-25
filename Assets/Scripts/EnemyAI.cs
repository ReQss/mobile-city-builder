using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

using UnityEngine.UI;
public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public AudioClip walkSound;
    public AudioClip attackSound;
    public AudioClip screamSound;
    private AudioSource audioSource;
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
    public GameObject DamageDealtPrefabSmall;
    public GameObject DamageDealtPrefabMagic2;
    public Transform DamageSpawnPoint;
    public Image healthBarImage;
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
    private float areaHitboxDamageTimer = 0f;
    public int damageAmount = 10;
    public int expAmount = 100;
    public int maxHealth;
    
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
                GameManager.Instance.AddExp(expAmount);
            }
        }
    }
  private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float fill = Mathf.Clamp01((float)health / maxHealth); // Cast to float!
            healthBarImage.fillAmount = fill;
        }
       
    }
    void Start()
    {
        maxHealth = health;
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
        UpdateHealthBar();
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
                 Vector3 lookDirection = (player.position - transform.position).normalized;
    lookDirection.y = 0; // Only rotate horizontally
    if (lookDirection != Vector3.zero)
        transform.rotation = Quaternion.LookRotation(lookDirection);
            }
            
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // if(healthBarImage != null)
            // GameUIHandler.Instance.PlayBossMusic();
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
            int damageAmount =(int)( (float)GameManager.Instance.playerAttack /1.3f);
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

            Destroy(other.gameObject, 0.1f);
            Destroy(damageDealt, 0.5f);
        }
        else if (other.CompareTag("Magic"))
        {
            int damageAmount = GameManager.Instance.playerAttack * 4;
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

            Destroy(other.gameObject, 0.4f);
            Destroy(damageDealt, 0.5f);

            // Start DoT effect (cancel previous if running)
            if (magicDotCoroutine != null)
                StopCoroutine(magicDotCoroutine);
            magicDotCoroutine = StartCoroutine(MagicDotEffect());
        }
        else if (other.CompareTag("Versus"))
        {
            int damageAmount = GameManager.Instance.playerAttack * 8;
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

            Destroy(other.gameObject, 0.4f);
            Destroy(damageDealt, 0.5f);

            // // Start DoT effect (cancel previous if running)
            // if (magicDotCoroutine != null)
            //     StopCoroutine(magicDotCoroutine);
            // magicDotCoroutine = StartCoroutine(MagicDotEffect());
        }
        else if (other.CompareTag("VersusBullet"))
        {
            int damageAmount = GameManager.Instance.playerAttack *6;
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

            Destroy(other.gameObject, 0.4f);
            Destroy(damageDealt, 0.5f);

            // // Start DoT effect (cancel previous if running)
            // if (magicDotCoroutine != null)
            //     StopCoroutine(magicDotCoroutine);
            // magicDotCoroutine = StartCoroutine(MagicDotEffect());
        }
        else if (other.CompareTag("SwordHitbox"))
        {
            int damageAmount = GameManager.Instance.playerAttack * 2;
            TakeDamage(damageAmount);
            GameObject damageDealt = Instantiate(DamageDealtPrefab, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);

            var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = damageAmount.ToString();
            }

            Destroy(damageDealt, 0.5f);

            // Debug.Log("Enemy hit by sword!");
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("AreaHitbox"))
        {
            areaHitboxDamageTimer += Time.deltaTime;
            if (areaHitboxDamageTimer >= 0.3f)
            {
                int damageAmount = 1;
                TakeDamage(damageAmount);
                GameObject damageDealt = Instantiate(DamageDealtPrefabSmall, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);

                var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = damageAmount.ToString();
                }

                Destroy(damageDealt, 0.5f);

                areaHitboxDamageTimer = 0f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("AreaHitbox"))
    {
        areaHitboxDamageTimer = 0f;
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
            GameObject bullet = null;
            if (parentFolder != null)
                bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, lookRotation, parentFolder);
            else bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, lookRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
            }
            Destroy(bullet, 3f);
        }
    }

    public void PlayWalkSound()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (walkSound != null && audioSource != null)
        {
            if (audioSource.isPlaying && audioSource.clip == walkSound)
                return;

            audioSource.Stop();
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.pitch = Random.Range(0.95f, 1.05f); // Dodaj losowy pitch
            audioSource.Play();
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (attackSound != null && audioSource != null)
        {
            audioSource.Stop(); // Dodaj to!
            audioSource.clip = attackSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    public void StopWalkSound()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop();
        }

    }
    public void PlayScreamSound()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (screamSound != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = screamSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}
