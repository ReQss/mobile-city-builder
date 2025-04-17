using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Vector3 velocity;

    private Vector3 isoRight;
    private Vector3 isoUp;

    public Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Jeśli Animator jest na tym samym obiekcie
        // animator = GetComponent<Animator>();

        // Jeśli Animator jest np. na dziecku (np. model 3D)
        // animator = GetComponentInChildren<Animator>();

        isoRight = new Vector3(1, 0, -1).normalized;
        isoUp = new Vector3(1, 0, 1).normalized;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        // float horizontal = Input.GetAxisRaw("Horizontal") + Input.GetAxis("TouchHorizontal");
        // float vertical = Input.GetAxisRaw("Vertical") + Input.GetAxis("TouchVertical");


        Vector3 moveDir = (isoRight * horizontal + isoUp * vertical).normalized;

        controller.Move(moveDir * speed * Time.deltaTime);

        // Obrót w kierunku ruchu
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Obsługa animacji
        if (animator != null)
        {
            bool isMoving = moveDir.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);
        }

        // Grawitacja
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
