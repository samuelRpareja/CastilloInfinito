using UnityEngine;

/// <summary>
/// Camera Controller following SOLID principles
/// Single Responsibility: Handles only camera behavior
/// Open/Closed: Open for extension, closed for modification
/// Liskov Substitution: Can be replaced with other camera implementations
/// Interface Segregation: Uses specific interfaces
/// Dependency Inversion: Depends on abstractions, not concretions
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private ICameraTarget target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private bool invertY = false;
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    
    // Dependencies
    private IInputProvider inputProvider;
    private ICameraBehavior cameraBehavior;
    
    // State
    private float mouseX = 0f;
    private float mouseY = 0f;
    
    private void Awake()
    {
        // Dependency Injection
        if (inputProvider == null)
        {
            inputProvider = GetComponent<IInputProvider>();
            if (inputProvider == null)
                inputProvider = gameObject.AddComponent<UnityInputProvider>();
        }
        
        if (cameraBehavior == null)
        {
            cameraBehavior = GetComponent<ICameraBehavior>();
            if (cameraBehavior == null)
                cameraBehavior = gameObject.AddComponent<ThirdPersonCameraBehavior>();
        }
    }
    
    private void Start()
    {
        InitializeTarget();
        InitializeRotation();
    }
    
    private void Update()
    {
        HandleMouseInput();
    }
    
    private void LateUpdate()
    {
        if (target == null || !target.IsActive) return;
        
        UpdateCameraPosition();
    }
    
    private void InitializeTarget()
    {
        if (target == null)
        {
            // Try to find player by tag first
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.GetComponent<ICameraTarget>();
                if (target == null)
                {
                    Debug.LogWarning("Player found but doesn't implement ICameraTarget interface");
                }
            }
            else
            {
                // Try to find any object with PlayerMovement component
                PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                if (playerMovement != null)
                {
                    target = playerMovement.GetComponent<ICameraTarget>();
                    if (target == null)
                    {
                        Debug.LogWarning("PlayerMovement found but doesn't implement ICameraTarget interface");
                    }
                }
                else
                {
                    Debug.LogWarning("No player found. Please assign target manually in the inspector.");
                }
            }
        }
    }
    
    private void InitializeRotation()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            mouseX = angles.y;
            mouseY = angles.x;
        }
    }
    
    private void HandleMouseInput()
    {
        if (inputProvider == null) return;
        
        Vector2 mouseInput = inputProvider.GetMouseInput();
        
        if (invertY)
            mouseInput.y = -mouseInput.y;
        
        mouseX += mouseInput.x;
        mouseY -= mouseInput.y;
        
        // Clamp vertical angle
        mouseY = Mathf.Clamp(mouseY, -maxLookAngle, maxLookAngle);
    }
    
    private void UpdateCameraPosition()
    {
        cameraBehavior?.UpdateCamera(transform, target, offset, mouseX, mouseY, followSpeed);
    }
    
    // Public methods
    public void SetTarget(ICameraTarget newTarget)
    {
        target = newTarget;
    }
    
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
