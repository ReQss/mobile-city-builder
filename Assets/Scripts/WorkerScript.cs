using UnityEngine;
using UnityEngine.AI;

public class WorkerScript : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public Animator animator;   // Reference to the Animator component

    private Vector3 targetPosition;
    private bool isDestinationAchieved = false;
    bool isTargetBuilding = false;
    private NavMeshAgent agent;

    void Start()
    {
        // Set the initial target position to the current position
        targetPosition = transform.position;

        // Ensure the animator is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Get or add NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = moveSpeed;
        agent.stoppingDistance = 0.3f;
        agent.updateRotation = false; // We'll handle rotation for animation
    }

    void Update()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            isDestinationAchieved = true;
            animator.SetBool("working", false);
            bool isTargetSet = SetTargetPosition();
            isTargetBuilding = CheckForTargetBuilding();
            if (isTargetSet)
            {
                isDestinationAchieved = false;
                agent.SetDestination(targetPosition);
            }
        }
        // Debug.Log(Vector3.Distance(transform.position, targetPosition));

        if (agent.pathPending)
            return;

        if (Vector3.Distance(transform.position, targetPosition) > agent.stoppingDistance && isDestinationAchieved == false)
        {
            animator.SetBool("walking", true);

            // Manual rotation for animation
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else if (isTargetBuilding)
        {
            SetTargetForBuilding();
            if (Vector3.Distance(transform.position, targetPosition) > 3.5f)
            {
                Debug.Log("waking");
                agent.SetDestination(targetPosition);
                animator.SetBool("walking", true);

                // Manual rotation for animation
                Vector3 direction = (agent.steeringTarget - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
            else
            {
                isTargetBuilding = false;
                animator.SetBool("working", true);
                animator.SetBool("walking", false);
                isDestinationAchieved = true;
                agent.ResetPath();
            }
        }
        else
        {
            // Stop the walking animation
            animator.SetBool("walking", false);
            isDestinationAchieved = true;
            agent.ResetPath();
        }
    }

    bool CheckForTargetBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Building")) return true;
        }
        return false;
    }

    bool SetTargetPosition()
    {
        // Perform a raycast from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Terrain") == false) return false;
            // Set the target position to the point where the ray hit
            targetPosition = hit.point;
            return true;
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
            return false;
        }
    }

    bool SetTargetForBuilding()
    {
        // Perform a raycast from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Building") == false) return false;
            targetPosition = hit.point;
            return true;
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
            return false;
        }
    }
}
