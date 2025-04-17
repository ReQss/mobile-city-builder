using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public float speed = 6f;  // Prędkość poruszania się postaci
    public float gravity = -9.81f;  // Siła grawitacji
    public float jumpHeight = 3f;  // Wysokość skoku
    private float jumpTime;
    private bool isJumping = false;
    Vector3 velocity;
    public bool isGrounded;

    // public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    private float playerVelocity = 0.0f;
    public float acceleration = 0.1f;
    public float decceleration = 0.5f;

    public CharacterController controller;
    public Transform cam;

    void Update()
    {
        // Sprawdzanie, czy postać dotyka ziemi
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !isJumping)
        {
            velocity.y = -2f;  // Minimalny efekt grawitacji, gdy postać jest na ziemi
        }

        if (Input.GetButtonDown("Jump") && isGrounded)  // Skakanie
        {
            jumpTime = 0f;
            isJumping = true;
        }

        if (isJumping)
        {
            jumpTime += Time.deltaTime;
            velocity.y = Mathf.Lerp(0, jumpHeight, jumpTime);  // Płynna animacja skoku

            if (jumpTime >= 1f)  // Kończenie skoku
            {
                isJumping = false;
                velocity.y = -2f;  // Wróć do ziemi po skoku
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;  // Efekt grawitacji, gdy postać nie skacze
        }
        controller.Move(velocity * Time.deltaTime);

        // Ruch postaci
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Obliczanie kąta obrotu postaci
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Ruch w kierunku obrotu postaci
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            playerVelocity = Mathf.Clamp(playerVelocity + Time.deltaTime * acceleration, 0f, 1f);
            controller.Move(moveDir.normalized * speed * Time.deltaTime * playerVelocity);
        }
        else if (playerVelocity > 0)
        {
            playerVelocity = Mathf.Clamp(playerVelocity - Time.deltaTime * decceleration, 0f, 1f);
        }
    }
}
