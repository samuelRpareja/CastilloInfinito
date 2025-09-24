using UnityEngine;

/// <summary>
/// Input provider que usa el joystick virtual para Android
/// Implementa IInputProvider para integrarse con el sistema existente
/// </summary>
public class JoystickInputProvider : MonoBehaviour, IInputProvider
{
    [Header("Joystick Input")]
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private bool usarJoystickEnAndroid = true;
    
    [Header("Configuración")]
    [SerializeField] private float deadZone = 0.1f;
    
    private void Awake()
    {
        // Buscar el joystick virtual si no está asignado
        if (virtualJoystick == null)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
        }
        
        #if !UNITY_ANDROID
        // En plataformas que no son Android, desactivar este componente
        if (usarJoystickEnAndroid)
        {
            enabled = false;
        }
        #endif
    }
    
    public Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;
        
        #if UNITY_ANDROID
        if (virtualJoystick != null && usarJoystickEnAndroid)
        {
            // Usar input del joystick virtual
            input.x = virtualJoystick.GetHorizontal();
            input.y = virtualJoystick.GetVertical();
        }
        else
        {
            // Fallback a input de teclado para testing en editor
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        }
        #else
        // En otras plataformas, usar input estándar
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        #endif
        
        // Aplicar dead zone
        if (Mathf.Abs(input.x) < deadZone) input.x = 0f;
        if (Mathf.Abs(input.y) < deadZone) input.y = 0f;
        
        return input;
    }
    
    public Vector2 GetMouseInput()
    {
        // El joystick no maneja mouse input
        return Vector2.zero;
    }
    
    public bool GetActionInput()
    {
        #if UNITY_ANDROID
        // En Android, detectar toques en la pantalla para ataque
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    // Verificar que no sea en el área del joystick
                    if (virtualJoystick != null && 
                        !RectTransformUtility.RectangleContainsScreenPoint(
                            virtualJoystick.joystickBackground, touch.position))
                    {
                        return true;
                    }
                }
            }
        }
        #endif
        
        // Fallback para otras plataformas
        return Input.GetButtonDown("Fire1");
    }
    
    /// <summary>
    /// Configura el joystick virtual a usar
    /// </summary>
    public void SetVirtualJoystick(VirtualJoystick joystick)
    {
        virtualJoystick = joystick;
    }
    
    /// <summary>
    /// Habilita o deshabilita el uso del joystick
    /// </summary>
    public void SetUseJoystick(bool useJoystick)
    {
        usarJoystickEnAndroid = useJoystick;
    }
}
