using UnityEngine;

/// <summary>
/// Script simple que reemplaza KeyboardPlayerInput con input de joystick
/// Se agrega directamente al player y funciona inmediatamente
/// </summary>
public class SimpleJoystickInput : MonoBehaviour, IPlayerInput
{
    [Header("Configuración")]
    [SerializeField] private bool usarJoystickEnAndroid = true;
    [SerializeField] private float deadZone = 0.1f;
    
    // Propiedades de IPlayerInput
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool AttackPressedThisFrame { get; private set; }
    
    private VirtualJoystick joystick;
    
    private void Awake()
    {
        // Buscar el joystick virtual
        joystick = FindObjectOfType<VirtualJoystick>();
        
        Debug.Log($"SimpleJoystickInput: Inicializado en {name}");
        Debug.Log($"  - Plataforma: {Application.platform}");
        Debug.Log($"  - Joystick encontrado: {joystick != null}");
    }
    
    public void Refresh()
    {
        // Resetear valores
        Horizontal = 0f;
        Vertical = 0f;
        AttackPressedThisFrame = false;
        
        #if UNITY_ANDROID
        if (usarJoystickEnAndroid && joystick != null)
        {
            // Usar input del joystick
            Horizontal = joystick.GetHorizontal();
            Vertical = joystick.GetVertical();
            
            // Aplicar dead zone
            if (Mathf.Abs(Horizontal) < deadZone) Horizontal = 0f;
            if (Mathf.Abs(Vertical) < deadZone) Vertical = 0f;
            
            // Detectar ataque
            AttackPressedThisFrame = DetectAttackInput();
        }
        else
        {
            // Fallback a teclado
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            AttackPressedThisFrame = Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1");
            
            if (Mathf.Abs(Horizontal) < deadZone) Horizontal = 0f;
            if (Mathf.Abs(Vertical) < deadZone) Vertical = 0f;
        }
        #else
        // En el editor, usar teclado
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        AttackPressedThisFrame = Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1");
        
        if (Mathf.Abs(Horizontal) < deadZone) Horizontal = 0f;
        if (Mathf.Abs(Vertical) < deadZone) Vertical = 0f;
        #endif
        
        // Debug si hay input
        if (Mathf.Abs(Horizontal) > 0.01f || Mathf.Abs(Vertical) > 0.01f)
        {
            Debug.Log($"🎮 Input - H: {Horizontal:F2}, V: {Vertical:F2}");
        }
    }
    
    private bool DetectAttackInput()
    {
        #if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    // Verificar que no sea en el área del joystick
                    if (joystick != null && joystick.joystickBackground != null &&
                        !RectTransformUtility.RectangleContainsScreenPoint(
                            joystick.joystickBackground, touch.position))
                    {
                        return true;
                    }
                }
            }
        }
        #endif
        
        return false;
    }
    
    /// <summary>
    /// Configura el joystick a usar
    /// </summary>
    public void SetJoystick(VirtualJoystick newJoystick)
    {
        joystick = newJoystick;
        Debug.Log($"SimpleJoystickInput: Joystick asignado - {joystick?.name}");
    }
    
    /// <summary>
    /// Habilita o deshabilita el uso del joystick
    /// </summary>
    public void SetUseJoystick(bool useJoystick)
    {
        usarJoystickEnAndroid = useJoystick;
        Debug.Log($"SimpleJoystickInput: Usar joystick configurado a {useJoystick}");
    }
}
