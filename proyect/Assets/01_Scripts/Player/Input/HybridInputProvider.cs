using UnityEngine;

/// <summary>
/// Input provider híbrido que combina teclado y joystick virtual
/// Automáticamente detecta la plataforma y usa el input apropiado
/// </summary>
public class HybridInputProvider : MonoBehaviour, IInputProvider
{
    [Header("Input Providers")]
    [SerializeField] private UnityInputProvider keyboardInput;
    [SerializeField] private JoystickInputProvider joystickInput;
    
    [Header("Configuración")]
    [SerializeField] private bool priorizarJoystickEnAndroid = true;
    [SerializeField] private float deadZone = 0.1f;
    
    private IInputProvider activeInputProvider;
    
    private void Awake()
    {
        SetupInputProviders();
    }
    
    private void SetupInputProviders()
    {
        // Buscar componentes si no están asignados
        if (keyboardInput == null)
        {
            keyboardInput = GetComponent<UnityInputProvider>();
            if (keyboardInput == null)
            {
                keyboardInput = gameObject.AddComponent<UnityInputProvider>();
            }
        }
        
        if (joystickInput == null)
        {
            joystickInput = GetComponent<JoystickInputProvider>();
            if (joystickInput == null)
            {
                joystickInput = gameObject.AddComponent<JoystickInputProvider>();
            }
        }
        
        // Determinar qué input provider usar
        #if UNITY_ANDROID
        if (priorizarJoystickEnAndroid)
        {
            activeInputProvider = joystickInput;
            Debug.Log("HybridInputProvider: Usando joystick virtual para Android");
        }
        else
        {
            activeInputProvider = keyboardInput;
            Debug.Log("HybridInputProvider: Usando teclado para Android");
        }
        #else
        activeInputProvider = keyboardInput;
        Debug.Log("HybridInputProvider: Usando teclado para otras plataformas");
        #endif
    }
    
    public Vector2 GetMovementInput()
    {
        if (activeInputProvider == null)
        {
            SetupInputProviders();
        }
        
        Vector2 input = activeInputProvider.GetMovementInput();
        
        // Aplicar dead zone adicional si es necesario
        if (Mathf.Abs(input.x) < deadZone) input.x = 0f;
        if (Mathf.Abs(input.y) < deadZone) input.y = 0f;
        
        return input;
    }
    
    public Vector2 GetMouseInput()
    {
        if (activeInputProvider == null)
        {
            SetupInputProviders();
        }
        
        return activeInputProvider.GetMouseInput();
    }
    
    public bool GetActionInput()
    {
        if (activeInputProvider == null)
        {
            SetupInputProviders();
        }
        
        return activeInputProvider.GetActionInput();
    }
    
    /// <summary>
    /// Cambia dinámicamente el input provider activo
    /// </summary>
    public void SwitchInputProvider(bool useJoystick)
    {
        if (useJoystick && joystickInput != null)
        {
            activeInputProvider = joystickInput;
            Debug.Log("HybridInputProvider: Cambiado a joystick virtual");
        }
        else if (keyboardInput != null)
        {
            activeInputProvider = keyboardInput;
            Debug.Log("HybridInputProvider: Cambiado a teclado");
        }
    }
    
    /// <summary>
    /// Obtiene el input provider actualmente activo
    /// </summary>
    public IInputProvider GetActiveInputProvider()
    {
        return activeInputProvider;
    }
}
