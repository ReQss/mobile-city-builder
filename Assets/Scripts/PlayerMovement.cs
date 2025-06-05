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
    public float speed = 5f;
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
    private int shotsFired = 0; 
    public int health = 100;
    private bool enemiesTouching = false;
    private float healthTickTimer = 0f;
    public float enemyRangeDamage = 1f;
    private float lastEnemyTouchTime = -1f;
    public Image healthBarImage; 
    public Animator healthBarAnimator;
    
    

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

        
    }

    void Update()
    {
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
        ||(GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered && currentWeapon != null))
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
        }
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);

            bool hasSword = false;
            bool hasCrossbow = false;
            if (currentWeapon != null)
            {
                string weaponName = currentWeapon.gameObject.name;
                hasSword = weaponName.IndexOf("Sword", System.StringComparison.OrdinalIgnoreCase) >= 0;
                hasCrossbow = weaponName.IndexOf("Crossbow", System.StringComparison.OrdinalIgnoreCase) >= 0;
            }

            animator.SetBool("isShooting", isCombat && hasCrossbow);
            animator.SetBool("isSlashing", isCombat && hasSword);
        }
        controller.Move(velocity * Time.deltaTime);
        CheckForItemsInRange();
        NPC.anyNPCDetectsPlayer = false;

        DamageHandling();

        if (Time.time - lastEnemyTouchTime > 0.1f)
        {
            enemiesTouching = false;
        }
        
        HandleActivePerks();
    }
    public void HandleActivePerks()
    {
        foreach (PlayerPerks perk in GameManager.Instance.playerPerks)
        {
            if (perk.perkIsActive)
            {
                if (perk.perkName == "Dash")
                {
                    HandleDash();
                }
            }
        }
    }
    private void DamageHandling()
    {
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
            if (combatTimer >= shootInterval)
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
    public void CheckForEnemiesInRange()
    {
        float range = enemyRange;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        Collider closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // Find the closest enemy
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

            // Enable lock-on for the closest enemy
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

            // Disable lock-on for all enemies in extended range
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
                    gameUIHandler.EnableNotification("Press E to pick up the " + collider.gameObject.name, GameUIHandler.NotificationType.Weapon);
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
                    gameUIHandler.EnableNotification("Press E to pick up the quest item", GameUIHandler.NotificationType.Weapon);
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
}
