using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // <-- Add this for new Input System

public class WorkerScript : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public Animator animator;   // Reference to the Animator component

    public Vector3 targetPosition;
    private bool isDestinationAchieved = false;
    bool isTargetBuilding = false;
     private bool isTargetRewards = false;
    bool isTargetUpgrader = false;
    private NavMeshAgent agent;
    public GameObject currentObject;
    public UIHandler uiHandler;
    public GameObject getRewardsAlert;
    public TextMeshProUGUI getRewardsText;
    private bool rewardPanelOpened = false;
    public bool isMouseClicked = false;
    
    public bool playerDisabled = false;

    void Start()
    {
        GameManager.Instance.playerHealth = 100;
        GameManager.Instance.isUIOpen = false;
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
           if (playerDisabled == true) return;
       
        if (GameUIHandler.Instance != null && GameUIHandler.Instance.cityMoveAction != null)
            isMouseClicked = GameUIHandler.Instance.cityMoveAction.action.triggered;
        if (GameManager.Instance.isUIOpen == true)
        {
            return;
        }
       
        if (isMouseClicked) // Left mouse button
        {
            ResetRewardPanelFlag();
            isDestinationAchieved = true;
            animator.SetBool("working", false);
            bool isTargetSet = SetTargetPosition();
            isTargetRewards = CheckForTargetRwards();
            isTargetBuilding = CheckForTargetBuilding();
            if (isTargetSet)
            {
                isDestinationAchieved = false;
                agent.SetDestination(targetPosition);
            }

        }
       
         SetDestinations();
        // Debug.Log(Vector3.Distance(transform.position, targetPosition));


    }
    public void SetDestinations()
    {
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
                GameManager.Instance.isWorkerUpgrading = true;
            }
        }
        else if (isTargetRewards)
        {
            // Debug.Log("isTargetRewards");
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
                GetRewards();
                isDestinationAchieved = true;
                agent.ResetPath();
                GameManager.Instance.isWorkerUpgrading = true;
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
    public void GetRewards()
    {
        if (rewardPanelOpened) return; // zapobiega wielokrotnemu otwieraniu
        rewardPanelOpened = true;

        uiHandler.SetMoneyToCollect();
        uiHandler.OpenUIObject(getRewardsAlert);
        uiHandler.SetRewardsCost(getRewardsText);
    }
    public void ResetRewardPanelFlag()
{
    rewardPanelOpened = false;
}
    // Helper to get pointer/touch position for both desktop and mobile
    private Vector2 GetPointerPosition()
    {
        // Touch (mobile)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        // Mouse (desktop)
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        // Fallback
        return Vector2.zero;
    }

    bool CheckForTargetBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetPointerPosition());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Building")) return true;
        }
        return false;
    }
    bool CheckForTargetRwards()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetPointerPosition());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Rewards")) return true;
        }
        return false;
    }

    bool SetTargetPosition()
    {
        // Perform a raycast from the pointer/touch position
        Ray ray = Camera.main.ScreenPointToRay(GetPointerPosition());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentObject = hit.collider.gameObject;
            // Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Terrain") == false) return false;
            // Set the target position to the point where the ray hit
            targetPosition = hit.point;
            return true;
        }
        else
        {
            // Debug.Log("Raycast did not hit anything.");
            return false;
        }
    }
    public void SetTargetForCurrentBuilding()
    {
        // Perform a raycast from the mouse position

        // currentObject = gameObject;
        // Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
        // if (hit.collider.CompareTag("Terrain") == false) return false;
        // Set the target position to the point where the ray hit
        targetPosition = GameManager.Instance.currentPickedBuilding.transform.position;
        isTargetUpgrader = true;
        isDestinationAchieved = false;

    }
    bool SetTargetForBuilding()
    {
        // Perform a raycast from the pointer/touch position
        Ray ray = Camera.main.ScreenPointToRay(GetPointerPosition());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Building") == false && hit.collider.CompareTag("Rewards") == false) return false;
            targetPosition = hit.point;
            return true;
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
            return false;
        }
    }
    bool SetTargetForBuilding2(GameObject setObject)
    {
        // Perform a raycast from the pointer/touch position
        Ray ray = Camera.main.ScreenPointToRay(GetPointerPosition());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            setObject = hit.collider.gameObject;
            // Debug.Log($"Raycast hit: {setObject.name}");
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
