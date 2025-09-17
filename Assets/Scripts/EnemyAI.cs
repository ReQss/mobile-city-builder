using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

using UnityEngine.UI;
using System.Threading.Tasks;
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
    // public GameObject DamageDealtPrefab;
    public GameObject DamageDealtPrefabSmall;
    public GameObject DamageDealtPrefabMagic2;
    public Transform DamageSpawnPoint;
    public Image healthBarImage;
    public int coinsAmount = 50;
    public bool isMele = false;
    public bool isRanged = false;

    // public GameObject bulletPrefab; // Assign in inspector
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
    private bool isMovementLocked = false;
    public bool hasKnockbackEffect = false;
    public bool explodeAfterDeath = false;
    [SerializeField]
    private GameObject explosionEffectPrefab;
    private bool blockMovement = false;
    
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                GameManager.Instance.coinsCollected += coinsAmount;
                DungeonRewardsInfo.Instance.goldCollected += coinsAmount;
                if (QuestManager.Instance.currentQuest != null)
                {
                    if (QuestManager.Instance.currentQuest.questType == QuestType.KillEnemies)
                    {
                        QuestManager.Instance.currentQuest.currentAmount++;
                        QuestManager.Instance.CheckQuestProgress(QuestManager.Instance.currentQuest);
                    }
                }
                if (explodeAfterDeath)
                {
                    _ = Explode();
                }
                else
                {
                    _ = EnemyDeath();
                }
            }
        }
    }
    private async Task EnemyDeath()
    {
        blockMovement = true;
        
        GetComponent<Animator>().enabled = false;
        
         var agent = GetComponent<NavMeshAgent>();
        if (agent) agent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;


        GameManager.Instance.AddExp(expAmount);
        DungeonRewardsInfo.Instance.experienceCollected += expAmount;
        await Task.Delay(3000);
        ReturnToPool();
    }
    
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float fill = Mathf.Clamp01((float)health / maxHealth);
            healthBarImage.fillAmount = fill;
        }

    }
    public async Task Explode()
    {
        agent.isStopped = true;
        isMovementLocked = true;
        
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetBool("explosion", true);
        await Task.Delay(2000);

        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        
        GameManager.Instance.AddExp(expAmount);
        DungeonRewardsInfo.Instance.experienceCollected += expAmount;
        await Task.Delay(1500);

        ReturnToPool();
    }
        private void ReturnToPool()
    {
        switch (gameObject.name.Replace("(Clone)", "").Trim())
        {
            case "ArcherEnemy":
                EnemyPool.Instance.ReturnArcherEnemy(gameObject);
                break;
            case "ThugEnemy":
                EnemyPool.Instance.ReturnThugEnemy(gameObject);
                break;
            case "BossKnightEnemy":
                EnemyPool.Instance.ReturnBossKnightEnemy(gameObject);
                break;
            case "BlackWidowEnemy":
                EnemyPool.Instance.ReturnBlackWidowEnemy(gameObject);
                break;
            case "RedWidowEnemy":
                EnemyPool.Instance.ReturnRedWidowEnemy(gameObject);
                break;
            case "GrayWidowEnemy":
                EnemyPool.Instance.ReturnGrayWidowEnemy(gameObject);
                break;
            case "BossWidowEnemy":
                EnemyPool.Instance.ReturnBossWidowEnemy(gameObject);
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }
    public void ResetEnemy(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        health = maxHealth;
        blockMovement = false;
        isMovementLocked = false;

        // var rb = GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.linearVelocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        //     rb.isKinematic = false;
        // }
        
        // var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // if (agent != null)
        // {
        //     agent.enabled = true;
        // }
    }
    void Start()
    {
        maxHealth = health;
        agent = GetComponent<NavMeshAgent>();
        if(agent.enabled == true)
            SetNewPatrolTarget();
        if (player == null)
        {
            player = GameObject.Find("Player").transform;
        }
    }

    void Update()
    {
         if (blockMovement || agent == null || !agent.enabled)
        return;
        if (!blockMovement)
        {
            EnemyMovementLogicAndAnimations();
        }
        if (EnemyCanvasLockOn != null)
        {
            EnemyCanvasLockOn.SetActive(EnemyCanvasLockOnIsEnabled);
        }
        UpdateHealthBar();
    }
    
    private void EnemyMovementLogicAndAnimations()
    {
        if (isMovementLocked || agent == null || !agent.enabled) return;

        Animator anim = GetComponentInChildren<Animator>();
        if (player == null)
            return;

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

            agent.isStopped = true;
            if (explodeAfterDeath)
                _ = Explode();
            if (anim != null)
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isAttacking", true);
                Vector3 lookDirection = (player.position - transform.position).normalized;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(lookDirection);
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
            agent.isStopped = false;
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

                if (Vector3.Distance(transform.position, patrolTarget) <= 0.1f)
                {
                    SetNewPatrolTarget();

                }
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolWaitTime)
                {
                    SetNewPatrolTarget();
                    patrolTimer = 0f;
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
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.SetDestination(patrolTarget);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

    }

    public void SpawnDamageDealt(GameObject bullet, int damageDealtAmount)
    {
        GameObject damageDealt = BulletPool.Instance.GetDamageDealt();

        damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
        damageDealt.transform.rotation = Quaternion.identity;
        var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = damageDealtAmount.ToString();
        }
        Vector3 randomDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)).normalized;
        float knockbackForce = 2f;
        Rigidbody rb = damageDealt.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(randomDir * knockbackForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 50f, ForceMode.Impulse);
        }
        
        BulletPool.Instance.ReturnBullet(bullet);
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Bullet"))
        {
            int damageAmount = (int)PlayerMovement.playerMovementInstance.playerAttack;
            TakeDamage(damageAmount);
            SpawnDamageDealt(other.gameObject,damageAmount);

        }
        else if (other.CompareTag("Magic"))
        {
            int damageAmount = (int)PlayerMovement.playerMovementInstance.playerAttack * 4;
            TakeDamage(damageAmount);
            GameObject damageDealt = BulletPool.Instance.GetDamageDealt();
            damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
            damageDealt.transform.rotation = Quaternion.identity;
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

            BulletPool.Instance.ReturnMagicalBullet(other.gameObject);
        }
        else if (other.CompareTag("Versus"))
        {
            int damageAmount = (int)PlayerMovement.playerMovementInstance.playerAttack * 8;
            TakeDamage(damageAmount);
            GameObject damageDealt = BulletPool.Instance.GetDamageDealtMagic();
            damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
            damageDealt.transform.rotation = Quaternion.identity;
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

            BulletPool.Instance.ReturnVersusProjectile1(other.gameObject);

        }
        else if (other.CompareTag("VersusBullet"))
        {
            int damageAmount = (int)PlayerMovement.playerMovementInstance.playerAttack * 6;
            TakeDamage(damageAmount);
            GameObject damageDealt = BulletPool.Instance.GetDamageDealtMagic();
            damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
            damageDealt.transform.rotation = Quaternion.identity;
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

            BulletPool.Instance.ReturnVersusProjectile2(other.gameObject);
        }
        else if (other.CompareTag("SwordHitbox"))
        {
            int damageAmount = (int)PlayerMovement.playerMovementInstance.playerAttack * 2;
            TakeDamage(damageAmount);
            GameObject damageDealt = BulletPool.Instance.GetDamageDealt();
            damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
            damageDealt.transform.rotation = Quaternion.identity;
            var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = damageAmount.ToString();
            }
        }
        if (other.CompareTag("SwordHitbox") || other.CompareTag("Bullet") || other.CompareTag("Magic"))
        {
            if (PlayerMovement.playerMovementInstance.playerWeapon.knockbackEffect)
            {
                _ = KnockbackEffect(other.transform);
            }
            if (PlayerMovement.playerMovementInstance.playerWeapon.igniteEffect)
            {
                if (magicDotCoroutine != null)
                    StopCoroutine(magicDotCoroutine);
                magicDotCoroutine = StartCoroutine(MagicDotEffect());
            }
        }
        
    }
    private async Task KnockbackEffect(Transform objectPosition)
    {
        if(health<=0) return;
        if (isMovementLocked) return;
        isMovementLocked = true;

        if (agent == null || player == null) return;

        Vector3 knockbackDir = (transform.position - objectPosition.position).normalized;
        float knockbackDistance = 2f; 
        float knockbackDuration = 0.2f;

        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            if (blockMovement) break;
            agent.Move(knockbackDir * (knockbackDistance / knockbackDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            await Task.Yield();
        }

        isMovementLocked = false;
    }
    public async Task KnockbackEffectPublic(Transform objectPosition)
    {
        if(health<=0) return;
        if (isMovementLocked) return;
        isMovementLocked = true;

        if (agent == null || player == null) return;

        Vector3 knockbackDir = (transform.position - objectPosition.position).normalized;
        float knockbackDistance = 2f; 
        float knockbackDuration = 0.2f;

        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            if (blockMovement) break;
            agent.Move(knockbackDir * (knockbackDistance / knockbackDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            await Task.Yield();
        }

        isMovementLocked = false;
    }
    private IEnumerator MagicDotEffect()
    {
        float duration = 2f;
        float tickInterval = 0.1f;
        int tickCount = Mathf.FloorToInt(duration / tickInterval);
        int tickDamage = 3;

        for (int i = 0; i < tickCount; i++)
        {
            TakeDamage(tickDamage);

            // GameObject damageDealt = Instantiate(DamageDealtPrefabMagic2, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);
            GameObject damageDealt = BulletPool.Instance.GetDamageDealt();
            damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
            damageDealt.transform.rotation = Quaternion.identity;
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
            yield return new WaitForSeconds(tickInterval);
        }
        magicDotCoroutine = null;
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
                // GameObject damageDealt = Instantiate(DamageDealtPrefabSmall, new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z), Quaternion.identity);
                GameObject damageDealt = BulletPool.Instance.GetDamageDealt();
                damageDealt.transform.position = new Vector3(DamageSpawnPoint.position.x, 3.2f, DamageSpawnPoint.position.z);
                damageDealt.transform.rotation = Quaternion.identity;
                var tmp = damageDealt.GetComponent<TMPro.TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = damageAmount.ToString();
                }


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
    

    public void ShootAtPlayer()
    {
        
        if (BulletPool.Instance.enemyBulletPrefab != null && bulletSpawnPoint != null && player != null)
        {

            Vector3 dir = (player.position - bulletSpawnPoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            GameObject bullet = null;

            bullet = BulletPool.Instance.GetEnemyBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = lookRotation;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
            }
        }
    }
    public void ShootWebAtPlayer()
    {
                
        if (BulletPool.Instance.spiderWebPrefab != null && bulletSpawnPoint != null && player != null)
        {

            Vector3 dir = (player.position - bulletSpawnPoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            GameObject bullet = null;

            bullet = BulletPool.Instance.GetSpiderWeb();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = lookRotation;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
            }
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
