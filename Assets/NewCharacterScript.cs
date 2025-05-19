using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRB : MonoBehaviour
{
    public Transform cam;
    public Transform groundCheck;
    public LayerMask groundMask;

    public float speed = 6f;
    public float jumpForce = 5f;
    public float turnSmoothTime = 0.1f;
    public float groundCheckRadius = 0.2f;

    private float turnSmoothVelocity;
    private bool isGrounded;

    private Rigidbody rb;
    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // We rotate the character manually
    }

    void Update()
    {
        // --- Movement Input ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(h, 0f, v).normalized;

        // --- Rotation ---
        if (movementInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // --- Jump ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        Debug.Log("Velocity: " + rb.linearVelocity + " | Grounded: " + isGrounded);
    }

    void FixedUpdate()
    {
        // --- Apply movement ---
        if (movementInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            Vector3 move = moveDir.normalized * speed;
            Vector3 velocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

            rb.linearVelocity = velocity;
        }
        else
        {
            // Preserve vertical velocity when idle
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
    
    // Debug ground check sphere
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
