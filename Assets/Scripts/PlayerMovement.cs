using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement playerMovementInstance; // Singleton instance

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
    public GameObject notificationPrefab;
    void Start()
    {
        playerMovementInstance = this; // Assign instance

        controller = GetComponent<CharacterController>();

        isoRight = new Vector3(1, 0, -1).normalized;
        isoUp = new Vector3(1, 0, 1).normalized;
    }

    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (isoRight * horizontal + isoUp * vertical).normalized;

        float currentSpeed = isCombat ? speed / divideMovementSpeedWhenShooting : speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCombat = !isCombat;

        }
        ShowWeapon();

        CheckForEnemiesInRange();
        ShootingFunction();
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);
            animator.SetBool("isShooting", isCombat);
        }

        controller.Move(velocity * Time.deltaTime);
        CheckForItemsInRange();
        EnableNotification();

        // Reset the flag for the next frame
        NPC.anyNPCDetectsPlayer = false;
    }
    public void EnableNotification()
    {
        if (NPC.anyNPCDetectsPlayer)
        {
            if (notificationPrefab != null)
            {
                notificationPrefab.SetActive(true);
            }
        }
        else
        {
            if (notificationPrefab != null)
            {
                notificationPrefab.SetActive(false);
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

                // Face the player toward the enemy before shooting
                if (closestEnemyInRangePosition != Vector3.zero)
                {
                    Vector3 lookDir = (closestEnemyInRangePosition - transform.position).normalized;
                    lookDir.y = 0f;
                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(lookDir);
                    }
                }

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
            }
        }
    }
    public void CheckForEnemiesInRange()
    {
        float range = enemyRange;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        bool enemyFound = false;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Debug.Log($"Player is in range of enemy: {collider.gameObject.name}");
                closestEnemyInRangePosition = collider.transform.position;
                enemyFound = true;

                EnemyAI enemyAI = collider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.EnemyCanvasLockOnIsEnabled = true;
                }
                break;
            }
        }

        if (!enemyFound)
        {
            closestEnemyInRangePosition = Vector3.zero;

            // Set all enemies' EnemyCanvasLockOnIsEnabled to false
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
    public void CheckForItemsInRange()
    {
        float range = maxDistanceCheck;

        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {

            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                // Debug.Log($"Player is in range of weapon: {collider.gameObject.name}");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DestroyAndCopyWeapon(collider);
                    ShowAlert();
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
        Debug.Log(originalRotation);
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
    private void ShowWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(isCombat);
            accelerationArrowPrefab.SetActive(isCombat);
        }
    }
}
