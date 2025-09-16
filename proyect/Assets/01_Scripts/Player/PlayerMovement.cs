using UnityEngine;

/// <summary>
/// Player Movement Controller following SOLID principles
/// Single Responsibility: Handles only player movement
/// Open/Closed: Open for extension, closed for modification
/// Liskov Substitution: Implements IMovable interface
/// Interface Segregation: Uses specific interfaces
/// Dependency Inversion: Depends on abstractions, not concretions
/// </summary>
public class PlayerMovement : MonoBehaviour, IMovable, ICameraTarget
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 250f;
    
    [Header("Components")]
    [SerializeField] private IInputProvider inputProvider;
    [SerializeField] private IAnimatorController animatorController;
    
    // Properties for ICameraTarget
    public Transform Transform => transform;
    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.forward;
    public bool IsActive => gameObject.activeInHierarchy;
    
    // Properties for IMovable
    public bool IsMoving { get; private set; }
    
    private void Awake()
    {
        // Dependency Injection - if not assigned, use default
        if (inputProvider == null)
        {
            inputProvider = GetComponent<IInputProvider>();
            if (inputProvider == null)
                inputProvider = gameObject.AddComponent<UnityInputProvider>();
        }
        
        if (animatorController == null)
        {
            animatorController = GetComponent<IAnimatorController>();
            if (animatorController == null)
                animatorController = gameObject.AddComponent<AnimatorController>();
        }
    }
    
    private void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        if (inputProvider == null) return;
        
        Vector2 input = inputProvider.GetMovementInput();
        
        // Rotate player with horizontal input
        if (Mathf.Abs(input.x) > 0.1f)
        {
            Rotate(input.x);
        }
        
        // Move player with vertical input
        if (Mathf.Abs(input.y) > 0.1f)
        {
            Move(Vector3.forward, input.y * moveSpeed);
        }
        
        // Update movement state
        IsMoving = input.magnitude > 0.1f;
        
        // Update animator
        if (animatorController != null)
        {
            animatorController.UpdateMovement(input.x, input.y, IsMoving);
        }
    }
    
    private void Rotate(float rotationInput)
    {
        transform.Rotate(0, rotationInput * Time.deltaTime * rotationSpeed, 0);
    }
    
    // IMovable implementation
    public void Move(Vector3 direction, float speed)
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.Translate(movement);
    }
    
    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }
    
    public void Stop()
    {
        IsMoving = false;
        animatorController?.UpdateMovement(0, 0, false);
    }
}
