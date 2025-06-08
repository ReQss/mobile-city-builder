using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

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
    public float shootInterval = 1f;
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
    private bool isPlayerDead = false;



    [Header("Player Dash Settings")]
    private bool isDashing = false;
    private float dashSpeed = 20f;
    private float dashDuration = 0.15f;
    private float dashCooldown = 1f;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;


    void Start()
    {
        gameUIHandler = FindObjectOfType<GameUIHandler>();
        playerMovementInstance = this;

        controller = GetComponent<CharacterController>();

        isoRight = new Vector3(1, 0, -1).normalized;
        isoUp = new Vector3(1, 0, 1).normalized;
        health = GameManager.Instance.playerHealth; 
        speed = GameManager.Instance.playerSpeed; 
        playerAttack = GameManager.Instance.playerAttack; 
        HandleActivePerks();
        GameUIHandler.Instance.HandleStatistics();
    }

    void Update()
    {
        if(isPlayerDead)
        {
            animator.SetBool("isDead", true);
            PlayerDeathScene();
            return; // Stop processing if the player is dead
        }
        // Read movement from InputAction
        Vector2 input = GameUIHandler.Instance.moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = (isoRight * input.x + isoUp * input.y).normalized;

        float currentSpeed = isCombat ? speed / divideMovementSpeedWhenShooting : speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if ((GameUIHandler.Instance.playerAction != null && GameUIHandler.Instance.playerAction.action.triggered && currentWeapon != null)
        || (GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered && currentWeapon != null))
        {
            isCombat = !isCombat;
        }

        ShowWeapon();

        CheckForEnemiesInRange();
        if (currentWeapon != null)
        {
            string weaponName = currentWeapon.gameObject.name;
            if (weaponName.IndexOf("Sword", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                SlashingFunction();
            }
            else if (weaponName.IndexOf("Crossbow", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ShootingFunction();
            }
            else if (weaponName.IndexOf("Rod", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MagicalRod();
            }
        }
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);

            bool hasSword = false;
            bool hasCrossbow = false;
            bool hasRod = false;
            if (currentWeapon != null)
            {
                string weaponName = currentWeapon.gameObject.name;
                hasSword = weaponName.IndexOf("Sword", System.StringComparison.OrdinalIgnoreCase) >= 0;
                hasCrossbow = weaponName.IndexOf("Crossbow", System.StringComparison.OrdinalIgnoreCase) >= 0;
                hasRod = weaponName.IndexOf("Rod", System.StringComparison.OrdinalIgnoreCase) >= 0;
            }

            animator.SetBool("isShooting", isCombat && hasCrossbow);
            animator.SetBool("isSlashing", isCombat && hasSword);
            animator.SetBool("isCasting", isCombat && hasRod); // Added for Rod
        }
       
        controller.Move(velocity * Time.deltaTime);
        CheckForItemsInRange();
        NPC.anyNPCDetectsPlayer = false;

        DamageHandling();

        if (Time.time - lastEnemyTouchTime > 0.1f)
        {
            enemiesTouching = false;
        }

        HandleActiveDash();
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
        if(health <= 0)
        {
            if (gameUIHandler != null)
            {
                isPlayerDead = true;
            }
            return;
        }
        if (enemiesTouching)
        {
            healthTickTimer += Time.deltaTime;
            if (healthTickTimer >= 0.1f)
            {
                int damage = 1;
                // Check if the closest enemy has the "mele" tag
                if (closestEnemyInRangePosition != Vector3.zero)
                {
                    Collider[] colliders = Physics.OverlapSphere(closestEnemyInRangePosition, 0.1f);
                    foreach (Collider col in colliders)
                    {
                        if (col.CompareTag("mele"))
                        {
                            damage = 5;
                            break;
                        }
                    }
                }

                healthBarAnimator.SetBool("isDamaged", true);
                health -= damage;
                healthTickTimer = 0f;
                Debug.Log("Health: " + health);
                UpdateHealthBar();
            }
        }
        else
        {
            healthTickTimer = 0f;
            healthBarAnimator.SetBool("isDamaged", false);
        }
    }
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float fill = Mathf.Clamp01(health / 100f); // Assuming max health is 100
            healthBarImage.fillAmount = fill;
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
    public void ShootingFunction()
    {
        if (isCombat && currentWeapon != null && shootingProjectilePrefab != null)
        {
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval/8)
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
                    gameUIHandler.UpdateUsesCount(45 - shotsFired);
                }
                if (shotsFired >= 45)
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
    public void SlashingFunction()
    {
        if (isCombat && currentWeapon != null && shootingProjectilePrefab != null)
        {
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval)
            {
                combatTimer = 0f;

                RotateTowardsEnemy();


                shotsFired++;
                if (gameUIHandler != null)
                {
                    gameUIHandler.UpdateUsesCount(5 - shotsFired);
                }
                if (shotsFired >= 5)
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
    public void MagicalRod()
    {
        if (isCombat && currentWeapon != null && magicalProjectilePrefab != null)
        {
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval/1.5f)
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
                    gameUIHandler.UpdateUsesCount(10 - shotsFired);
                }
                if (shotsFired >= 10)
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

        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {

            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                if (gameUIHandler != null)
                {
                    gameUIHandler.EnableNotification("Press Button to pick up the " + collider.gameObject.name, GameUIHandler.NotificationType.Weapon);
                }

                if (GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered)
                {
                    DestroyAndCopyWeapon(collider);
                    GameUIHandler gameUIHandler = FindObjectOfType<GameUIHandler>();
                    if (gameUIHandler != null)
                    {
                        currentWeaponName = collider.gameObject.name;
                        gameUIHandler.UpdateWeaponImage(currentWeaponName);
                        gameUIHandler.UpdateUsesCount(5);
                        shotsFired = 0;
                    }
                    ShowAlert();
                }
            }
            if (collider.gameObject.layer == LayerMask.NameToLayer("QuestItem"))
            {
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
    }
    public void DestroyAndCopyWeapon(Collider collider)
    {
        Quaternion originalRotation = collider.transform.localRotation;
        Vector3 originalScale = collider.transform.localScale;
        GameObject weaponCopy = Instantiate(collider.gameObject, playerHandPos.position, playerHandPos.rotation);

        weaponCopy.transform.SetParent(playerHandPos, true);
        weaponCopy.transform.localRotation = originalRotation;
        weaponCopy.transform.localScale = originalScale;

        Destroy(currentWeapon);
        currentWeapon = weaponCopy;
        Destroy(currentWeapon.GetComponent<SphereCollider>());

        Destroy(collider.gameObject);
        isCombat = !isCombat;
    }
    private void ShowAlert()
    {
        if (alertAnimator != null)
        {
            alertAnimator.SetBool("openAlert", true);
            CancelInvoke(nameof(CloseAlert));
            Invoke(nameof(CloseAlert), 1f);
        }
    }

    private void CloseAlert()
    {
        if (alertAnimator != null)
        {
            alertAnimator.SetBool("openAlert", false);
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemiesTouching = true;
            lastEnemyTouchTime = Time.time;
            closestEnemyInRangePosition = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemiesTouching = false;
        }
    }

    private void HandleDash()
    {
        // Cooldown timer
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing && GameUIHandler.Instance.specialAction.action.triggered && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashDuration;
            // Dash in the direction the player is facing (forward)
            dashDirection = transform.forward;
            dashCooldownTimer = dashCooldown;
        }

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
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
}
