using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    private Animator animator;
    
    public bool canMove = true;
    
    public float speed = 6f;
    public float runSpeed = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    public float turnSmoothTime = 0.1f;
    

    float turnSmoothVelocity;

    Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public LayerMask groundMask;
    bool isGrounded;
  

    public bool running = false; // <-- ✅ You can hook this into Animator

    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, groundMask))
        {
            Vector3 groundedPosition = new Vector3(transform.position.x, hit.point.y + controller.height / 2f, transform.position.z);
            transform.position = groundedPosition;
        }

        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        //check if player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Reset downward velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        if (!canMove) return;
            // Movement Input
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Sprint only when moving forward
            float currentSpeed = speed;
            bool isMovingForward = verticalInput > 0.1f;

            running = false; // Reset each frame

            Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            if (movementDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
            }
        
            //Debug.Log("Grounded: " + isGrounded + " | Space: " + Input.GetKeyDown(KeyCode.Space));
            Debug.DrawRay(groundCheck.position, Vector3.down * groundDistance, Color.red);
            //Debug.Log($"Y-Velocity: {velocity.y}, IsGrounded: {isGrounded}");

            //Send to Animator
            float inputMagnitude = new Vector2(horizontalInput, verticalInput).magnitude;
            animator.SetFloat("Speed", inputMagnitude, 2f, Time.deltaTime); 

            //Apply Run

            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= runSpeed; //multiply based on run multiplier
            }
            // Apply jump
            //if (Input.GetButton("Jump") && isGrounded)
            //{
            //    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //}

            // Compound gravity
            velocity.y += gravity * (isGrounded ? 1f : fallMultiplier) * Time.deltaTime;

            // Move character
            controller.Move(velocity * currentSpeed * Time.deltaTime);

            float movementSpeed = new Vector3(horizontalInput, 0f, verticalInput).magnitude;
            animator.SetFloat("Speed", movementSpeed);
            animator.SetBool("IsRunning", running);
            animator.SetBool("IsGrounded", isGrounded);


    }

}
