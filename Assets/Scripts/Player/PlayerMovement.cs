using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public GameObject dashEffectParticles;
    public AudioClip walkSound;
    public AudioClip attackSound;
    private AudioSource audioSource;

    public static PlayerMovement playerMovementInstance;
    [Header("Input Actions")]

    public Vector2 _moveDirection;


    public Transform playerHandPos;

    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Vector3 velocity;

    private Vector3 isoRight;
    private Vector3 isoUp;

    public Animator animator;

    private bool isCombat = false;
    public GameObject currentWeapon;
    public GameObject shootingProjectilePrefab;
    public GameObject accelerationArrowPrefab;
    public GameObject magicalProjectilePrefab;

    private float combatTimer = 0f;
    public float shootIntervals = 1f;
    public float shootIntervalBow = 4f;
    public float projectileSpeed = 35f;
    public float divideMovementSpeedWhenShooting = 3f;
    public float maxDistanceCheck = 10f;
    public Animator alertAnimator;
    public Vector3 closestEnemyInRangePosition;
    public float enemyRange = 15f;
    public bool notificationEnabled = false;
    private GameUIHandler gameUIHandler;
    public string currentWeaponName;
    public int shotsFired = 0;
    [Header("Player Stats")]
    public int health = 100;
    public float speed = 5f;
    public int playerAttack = 0;

    private bool enemiesTouching = false;
    private float healthTickTimer = 0f;
    public float enemyRangeDamage = 1f;
    private float lastEnemyTouchTime = -1f;
    public Image healthBarImage;
    public Animator healthBarAnimator;

    public Image healthBarImage2;
    private bool isPlayerDead = false;



    [Header("Player Dash Settings")]
    private bool isDashing = false;
    private float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    private float dashCooldown = 1f;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    public GameObject shieldPrefab;

    private bool isShieldActive = false;
    private float shieldDuration = 2f;
    private float shieldCooldown = 10f;
    public int numberOfUsesForWeapon = 150;
    private float shieldTimer = 0f;
    private float shieldCooldownTimer = 0f;
    public Vector3 moveDir;
    [Header("Navigation")]
    private NavMeshAgent navMeshAgent;
    public bool autoAttackEnabled = false;
    public bool isFighting = false;
    // public bool isTargetPicked = false;
    public GameObject enemiesFolder;
    public Transform currentTarget = null;
    public float stopDistance;
    public bool autoNavigationEnabled = false;
    public bool attackEnabled = false;
    public GameObject speedBoostPrefab;
    public TrailRenderer[] dashTrails;

    private int currentMeleDamage = 0;
    [Header("Versus settings")]
    public GameObject versusButton;
    public GameObject versusButtonMele;
    public GameObject versusProjectile;
    
    public GameObject versusProjectile2;
    public GameObject TutorialManager;
    public bool isInvincible;
    public TextMeshProUGUI healthValue;
    public ProceduralWeaponPlacement proceduralWeaponPlacement;
    public int playerBulletsCount = 1000;


    void Start()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();
        EnableOrDisableAttack();
        if (navMeshAgent != null)
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = false;
        }
        gameUIHandler = FindObjectOfType<GameUIHandler>();
        playerMovementInstance = this;

        controller = GetComponent<CharacterController>();

        isoRight = new Vector3(1, 0, -1).normalized;
        isoUp = new Vector3(1, 0, 1).normalized;
        health = GameManager.Instance.playerHealth;
        speed = GameManager.Instance.playerSpeed;
        playerAttack = GameManager.Instance.playerAttack;
        UpdateAdditionalBonuses();
        HandleActivePerks();
        GameUIHandler.Instance.HandleStatistics();
        UpdateHealthBar();
    }
    public void UpdateAdditionalBonuses()
    {
        List<InventoryItem> unlockedItems = GameManager.Instance.unlockedItems;

        int bonusHealth = 0;
        int bonusAttack = 0;
        int bonusAttackSpeed = 0;
        int bonusMovementSpeed = 0;
        foreach (var item in unlockedItems)
        {
            if (item.isEquipped)
            {
                bonusHealth += item.health;
                bonusAttack += item.attack;
                bonusAttackSpeed += item.attackSpeed;
                bonusMovementSpeed += item.movementSpeed;
            }
        }

        health = GameManager.Instance.playerHealth + bonusHealth;
        playerAttack = GameManager.Instance.playerAttack + bonusAttack;
        speed = GameManager.Instance.playerSpeed + bonusMovementSpeed;
    }

    void Update()
    {
        if (speed >= 8)
        {
            if (speedBoostPrefab != null)
                speedBoostPrefab.SetActive(true);
        }
        if (autoAttackEnabled)
        {

            // Find enemy
            if (currentTarget == null)
                currentTarget = FindClosestEnemy();
            if (currentTarget == null)
            {
                isCombat = false;
                autoAttackEnabled = false;
            }
            if (currentTarget != null)
            {
                RunTowardsTargetEnemy();
            }
        }
        if (attackEnabled)
        {
            if (currentWeapon == null)
            {
                isCombat = false;
            }
            else if (closestEnemyInRangePosition != Vector3.zero)
            {
                isCombat = true;
            }
            else isCombat = false;
        }
        else
        {
            GameObject temp = null;
            if (GameUIHandler.Instance.autoAttackUI != null)
                temp = GameUIHandler.Instance.autoAttackUI;
            if (temp != null)
                temp.SetActive(false);
        }
        if (autoNavigationEnabled)
        {
            NavigateTowardsCurrentQuestNpc();
        }
        else
        {
            GameObject temp = null;
            if (GameUIHandler.Instance.autonavigationUI != null)
                temp = GameUIHandler.Instance.autonavigationUI;
            if (temp != null)
                temp.SetActive(false);
        }

        if (isPlayerDead)
        {
            velocity = Vector3.zero;
            animator.SetBool("isDead", true);
            if (QuestManager.Instance.currentQuest.questType != QuestType.Unfreeze)
                PlayerDeathScene();
            return; 
        }
        Vector2 input = GameUIHandler.Instance.moveAction.action.ReadValue<Vector2>();
        moveDir = (isoRight * input.x + isoUp * input.y).normalized;

        float currentSpeed = isCombat ? speed / divideMovementSpeedWhenShooting : speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        // ROTATION
        if (autoNavigationEnabled && currentTarget != null && navMeshAgent != null)
        {
            Vector3 navVelocity = navMeshAgent.desiredVelocity;
            navVelocity.y = 0f;
            if (navVelocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(navVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else if (moveDir != Vector3.zero && (!autoAttackEnabled || currentTarget == null))
        {
            // Rotate towards input direction when not auto-attacking
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if ((autoNavigationEnabled || autoAttackEnabled) && currentTarget != null && navMeshAgent != null)
        {
            Vector3 navDir = navMeshAgent.nextPosition - transform.position;
            navDir.y = 0f;
            if (navDir.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(navDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        ShowWeapon();

        CheckForEnemiesInRange();
        FightingMode();
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0.1f;

            if ((autoNavigationEnabled || autoAttackEnabled) && currentTarget != null && navMeshAgent != null)
            {
                Vector3 navDir = navMeshAgent.nextPosition - transform.position;
                navDir.y = 0f;
                isMoving = navDir.magnitude > 0.1f;
            }
            if (autoNavigationEnabled || autoAttackEnabled) animator.SetBool("isRunning", true);
            else animator.SetBool("isRunning", isMoving);

            bool hasSword = false;
            bool hasCrossbow = false;
            bool hasRod = false;
            bool hasBow = false;
            if (currentWeapon != null)
            {
                string weaponName = currentWeapon.gameObject.name;
                hasSword = weaponName.IndexOf("Sword", System.StringComparison.OrdinalIgnoreCase) >= 0;
                hasCrossbow = weaponName.IndexOf("Crossbow", System.StringComparison.OrdinalIgnoreCase) >= 0;
                hasRod = weaponName.IndexOf("Rod", System.StringComparison.OrdinalIgnoreCase) >= 0;
                hasBow = weaponName.IndexOf("Bow", System.StringComparison.OrdinalIgnoreCase) >= 0;
            }

            animator.SetBool("isShooting", isCombat && hasCrossbow);
            animator.SetBool("isSlashing", isCombat && hasSword);
            animator.SetBool("isCasting", isCombat && hasRod); // Added for Rod
            animator.SetBool("bowAttack", isCombat && hasBow); // Added for Bow
        }

        if (controller.isGrounded)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move((moveDir * currentSpeed + velocity) * Time.deltaTime);
        CheckForItemsInRange();
        NPC.anyNPCDetectsPlayer = false;

        DamageHandling();

        if (Time.time - lastEnemyTouchTime > 0.1f)
        {
            enemiesTouching = false;
        }

        HandleActiveDash();
        HandleShield();
        if (navMeshAgent != null)
        {
            if (autoNavigationEnabled)
                navMeshAgent.nextPosition = transform.position;
        }
        CheckForBullets();
    }
    public void FightingMode()
    {
        if (currentWeapon != null)
        {
            string weaponName = currentWeapon.gameObject.name;
            if (weaponName.IndexOf("Sword", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                SlashingFunction(shootIntervals);
            }
            else if (weaponName.IndexOf("Crossbow", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShootingFunction(shootIntervals,numberOfUsesForWeapon);
            }
            else if (weaponName.IndexOf("Rod", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MagicalRod(shootIntervals);
            }
            else if (weaponName.IndexOf("Bow", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShootingFunction(shootIntervalBow,numberOfUsesForWeapon);

                //Bow
            }
          
        }
    }
    public void EnableOrDisableAutoAttack()
    {
        autoAttackEnabled = !autoAttackEnabled;
        isCombat = false;
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position);
        }

        autoNavigationEnabled = false;
        currentTarget = null;

    }
    public void EnableOrDisableAutoNavigation()
    {

        autoNavigationEnabled = !autoNavigationEnabled;
        if (autoNavigationEnabled == false)
        {
            GameUIHandler.Instance.autoNavigationNofication.SetActive(false);
        }
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position);
        }

        autoAttackEnabled = false;
        currentTarget = null;

    }
    public void EnableOrDisableAttack()
    {
        attackEnabled = !attackEnabled;
        if (attackEnabled && GameUIHandler.Instance.attackNotification != null)
        {
            GameUIHandler.Instance.attackNotification.SetActive(true);

        }
        if (attackEnabled == false)
        {
            isCombat = false;
             GameUIHandler.Instance.attackNotification.SetActive(false);
           
        }
        

    }
    public Transform FindClosestEnemy()
    {
        if (enemiesFolder == null) return null;

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform enemy in enemiesFolder.transform)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        isFighting = false;
        return closestEnemy;
    }

    public void NavigateTowardsCurrentQuestNpc()
    {
        if (autoNavigationEnabled == false) return;
        if (QuestManager.Instance == null || QuestManager.Instance.currentQuest == null || QuestManager.Instance.currentQuest.npc == null)
            return;

        Transform currentQuestNPC = QuestManager.Instance.currentQuest.npc.transform;
        currentTarget = currentQuestNPC;

        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(currentTarget.position);

        // Sync player position to NavMeshAgent
        if (Vector3.Distance(transform.position, navMeshAgent.nextPosition) > 0.01f)
        {
            controller.enabled = false; // Disable CharacterController to avoid conflicts
            transform.position = navMeshAgent.nextPosition;
            controller.enabled = true;
        }

        float distanceToNPC = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToNPC <= 5)
        {
            EnableOrDisableAutoNavigation();
            currentQuestNPC.GetComponent<DialogueTrigger>().TriggerDialogue();
            navMeshAgent.ResetPath();
        }
        else
        {
            isCombat = false;
            // No manual controller.Move here; NavMeshAgent handles movement
        }
    }

    public void RunTowardsTargetEnemy()
    {
        if (currentTarget == null || navMeshAgent == null || !autoAttackEnabled) return;

        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(currentTarget.position);

        // Sync player position to NavMeshAgent
        if (Vector3.Distance(transform.position, navMeshAgent.nextPosition) > 0.01f)
        {
            controller.enabled = false;
            transform.position = navMeshAgent.nextPosition;
            controller.enabled = true;
        }

        // Rotate towards movement direction
        if (navMeshAgent.hasPath && navMeshAgent.desiredVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 lookDir = navMeshAgent.desiredVelocity;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        float distanceToEnemy = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToEnemy <= stopDistance)
        {
            isFighting = true;
            isCombat = true;
            FightingMode();
            navMeshAgent.ResetPath();
        }
        else if (!isFighting)
        {
            isCombat = false;
        }
    }
    private void PlayerDeathScene()
    {
        if (gameUIHandler != null)
        {
            StartCoroutine(gameUIHandler.ShowGameOverScreen());
        }
    }
    public void HandleActivePerks()
    {
        foreach (PlayerPerks perk in GameManager.Instance.playerPerks)
        {
            if (perk.perkIsActive)
            {
                if (perk.perkName == "Iron Constitution")
                {
                    HandleHealthPerk(perk);
                }
                if (perk.perkName == "Swift Steps")
                {
                    HandleSpeedPerk(perk);
                }
            }
        }
    }
    public void HandleActiveDash()
    {
        foreach (PlayerPerks perk in GameManager.Instance.playerPerks)
        {
            if (perk.perkIsActive)
            {
                if (perk.perkName == "Windwalker's Step")
                {
                    HandleDash();
                }

            }
        }
    }
    private void DamageHandling()
    {
        if (isInvincible) return;
        if (isShieldActive)
        {
            healthBarAnimator.SetBool("isDamaged", false);
            return;
        }

        if (health <= 0)
        {
        QuestManager.Instance.CheckQuestProgress(QuestManager.Instance.currentQuest);
            if (gameUIHandler != null)
            {
                isPlayerDead = true;
            }
            return;
        }
        // if (enemiesTouching)
        // {
        //     healthTickTimer += Time.deltaTime;
            
        //     if(healthTickTimer >= 0.1f)
        //     {
        //         healthBarAnimator.SetBool("isDamaged", true);
        //         health -= currentMeleDamage / 4;
        //         Debug.Log("Damage taken: " + currentMeleDamage);
        //         Debug.Log("Health" + health);
        //         healthTickTimer = 0f;
        //         UpdateHealthBar();
        //     }
        // }
        // else
        // {
        //     healthTickTimer = 0f;
        //     healthBarAnimator.SetBool("isDamaged", false);
        // }
    }
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float fill = Mathf.Clamp01(health / (float)GameManager.Instance.playerHealth);
            healthValue.text = health.ToString();
            healthBarImage.fillAmount = fill;
        }
        if (healthBarImage2 != null)
        {
            float fill = Mathf.Clamp01(health / (float)GameManager.Instance.playerHealth);
            healthBarImage2.fillAmount = fill;
        }
    }

    private void RotateTowardsEnemy()
    {
        if (closestEnemyInRangePosition != Vector3.zero)
        {
            Vector3 lookDir = (closestEnemyInRangePosition - transform.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
    public void ShootingFunction(float shootInterval, int numberOfUses)
    {
        if (isCombat && currentWeapon != null && shootingProjectilePrefab != null)
        {
            stopDistance = 10f;
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval / 8)
            {
                combatTimer = 0f;

                RotateTowardsEnemy();
                GameObject projectile = Instantiate(shootingProjectilePrefab, currentWeapon.transform.position, currentWeapon.transform.rotation);

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (closestEnemyInRangePosition != Vector3.zero)
                    {
                        Vector3 direction = (closestEnemyInRangePosition - currentWeapon.transform.position).normalized;
                        rb.linearVelocity = direction * projectileSpeed;
                    }
                    else
                    {
                        rb.linearVelocity = transform.forward * projectileSpeed;
                    }
                }

                shotsFired++;
                if (gameUIHandler != null)
                {
                    gameUIHandler.UpdateUsesCount(numberOfUses - shotsFired);
                }
                if (shotsFired >= numberOfUses)
                {

                    if (gameUIHandler != null)
                    {
                        gameUIHandler.UpdateWeaponImage("Nothing");
                    }
                    Destroy(currentWeapon);
                    currentWeapon = null;
                    ChangeCombat();
                    shotsFired = 0;
                }
            }
        }
    }
    public void SlashingFunction(float shootInterval)
    {
        if (isCombat && currentWeapon != null && shootingProjectilePrefab != null)
        {
            stopDistance = 4f;
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval)
            {
                combatTimer = 0f;

                RotateTowardsEnemy();


                shotsFired++;
                if (gameUIHandler != null)
                {
                    gameUIHandler.UpdateUsesCount(numberOfUsesForWeapon - shotsFired);
                }
                if (shotsFired >= numberOfUsesForWeapon)
                {

                    if (gameUIHandler != null)
                    {
                        gameUIHandler.UpdateWeaponImage("Nothing");
                    }
                    Destroy(currentWeapon);
                    currentWeapon = null;
                    ChangeCombat();
                    shotsFired = 0;
                }
            }
        }
    }
    public void MagicalRod(float shootInterval)
    {
        if (isCombat && currentWeapon != null && magicalProjectilePrefab != null)
        {
            stopDistance = 10f;
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval / 1.5f)
            {
                combatTimer = 0f;

                RotateTowardsEnemy();
                Vector3 spawnPos = currentWeapon.transform.position + Vector3.up * 1f;
                GameObject projectile = Instantiate(magicalProjectilePrefab, spawnPos, currentWeapon.transform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (closestEnemyInRangePosition != Vector3.zero)
                    {
                        Vector3 direction = (closestEnemyInRangePosition - currentWeapon.transform.position).normalized;
                        rb.linearVelocity = direction * projectileSpeed;
                    }
                    else
                    {
                        rb.linearVelocity = transform.forward * projectileSpeed;
                    }
                }

                shotsFired++;
                if (gameUIHandler != null)
                {
                    gameUIHandler.UpdateUsesCount(numberOfUsesForWeapon - shotsFired);
                }
                if (shotsFired >= numberOfUsesForWeapon)
                {

                    if (gameUIHandler != null)
                    {
                        gameUIHandler.UpdateWeaponImage("Nothing");
                    }
                    Destroy(currentWeapon);
                    currentWeapon = null;
                    ChangeCombat();
                    shotsFired = 0;
                }
            }
        }
    }
    public void SpawnMagicBullet()
    {
        RotateTowardsEnemy();
        // Vector3 spawnPos = currentWeapon.transform.position + Vector3.up * 1f;
        GameObject projectile = Instantiate(magicalProjectilePrefab, this.transform.position, this.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
                {
                if (closestEnemyInRangePosition != Vector3.zero)
                    {
                    Vector3 direction = (closestEnemyInRangePosition - this.transform.position).normalized;
                    rb.linearVelocity = direction * projectileSpeed;
                    }
                    else
                    {
                        rb.linearVelocity = transform.forward * projectileSpeed;
                    }
                }
    }
    public void SpawnVersusMele()
    {
        versusButton.SetActive(false);
        versusButtonMele.SetActive(false);
        RotateTowardsEnemy();
        GameObject projectile = Instantiate(versusProjectile, this.transform.position, this.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (closestEnemyInRangePosition != Vector3.zero)
            {
                Vector3 direction = (closestEnemyInRangePosition - this.transform.position).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }
            else
            {
                rb.linearVelocity = transform.forward * projectileSpeed;
            }
        }
        
    }
    public void SpawnVersusBullet()
    {
        
        versusButton.SetActive(false);
        versusButtonMele.SetActive(false);
        RotateTowardsEnemy();
        GameObject projectile = Instantiate(versusProjectile2, this.transform.position, this.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (closestEnemyInRangePosition != Vector3.zero)
            {
                Vector3 direction = (closestEnemyInRangePosition - this.transform.position).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }
            else
            {
                rb.linearVelocity = transform.forward * projectileSpeed;
            }
        }
        
    }
    public void CheckForEnemiesInRange()
    {
        float range = enemyRange;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        Collider closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider;
                }
            }
        }

        if (closestEnemy != null)
        {
            closestEnemyInRangePosition = closestEnemy.transform.position;

            EnemyAI enemyAI = closestEnemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.EnemyCanvasLockOnIsEnabled = true;
            }
            if (autoAttackEnabled)
            {
                isCombat = true;
            }

            LookAtClosestEnemy();
        }
        else
        {
            closestEnemyInRangePosition = Vector3.zero;

            Collider[] allEnemies = Physics.OverlapSphere(transform.position, enemyRange * 2);
            foreach (Collider col in allEnemies)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyAI enemyAI = col.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.EnemyCanvasLockOnIsEnabled = false;
                    }
                }
            }
        }
    }
    public List<Collider> colliders;
    public void CheckForBullets()
    {
        // Debug.Log("Checking for bullets");
        // List<Collider> colliders = null;
        int mask = LayerMask.GetMask("EnemyWeapon", "EnemyBullet");
        List<Collider> colliders2 = Physics.OverlapSphere(transform.position, 3f, mask).ToList();
        // Debug.Log(transform.position);
        
        foreach (Collider collider in colliders)
        {
            if (collider == null) continue;
            
            if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon"))
            {
                if (TutorialManager != null && TutorialScript.Instance.currentObjectiveIndex == 2)
                {
                    Time.timeScale = 0f;
                    //  TutorialScript.Instance.GetComponent<DialogueTrigger>().TriggerDialogueNoQuests();
                }
                if (isDashing == true)
                {

                    StartCoroutine(SlowTimeForPerfectTimingMele());
                }
            }
        }
        foreach (Collider collider in colliders2)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
            {
                if (isDashing == true)
                {
                    StartCoroutine(SlowTimeForPerfectTiming());
                }
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon"))
            {
                if (TutorialManager != null && TutorialScript.Instance.currentObjectiveIndex == 2)
                {
                    Time.timeScale = 0f;
                    //  TutorialScript.Instance.GetComponent<DialogueTrigger>().TriggerDialogueNoQuests();
                }
                if (isDashing == true)
                {

                    StartCoroutine(SlowTimeForPerfectTimingMele());
                }
            }
        }

    }
    private IEnumerator SlowTimeForPerfectTiming()
    {
           if (TutorialScript.Instance == null && isVersus) yield break;
        isVersus = true;
        Debug.Log("perfect timing - slow start");
        Time.timeScale = 0.2f;
        isInvincible = true;
        Debug.Log("Time.timeScale set to: " + Time.timeScale);
        versusButton.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        Debug.Log("perfect timing - slow end");
        versusButton.SetActive(false);
        Time.timeScale = 1f;
        isInvincible = false;
        Debug.Log("Time.timeScale set to: " + Time.timeScale);
        isVersus = false;
    }
    private bool isVersus = false;
    private IEnumerator SlowTimeForPerfectTimingMele()
    {
        if (TutorialScript.Instance == null && isVersus) yield break;
        isVersus = true;
        if (TutorialManager != null && TutorialScript.Instance.currentObjectiveIndex == 2)
        {

            TutorialScript.Instance.SetDescription2("Teraz! Użyj dashowania żeby wykonać potężny atak kontrujący!");
        }

        Debug.Log("perfect timing - slow start");
        Time.timeScale = 0.2f;
        isInvincible = true;
        Debug.Log("Time.timeScale set to: " + Time.timeScale);
        versusButtonMele.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        Debug.Log("perfect timing - slow end");
        if (TutorialManager != null && TutorialScript.Instance.currentObjectiveIndex == 2)
        {
            Time.timeScale = 0f;
        }
        else
        {
            versusButtonMele.SetActive(false);

            Time.timeScale = 1f;

            Debug.Log("Time.timeScale set to: " + Time.timeScale);
        }
        isInvincible = false;
        isVersus = false;

    }
    public void RemoveVersus()
    {
         versusButtonMele.SetActive(false);

            Time.timeScale = 1f;
            Debug.Log("Time.timeScale set to: " + Time.timeScale);
    }
    public void LookAtClosestEnemy()
    {
        if (closestEnemyInRangePosition != Vector3.zero)
        {
            Vector3 lookDir = (closestEnemyInRangePosition - transform.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero && isCombat)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
    public void CheckForItemsInRange()
    {
        float range = maxDistanceCheck;
        bool foundInteractable = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                foundInteractable = true;
                if (gameUIHandler != null)
                {
                    gameUIHandler.EnableNotification("Press Button to interact with NPC", GameUIHandler.NotificationType.NPC);
                }
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                foundInteractable = true;
                if (gameUIHandler != null)
                {
                    gameUIHandler.EnableNotification("Press Button to pick up the " + collider.gameObject.name, GameUIHandler.NotificationType.Weapon);
                }

                if (GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered)
                {
                    SetWeapon(collider.gameObject);
                    ShowAlert();
                }
            }

            if (collider.gameObject.layer == LayerMask.NameToLayer("QuestItem"))
            {
                foundInteractable = true;
                if (gameUIHandler != null)
                {
                    gameUIHandler.EnableNotification("Press Button to pick up the quest item", GameUIHandler.NotificationType.Weapon);
                }

                if (GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered)
                {
                    // Destroy(collider.gameObject);
                    ShowAlert();
                    QuestManager.Instance.CheckQuestProgress(QuestManager.Instance.currentQuest);
                }
            }
        }

        // If no interactable found, disable notification
        if (!foundInteractable && gameUIHandler != null)
        {
            gameUIHandler.EnableNotification("", GameUIHandler.NotificationType.None);
        }
    }
    public void SetWeapon(GameObject weapon)
    {

        DestroyAndCopyWeapon(weapon);
        GameUIHandler gameUIHandler = FindObjectOfType<GameUIHandler>();
        if (gameUIHandler != null)
        {
            currentWeaponName = weapon.gameObject.name;
            gameUIHandler.UpdateWeaponImage(currentWeaponName);
            gameUIHandler.UpdateUsesCount(5);
            shotsFired = 0;
        }
    }
    public void DestroyAndCopyWeapon(GameObject collider)
    {
        Quaternion originalRotation = collider.transform.localRotation;
        Vector3 originalScale = collider.transform.localScale;
        GameObject weaponCopy = Instantiate(collider.gameObject, playerHandPos.position, playerHandPos.rotation);

        weaponCopy.transform.SetParent(playerHandPos, true);
        weaponCopy.transform.localRotation = originalRotation;
        weaponCopy.transform.localScale = originalScale;
        if(currentWeapon!=null)
            Destroy(currentWeapon);
        currentWeapon = weaponCopy;
        if(currentWeapon != null)
            Destroy(currentWeapon.GetComponent<SphereCollider>());
        if (collider.gameObject.transform.parent != null)
            Destroy(collider.gameObject.transform.parent.gameObject);
        // isCombat = !isCombat;
    }
    private void ShowAlert()
    {
        if (alertAnimator != null)
        {
            alertAnimator.SetBool("openAlertTop", true);
            CancelInvoke(nameof(CloseAlert));
            Invoke(nameof(CloseAlert), 1f);
        }
    }

    private void CloseAlert()
    {
        if (alertAnimator != null)
        {
            alertAnimator.SetBool("openAlertTop", false);
        }
    }
    private void ChangeCombat()
    {
        isCombat = !isCombat;
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(isCombat);
        }
        if (accelerationArrowPrefab != null)
        {
            accelerationArrowPrefab.SetActive(isCombat);
        }
    }
    private void ShowWeapon()
    {

        if (currentWeapon != null)
        {
            currentWeapon.SetActive(isCombat);
            if (accelerationArrowPrefab != null)
            {
                accelerationArrowPrefab.SetActive(isCombat);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
           if (other.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon"))
        {
            closestEnemyInRangePosition = other.transform.position;

            // Check for mele tag directly
            if (other.CompareTag("mele"))
            {
                var enemyAI = other.gameObject.transform.parent.GetComponent<EnemyAI>();
                if (enemyAI != null&& isInvincible == false)
                {
                  

                    currentMeleDamage = enemyAI.damageAmount;
                    health -= currentMeleDamage / 6;
                     Debug.Log("Damage taken: " + currentMeleDamage);
                 Debug.Log("Health" + health);
                    UpdateHealthBar();
                }
                

            }
        }
    }
    // if (enemiesTouching)
        // {
        //     healthTickTimer += Time.deltaTime;
            
        //     if(healthTickTimer >= 0.1f)
        //     {
        //         healthBarAnimator.SetBool("isDamaged", true);
        //         health -= currentMeleDamage / 4;
        //         Debug.Log("Damage taken: " + currentMeleDamage);
        //         Debug.Log("Health" + health);
        //         healthTickTimer = 0f;
        //         UpdateHealthBar();
        //     }
        // }
        // else
        // {
        //     healthTickTimer = 0f;
        //     healthBarAnimator.SetBool("isDamaged", false);
        // }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon"))
        {
            if (colliders.Contains(other) == false)
                colliders.Add(other);
        }
        // Healing logic
        if (other.CompareTag("Healing"))
        {
            health = Mathf.Min(health + 25, 300); // Heal by 25, max 300
            UpdateHealthBar();
            Destroy(other.gameObject);
        }
        // Damage from enemy bullets
        if (other.CompareTag("EnemyBullet"))
        {
            if (isShieldActive) return;
            int bulletDamage = 2;
            if (other.name.ToLower().Contains("magic"))
                bulletDamage = 2;
            else if (other.name.ToLower().Contains("arrow"))
                bulletDamage = 2;

            health -= bulletDamage;
            UpdateHealthBar();
            Destroy(other.gameObject);
            if (healthBarAnimator != null)
                healthBarAnimator.SetBool("isDamaged", true);
            if (health <= 0 && gameUIHandler != null)
            {
                isPlayerDead = true;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(colliders.Contains(other))
        {
            colliders.Remove(other);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyWeapon") ||
        other.gameObject.layer == LayerMask.NameToLayer("Enemy")) // Add this line
        {
            enemiesTouching = false;
            currentMeleDamage = 0;
        }
    }
    private void HandleShield()
    {
        // Cooldown timer
        if (shieldCooldownTimer > 0f)
            shieldCooldownTimer -= Time.deltaTime;

        // Activate shield if special action triggered and not on cooldown
        if (!isShieldActive && GameUIHandler.Instance.specialAction.action.triggered && shieldCooldownTimer <= 0f)
        {
            isShieldActive = true;
            shieldTimer = shieldDuration;
            shieldCooldownTimer = shieldCooldown;

            if (shieldPrefab != null)
                shieldPrefab.SetActive(true);
        }

        // While shield is active
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                isShieldActive = false;
                if (shieldPrefab != null)
                    shieldPrefab.SetActive(false);
            }
        }
    }
    private void HandleDash()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            GameUIHandler.Instance.ChangeButtonColorBasedOnTime( dashCooldownTimer,dashDuration);
        }
        if (!isDashing && GameUIHandler.Instance.specialAction.action.triggered && dashCooldownTimer <= 0f)
        {
            if (dashTrails != null)
            {
                foreach (var trail in dashTrails)
                {
                    if (trail != null)
                    {
                        trail.Clear();
                        trail.emitting = true;
                    }
                }
            }
            isDashing = true;
            dashTimer = dashDuration;
            dashDirection = moveDir.normalized != Vector3.zero ? moveDir.normalized : transform.forward;
            dashCooldownTimer = dashCooldown;
        }

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                if (dashTrails != null)
                {
                    foreach (var trail in dashTrails)
                    {
                        if (trail != null)
                            trail.emitting = false;
                    }
                }
            }
        }
    }
    private void HandleHealthPerk(PlayerPerks perk)
    {
        if (perk.perkLevel == 1)
        {
            health += 50;
        }
        else if (perk.perkLevel == 2)
        {
            health += 100;
        }
        else if (perk.perkLevel == 3)
        {
            health += 150;
        }
    }
    private void HandleSpeedPerk(PlayerPerks perk)
    {
        if (perk.perkLevel == 1)
        {
            speed += 2f;
        }
        else if (perk.perkLevel == 2)
        {
            speed += 4f;
        }
        else if (perk.perkLevel == 3)
        {
            speed += 6f;
        }

    }
     public void PlayWalkSound()
    {
        // Debug.Log("PlayWalkSound called!");
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

  
}
