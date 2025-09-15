using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float joystickDeadZone = 0.1f;
    
    [Header("Components")]
    private Rigidbody rb;
    
    void Start()
    {
        // Get rigidbody component
        rb = GetComponent<Rigidbody>();
        
        // Set up rigidbody
        if (rb != null)
        {
            rb.freezeRotation = true; // Prevent physics from rotating the player
        }
    }
    
    void Update()
    {
        HandleMovement();
    }
    
    void HandleMovement()
    {
        // Get joystick/keyboard input
        float VelX = Input.GetAxis("Horizontal");
        float VelY = Input.GetAxis("Vertical");
        
        // Apply dead zone
        if (Mathf.Abs(VelX) < joystickDeadZone)
            VelX = 0f;
        if (Mathf.Abs(VelY) < joystickDeadZone)
            VelY = 0f;
        
        // Create movement vector
        Vector3 movement = new Vector3(VelX, 0, VelY) * moveSpeed;
        
        // Apply movement
        if (rb != null)
        {
            // For 3D physics movement
            Vector3 targetVelocity = new Vector3(movement.x, rb.velocity.y, movement.z);
            rb.velocity = targetVelocity;
        }
        else
        {
            // For non-physics movement
            transform.Translate(movement * Time.deltaTime);
        }
        
        // Rotate player to face movement direction
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}
