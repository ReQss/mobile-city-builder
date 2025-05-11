using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
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

    private float combatTimer = 0f; // Timer for shooting projectiles
    public float shootInterval = 1f; // Interval between shots
    public float projectileSpeed = 35f; // Speed of the projectile
    public float divideMovementSpeedWhenShooting = 3f; // Speed reduction when shooting
    public float maxDistanceCheck = 10f; // Maximum distance to check for items

    void Start()
    {
        controller = GetComponent<CharacterController>();

        isoRight = new Vector3(1, 0, -1).normalized;
        isoUp = new Vector3(1, 0, 1).normalized;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (isoRight * horizontal + isoUp * vertical).normalized;

        // Adjust speed based on combat mode
        float currentSpeed = isCombat ? speed / divideMovementSpeedWhenShooting : speed;

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        // Obrót w kierunku ruchu
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Combat mode toggle
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCombat = !isCombat;
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(isCombat);
                accelerationArrowPrefab.SetActive(isCombat);
            }
        }

        // Shooting logic
        ShootingFunction();
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);
            animator.SetBool("isShooting", isCombat);
        }

        // // Grawitacja
        // if (controller.isGrounded && velocity.y < 0)
        // {
        //     velocity.y = -2f;
        // }

        // velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        CheckForItemsInRange();
    }
    public void ShootingFunction()
    {
        if (isCombat && currentWeapon != null && shootingProjectilePrefab != null)
        {
            combatTimer += Time.deltaTime;
            if (combatTimer >= shootInterval)
            {
                combatTimer = 0f;

                // Spawn the projectile
                GameObject projectile = Instantiate(shootingProjectilePrefab, currentWeapon.transform.position, currentWeapon.transform.rotation);

                // Add velocity to the projectile to move it forward
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = transform.forward * projectileSpeed; // Adjust speed as needed
                }
            }
        }
    }
    public void CheckForItemsInRange()
    {
        // Define the range within which to check for objects
        float range = maxDistanceCheck; // Adjust the range as needed

        // Find all colliders within the specified range
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {
            // Check if the object is on the "Weapon" layer
            if (collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                Debug.Log($"Player is in range of weapon: {collider.gameObject.name}");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Perform any action when the player is in range of a weapon
                    Debug.Log($"Picked up weapon: {collider.gameObject.name}");
                    Quaternion originalRotation = collider.transform.localRotation;
                    Vector3 originalScale = collider.transform.localScale;
                    // Create a copy of the weapon
                    GameObject weaponCopy = Instantiate(collider.gameObject, playerHandPos.position, playerHandPos.rotation);

                    // Set the weapon as a child of the player's hand
                    weaponCopy.transform.SetParent(playerHandPos, true);
                    Debug.Log(originalRotation);
                    weaponCopy.transform.localRotation = originalRotation;
                    weaponCopy.transform.localScale = originalScale;

                    // Assign the copy to the player's current weapon
                    Destroy(currentWeapon); // Destroy the previous weapon if it exists
                    currentWeapon = weaponCopy;
                    Destroy(currentWeapon.GetComponent<SphereCollider>());
                    // Optionally, disable or destroy the original weapon object
                    Destroy(collider.gameObject);
                }
            }
        }
    }
}
