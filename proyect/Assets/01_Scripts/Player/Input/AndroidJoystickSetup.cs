using UnityEngine;

/// <summary>
/// Configurador automático del sistema de joystick para Android
/// Se puede agregar a cualquier GameObject en la escena para configurar automáticamente todo el sistema
/// </summary>
public class AndroidJoystickSetup : MonoBehaviour
{
    [Header("Configuración Automática")]
    [SerializeField] private bool configurarAutomaticamente = true;
    [SerializeField] private bool crearJoystickUI = true;
    [SerializeField] private bool reemplazarInputExistente = true;
    
    [Header("Configuración del Joystick")]
    [SerializeField] private Vector2 posicionJoystick = new Vector2(-200, -200);
    [SerializeField] private float tamanoJoystick = 150f;
    
    [Header("Referencias (Opcional)")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Canvas targetCanvas;
    
    private void Start()
    {
        if (configurarAutomaticamente)
        {
            ConfigurarSistemaCompleto();
        }
    }
    
    /// <summary>
    /// Configura todo el sistema de joystick automáticamente
    /// </summary>
    [ContextMenu("Configurar Sistema Completo")]
    public void ConfigurarSistemaCompleto()
    {
        Debug.Log("AndroidJoystickSetup: Iniciando configuración automática del sistema de joystick...");
        
        // 1. Configurar el PlayerController
        ConfigurarPlayerController();
        
        // 2. Crear la UI del joystick si es necesario
        if (crearJoystickUI)
        {
            ConfigurarJoystickUI();
        }
        
        // 3. Configurar el sistema de input
        ConfigurarSistemaInput();
        
        Debug.Log("AndroidJoystickSetup: Configuración completada exitosamente");
    }
    
    private void ConfigurarPlayerController()
    {
        // Buscar el PlayerController si no está asignado
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        if (playerController == null)
        {
            Debug.LogWarning("AndroidJoystickSetup: No se encontró PlayerController en la escena");
            return;
        }
        
        // Agregar el PlayerInputManager si no existe
        PlayerInputManager inputManager = playerController.GetComponent<PlayerInputManager>();
        if (inputManager == null)
        {
            inputManager = playerController.gameObject.AddComponent<PlayerInputManager>();
            Debug.Log("AndroidJoystickSetup: PlayerInputManager agregado al PlayerController");
        }
        
        Debug.Log("AndroidJoystickSetup: PlayerController configurado correctamente");
    }
    
    private void ConfigurarJoystickUI()
    {
        // Buscar el Canvas si no está asignado
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
        }
        
        if (targetCanvas == null)
        {
            Debug.LogWarning("AndroidJoystickSetup: No se encontró Canvas en la escena");
            return;
        }
        
        // Verificar si ya existe un JoystickUISetup
        JoystickUISetup existingSetup = FindObjectOfType<JoystickUISetup>();
        if (existingSetup != null)
        {
            Debug.Log("AndroidJoystickSetup: Ya existe un JoystickUISetup en la escena");
            return;
        }
        
        // Crear el JoystickUISetup
        GameObject setupObject = new GameObject("JoystickUISetup");
        setupObject.transform.SetParent(targetCanvas.transform, false);
        
        JoystickUISetup joystickSetup = setupObject.AddComponent<JoystickUISetup>();
        
        // Configurar la posición y tamaño
        joystickSetup.SetJoystickPosition(posicionJoystick);
        joystickSetup.SetJoystickSize(tamanoJoystick);
        
        Debug.Log("AndroidJoystickSetup: UI del joystick creada y configurada");
    }
    
    private void ConfigurarSistemaInput()
    {
        if (playerController == null) return;
        
        // Verificar si ya existe un AdaptivePlayerInput
        AdaptivePlayerInput existingInput = playerController.GetComponent<AdaptivePlayerInput>();
        if (existingInput != null)
        {
            Debug.Log("AndroidJoystickSetup: Ya existe AdaptivePlayerInput en el PlayerController");
            return;
        }
        
        // Agregar los componentes necesarios
        if (reemplazarInputExistente)
        {
            // Remover KeyboardPlayerInput si existe
            KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
            if (oldInput != null)
            {
                DestroyImmediate(oldInput);
                Debug.Log("AndroidJoystickSetup: KeyboardPlayerInput removido");
            }
        }
        
        // Agregar AdaptivePlayerInput
        AdaptivePlayerInput adaptiveInput = playerController.gameObject.AddComponent<AdaptivePlayerInput>();
        
        // Agregar HybridInputProvider
        HybridInputProvider hybridProvider = playerController.gameObject.AddComponent<HybridInputProvider>();
        
        // Agregar JoystickInputProvider
        JoystickInputProvider joystickProvider = playerController.gameObject.AddComponent<JoystickInputProvider>();
        
        Debug.Log("AndroidJoystickSetup: Sistema de input configurado correctamente");
    }
    
    /// <summary>
    /// Limpia el sistema de joystick (útil para testing)
    /// </summary>
    [ContextMenu("Limpiar Sistema")]
    public void LimpiarSistema()
    {
        Debug.Log("AndroidJoystickSetup: Limpiando sistema de joystick...");
        
        // Remover componentes del PlayerController
        if (playerController != null)
        {
            PlayerInputManager inputManager = playerController.GetComponent<PlayerInputManager>();
            if (inputManager != null) DestroyImmediate(inputManager);
            
            AdaptivePlayerInput adaptiveInput = playerController.GetComponent<AdaptivePlayerInput>();
            if (adaptiveInput != null) DestroyImmediate(adaptiveInput);
            
            HybridInputProvider hybridProvider = playerController.GetComponent<HybridInputProvider>();
            if (hybridProvider != null) DestroyImmediate(hybridProvider);
            
            JoystickInputProvider joystickProvider = playerController.GetComponent<JoystickInputProvider>();
            if (joystickProvider != null) DestroyImmediate(joystickProvider);
        }
        
        // Remover UI del joystick
        JoystickUISetup joystickSetup = FindObjectOfType<JoystickUISetup>();
        if (joystickSetup != null) DestroyImmediate(joystickSetup.gameObject);
        
        VirtualJoystick virtualJoystick = FindObjectOfType<VirtualJoystick>();
        if (virtualJoystick != null) DestroyImmediate(virtualJoystick.gameObject);
        
        Debug.Log("AndroidJoystickSetup: Sistema limpiado");
    }
    
    /// <summary>
    /// Configura la posición del joystick
    /// </summary>
    public void SetJoystickPosition(Vector2 position)
    {
        posicionJoystick = position;
        
        JoystickUISetup setup = FindObjectOfType<JoystickUISetup>();
        if (setup != null)
        {
            setup.SetJoystickPosition(position);
        }
    }
    
    /// <summary>
    /// Configura el tamaño del joystick
    /// </summary>
    public void SetJoystickSize(float size)
    {
        tamanoJoystick = size;
        
        JoystickUISetup setup = FindObjectOfType<JoystickUISetup>();
        if (setup != null)
        {
            setup.SetJoystickSize(size);
        }
    }
}
