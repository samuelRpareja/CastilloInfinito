using UnityEngine;

public class PlayerJoystickInput : MonoBehaviour, IPlayerInput
{
    [Header("Configuración")]
    public float deadZone = 0.1f;
    
    private VirtualJoystick virtualJoystick;
    
    // Propiedades de IPlayerInput
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool AttackPressedThisFrame { get; private set; }
    
    private void Awake()
    {
        virtualJoystick = FindObjectOfType<VirtualJoystick>();
    }
    
    public void Refresh()
    {
        // Resetear valores
        Horizontal = 0f;
        Vertical = 0f;
        AttackPressedThisFrame = false;
        
        // Intentar usar joystick primero
        if (virtualJoystick != null)
        {
            Horizontal = virtualJoystick.GetHorizontal();
            Vertical = virtualJoystick.GetVertical();
        }
        else
        {
            // Fallback a teclado
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
        }
        
        // Aplicar dead zone
        if (Mathf.Abs(Horizontal) < deadZone) Horizontal = 0f;
        if (Mathf.Abs(Vertical) < deadZone) Vertical = 0f;
        
        // Detectar ataque
        AttackPressedThisFrame = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1");
    }
}