using UnityEngine;

public class WorkerScript : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public Animator animator;   // Reference to the Animator component

    private Vector3 targetPosition;
    private bool isDestinationAchieved = false;
    bool isTargetBuilding = false;

    void Start()
    {
        // Set the initial target position to the current position
        targetPosition = transform.position;

        // Ensure the animator is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
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
            // Debug.Log(isTargetBuilding);
            if (isTargetSet)
                isDestinationAchieved = false;
        }
        Debug.Log(Vector3.Distance(transform.position, targetPosition));
        if (Vector3.Distance(transform.position, targetPosition) > 0.3f && isDestinationAchieved == false)
        {

            MoveWorkerTowardsDestination();
        }
        else if (isTargetBuilding)
        {
            SetTargetForBuilding();
            if (Vector3.Distance(transform.position, targetPosition) > 3.5f)
            {
                Debug.Log("waking");
                MoveWorkerTowardsDestination();
            }
            else
            {
                isTargetBuilding = false;
                animator.SetBool("working", true);
            }
        }
        else
        {
            // Stop the walking animation
            animator.SetBool("walking", false);
            isDestinationAchieved = true;
        }

    }
    void MoveWorkerTowardsDestination()
    {
        animator.SetBool("walking", true);

        // Adjust the target position to match the worker's current Y position
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        // Rotate towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero) // Ensure direction is valid
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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
            // Log the name of the object hit by the raycast
            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Terrain") == false) return false;
            // Set the target position to the point where the ray hit
            targetPosition = hit.point;
            return true;
        }
        else
        {
            // Log if the raycast doesn't hit anything
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
            // Log the name of the object hit by the raycast
            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Building") == false) return false;
            // Set the target position to the point where the ray hit
            targetPosition = hit.point;
            return true;
        }
        else
        {
            // Log if the raycast doesn't hit anything
            Debug.Log("Raycast did not hit anything.");
            return false;
        }
    }
}
