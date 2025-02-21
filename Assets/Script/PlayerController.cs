using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;
    public float kickForce = 10f;
    public float mouseSensitivity = 2f; // Adjust for smoother rotation

    private Rigidbody rb;
    private Animator anim;
    
    public Transform foot; // Assign in Inspector
    public LayerMask footballLayer;

    private float currentSpeed;
    private float mouseX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleKick();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveZ = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow

        Vector3 movement = transform.right * moveX + transform.forward * moveZ; // Move relative to player's direction

        // Set running speed if Shift is pressed and moving forward
        if (Input.GetKey(KeyCode.LeftShift) && moveZ > 0)
        {
            currentSpeed = runSpeed;
            anim.SetBool("isRunning", true);
        }
        else
        {
            currentSpeed = moveSpeed;
            anim.SetBool("isRunning", false);
        }

        // Reduce speed when moving left or right (1/3rd of normal speed)
        if (moveX != 0 && moveZ == 0) 
        {
            currentSpeed *= 1f / 3f;
        }

        // Handle animations based on movement
        if (moveZ > 0) // Walking Forward
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isWalkingBackward", false);
            anim.SetBool("isMovingLeft", false);
            anim.SetBool("isMovingRight", false);
        }
        else if (moveZ < 0) // Walking Backward
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isWalkingBackward", true);
            anim.SetBool("isMovingLeft", false);
            anim.SetBool("isMovingRight", false);
        }
        else if (moveX < 0) // Moving Left
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isWalkingBackward", false);
            anim.SetBool("isMovingLeft", true);
            anim.SetBool("isMovingRight", false);
        }
        else if (moveX > 0) // Moving Right
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isWalkingBackward", false);
            anim.SetBool("isMovingLeft", false);
            anim.SetBool("isMovingRight", true);
        }
        else // Idle
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isWalkingBackward", false);
            anim.SetBool("isMovingLeft", false);
            anim.SetBool("isMovingRight", false);
        }

        rb.linearVelocity = new Vector3(movement.x * currentSpeed, rb.linearVelocity.y, movement.z * currentSpeed);
    }

    void HandleMouseLook()
    {
        // Rotate player left/right using mouse
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleKick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("kick");
        }
    }

    // This function will be called from the kick animation event
    public void ThrowFootball()
    {
        Collider[] hitObjects = Physics.OverlapSphere(foot.position, 0.5f, footballLayer);
        foreach (Collider hit in hitObjects)
        {
            Rigidbody ballRb = hit.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 kickDirection = (transform.forward * 1f) + (transform.up * 0.5f); // Forward + Up
                ballRb.AddForce(kickDirection.normalized * kickForce, ForceMode.Impulse);
            }
        }
    }
}
