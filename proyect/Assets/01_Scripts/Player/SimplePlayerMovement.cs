using UnityEngine;

/// <summary>
/// Simple Player Movement - Fallback version without SOLID complexity
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private float joystickDeadZone = 0.1f;
    
    [Header("Components")]
    private Animator animator;
    private Rigidbody rb;
    
    void Start()
    {
        // Get components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // Set up rigidbody for physics
        if (rb != null)
        {
            rb.freezeRotation = true; // Prevent physics from rotating the player
            rb.drag = 5f; // Add some drag to stop sliding
        }
    }
    
    void Update()
    {
        // Get joystick/keyboard input
        float VelX = Input.GetAxis("Horizontal");
        float VelY = Input.GetAxis("Vertical");
        
        // Apply dead zone
        if (Mathf.Abs(VelX) < joystickDeadZone)
            VelX = 0f;
        if (Mathf.Abs(VelY) < joystickDeadZone)
            VelY = 0f;
        
        // Rotate player with horizontal input
        if (Mathf.Abs(VelX) > 0.1f)
        {
            transform.Rotate(0, VelX * Time.deltaTime * rotationSpeed, 0);
        }
        
        // Update animator
        if (animator != null)
        {
            animator.SetFloat("VelX", VelX);
            animator.SetFloat("VelY", VelY);
        }
    }
    
    void FixedUpdate()
    {
        // Get joystick/keyboard input for physics
        float VelX = Input.GetAxis("Horizontal");
        float VelY = Input.GetAxis("Vertical");
        
        // Apply dead zone
        if (Mathf.Abs(VelX) < joystickDeadZone)
            VelX = 0f;
        if (Mathf.Abs(VelY) < joystickDeadZone)
            VelY = 0f;
        
        // Move player with physics
        if (rb != null)
        {
            Vector3 movement = new Vector3(0, 0, VelY) * moveSpeed;
            Vector3 targetVelocity = transform.TransformDirection(movement);
            
            // Apply movement while preserving Y velocity for gravity
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        }
    }
}
