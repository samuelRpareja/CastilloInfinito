using UnityEngine;

/// <summary>
/// Unity Input System implementation
/// Single Responsibility Principle (SRP)
/// </summary>
public class UnityInputProvider : MonoBehaviour, IInputProvider
{
    [Header("Input Settings")]
    [SerializeField] private float deadZone = 0.1f;
    [SerializeField] private float mouseSensitivity = 2f;
    
    public Vector2 GetMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Apply dead zone
        if (Mathf.Abs(horizontal) < deadZone) horizontal = 0f;
        if (Mathf.Abs(vertical) < deadZone) vertical = 0f;
        
        return new Vector2(horizontal, vertical);
    }
    
    public Vector2 GetMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        return new Vector2(mouseX, mouseY);
    }
    
    public bool GetActionInput()
    {
        return Input.GetButtonDown("Fire1");
    }
}
