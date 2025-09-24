using UnityEngine;

/// <summary>
/// Input del jugador que se adapta automáticamente a la plataforma
/// Implementa IPlayerInput para integrarse con PlayerController
/// </summary>
public class AdaptivePlayerInput : MonoBehaviour, IPlayerInput
{
    [Header("Input Providers")]
    [SerializeField] private HybridInputProvider hybridInputProvider;
    [SerializeField] private KeyboardPlayerInput keyboardInput;
    
    [Header("Configuración")]
    [SerializeField] private bool usarJoystickEnAndroid = true;
    
    private IPlayerInput activeInput;
    
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool AttackPressedThisFrame { get; private set; }
    
    private void Awake()
    {
        SetupInputProviders();
    }
    
    private void SetupInputProviders()
    {
        // Buscar componentes si no están asignados
        if (hybridInputProvider == null)
        {
            hybridInputProvider = GetComponent<HybridInputProvider>();
            if (hybridInputProvider == null)
            {
                hybridInputProvider = gameObject.AddComponent<HybridInputProvider>();
            }
        }
        
        if (keyboardInput == null)
        {
            keyboardInput = GetComponent<KeyboardPlayerInput>();
            if (keyboardInput == null)
            {
                keyboardInput = gameObject.AddComponent<KeyboardPlayerInput>();
            }
        }
        
        // Determinar qué input usar
        #if UNITY_ANDROID
        if (usarJoystickEnAndroid)
        {
            activeInput = this; // Usar este componente como input activo
            Debug.Log("AdaptivePlayerInput: Configurado para usar joystick virtual en Android");
        }
        else
        {
            activeInput = keyboardInput;
            Debug.Log("AdaptivePlayerInput: Configurado para usar teclado en Android");
        }
        #else
        activeInput = keyboardInput;
        Debug.Log("AdaptivePlayerInput: Configurado para usar teclado en otras plataformas");
        #endif
    }
    
    public void Refresh()
    {
        if (activeInput == this)
        {
            // Usar input híbrido (joystick + teclado)
            RefreshHybridInput();
        }
        else
        {
            // Usar input de teclado
            activeInput.Refresh();
            Horizontal = activeInput.Horizontal;
            Vertical = activeInput.Vertical;
            AttackPressedThisFrame = activeInput.AttackPressedThisFrame;
        }
    }
    
    private void RefreshHybridInput()
    {
        if (hybridInputProvider == null)
        {
            SetupInputProviders();
            return;
        }
        
        // Obtener input de movimiento
        Vector2 movementInput = hybridInputProvider.GetMovementInput();
        Horizontal = movementInput.x;
        Vertical = movementInput.y;
        
        // Obtener input de ataque
        AttackPressedThisFrame = hybridInputProvider.GetActionInput();
    }
    
    /// <summary>
    /// Cambia dinámicamente entre joystick y teclado
    /// </summary>
    public void SwitchToJoystick(bool useJoystick)
    {
        if (useJoystick)
        {
            activeInput = this;
            if (hybridInputProvider != null)
            {
                hybridInputProvider.SwitchInputProvider(true);
            }
            Debug.Log("AdaptivePlayerInput: Cambiado a joystick virtual");
        }
        else
        {
            activeInput = keyboardInput;
            if (hybridInputProvider != null)
            {
                hybridInputProvider.SwitchInputProvider(false);
            }
            Debug.Log("AdaptivePlayerInput: Cambiado a teclado");
        }
    }
    
    /// <summary>
    /// Obtiene el tipo de input actualmente activo
    /// </summary>
    public string GetActiveInputType()
    {
        if (activeInput == this)
        {
            return "Joystick Virtual";
        }
        else
        {
            return "Teclado";
        }
    }
}
