using UnityEngine;

/// <summary>
/// Gestor principal del input del jugador
/// Maneja la transición entre diferentes tipos de input según la plataforma
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool usarJoystickEnAndroid = true;
    [SerializeField] private bool permitirCambioDinamico = true;
    
    [Header("Referencias")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private JoystickUISetup joystickUISetup;
    
    private AdaptivePlayerInput adaptiveInput;
    private VirtualJoystick virtualJoystick;
    
    private void Awake()
    {
        SetupInputSystem();
    }
    
    private void SetupInputSystem()
    {
        // Buscar el PlayerController si no está asignado
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }
        }
        
        // Buscar el JoystickUISetup si no está asignado
        if (joystickUISetup == null)
        {
            joystickUISetup = FindObjectOfType<JoystickUISetup>();
        }
        
        // Configurar el input adaptativo
        SetupAdaptiveInput();
        
        // Configurar el joystick virtual
        SetupVirtualJoystick();
        
        Debug.Log($"PlayerInputManager: Sistema de input configurado para {Application.platform}");
    }
    
    private void SetupAdaptiveInput()
    {
        // Remover el KeyboardPlayerInput existente si existe
        KeyboardPlayerInput oldInput = GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
        }
        
        // Agregar el AdaptivePlayerInput
        adaptiveInput = GetComponent<AdaptivePlayerInput>();
        if (adaptiveInput == null)
        {
            adaptiveInput = gameObject.AddComponent<AdaptivePlayerInput>();
        }
        
        // Configurar según la plataforma
        #if UNITY_ANDROID
        if (usarJoystickEnAndroid)
        {
            adaptiveInput.SwitchToJoystick(true);
            Debug.Log("PlayerInputManager: Configurado para usar joystick virtual en Android");
        }
        else
        {
            adaptiveInput.SwitchToJoystick(false);
            Debug.Log("PlayerInputManager: Configurado para usar teclado en Android");
        }
        #else
        adaptiveInput.SwitchToJoystick(false);
        Debug.Log("PlayerInputManager: Configurado para usar teclado en otras plataformas");
        #endif
    }
    
    private void SetupVirtualJoystick()
    {
        // Obtener referencia al joystick virtual
        if (joystickUISetup != null)
        {
            virtualJoystick = joystickUISetup.GetVirtualJoystick();
        }
        else
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
        }
        
        // Configurar el joystick para Android
        #if UNITY_ANDROID
        if (virtualJoystick != null && usarJoystickEnAndroid)
        {
            virtualJoystick.gameObject.SetActive(true);
            Debug.Log("PlayerInputManager: Joystick virtual activado para Android");
        }
        else if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(false);
            Debug.Log("PlayerInputManager: Joystick virtual desactivado");
        }
        #else
        if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(false);
            Debug.Log("PlayerInputManager: Joystick virtual desactivado para plataformas no móviles");
        }
        #endif
    }
    
    private void Update()
    {
        // Permitir cambio dinámico de input (útil para testing)
        if (permitirCambioDinamico && Input.GetKeyDown(KeyCode.J))
        {
            ToggleInputMethod();
        }
    }
    
    /// <summary>
    /// Cambia entre joystick y teclado
    /// </summary>
    public void ToggleInputMethod()
    {
        if (adaptiveInput == null) return;
        
        string currentType = adaptiveInput.GetActiveInputType();
        bool useJoystick = currentType != "Joystick Virtual";
        
        adaptiveInput.SwitchToJoystick(useJoystick);
        
        if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(useJoystick);
        }
        
        Debug.Log($"PlayerInputManager: Cambiado a {adaptiveInput.GetActiveInputType()}");
    }
    
    /// <summary>
    /// Fuerza el uso del joystick virtual
    /// </summary>
    public void ForceJoystickInput()
    {
        if (adaptiveInput != null)
        {
            adaptiveInput.SwitchToJoystick(true);
        }
        
        if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(true);
        }
        
        Debug.Log("PlayerInputManager: Forzado uso de joystick virtual");
    }
    
    /// <summary>
    /// Fuerza el uso del teclado
    /// </summary>
    public void ForceKeyboardInput()
    {
        if (adaptiveInput != null)
        {
            adaptiveInput.SwitchToJoystick(false);
        }
        
        if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(false);
        }
        
        Debug.Log("PlayerInputManager: Forzado uso de teclado");
    }
    
    /// <summary>
    /// Obtiene el tipo de input actualmente activo
    /// </summary>
    public string GetCurrentInputType()
    {
        if (adaptiveInput != null)
        {
            return adaptiveInput.GetActiveInputType();
        }
        
        return "Desconocido";
    }
    
    /// <summary>
    /// Configura si se debe usar joystick en Android
    /// </summary>
    public void SetUseJoystickInAndroid(bool useJoystick)
    {
        usarJoystickEnAndroid = useJoystick;
        
        #if UNITY_ANDROID
        if (adaptiveInput != null)
        {
            adaptiveInput.SwitchToJoystick(useJoystick);
        }
        
        if (virtualJoystick != null)
        {
            virtualJoystick.gameObject.SetActive(useJoystick);
        }
        #endif
    }
}
