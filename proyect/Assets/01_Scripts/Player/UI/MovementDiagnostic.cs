using UnityEngine;

/// <summary>
/// Script de diagnóstico para verificar por qué el joystick no mueve al jugador
/// </summary>
public class MovementDiagnostic : MonoBehaviour
{
    [Header("Diagnóstico")]
    [SerializeField] private bool mostrarDiagnostico = true;
    [SerializeField] private bool mostrarInputEnTiempoReal = true;
    
    private VirtualJoystick joystick;
    private PlayerController playerController;
    private AdaptivePlayerInput adaptiveInput;
    private HybridInputProvider hybridProvider;
    private JoystickInputProvider joystickProvider;
    private SimpleMovementController movementController;
    
    private void Start()
    {
        if (mostrarDiagnostico)
        {
            DiagnosticarMovimiento();
        }
    }
    
    private void Update()
    {
        if (mostrarInputEnTiempoReal)
        {
            MostrarInputEnTiempoReal();
        }
    }
    
    private void DiagnosticarMovimiento()
    {
        Debug.Log("=== DIAGNÓSTICO DEL MOVIMIENTO ===");
        
        // 1. Verificar VirtualJoystick
        joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            Debug.Log($"✅ VirtualJoystick encontrado: {joystick.name}");
            Debug.Log($"   - Activo: {joystick.gameObject.activeInHierarchy}");
            Debug.Log($"   - Mostrar Joystick: {joystick.mostrarJoystick}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ VIRTUALJOYSTICK");
        }
        
        // 2. Verificar PlayerController
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log($"✅ PlayerController encontrado: {playerController.name}");
            Debug.Log($"   - Activo: {playerController.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ PLAYERCONTROLLER");
        }
        
        // 3. Verificar AdaptivePlayerInput
        adaptiveInput = playerController?.GetComponent<AdaptivePlayerInput>();
        if (adaptiveInput != null)
        {
            Debug.Log($"✅ AdaptivePlayerInput encontrado");
            Debug.Log($"   - Activo: {adaptiveInput.enabled}");
            Debug.Log($"   - Tipo de input activo: {adaptiveInput.GetActiveInputType()}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ ADAPTIVEPLAYERINPUT");
        }
        
        // 4. Verificar HybridInputProvider
        hybridProvider = playerController?.GetComponent<HybridInputProvider>();
        if (hybridProvider != null)
        {
            Debug.Log($"✅ HybridInputProvider encontrado");
            Debug.Log($"   - Activo: {hybridProvider.enabled}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ HYBRIDINPUTPROVIDER");
        }
        
        // 5. Verificar JoystickInputProvider
        joystickProvider = playerController?.GetComponent<JoystickInputProvider>();
        if (joystickProvider != null)
        {
            Debug.Log($"✅ JoystickInputProvider encontrado");
            Debug.Log($"   - Activo: {joystickProvider.enabled}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ JOYSTICKINPUTPROVIDER");
        }
        
        // 6. Verificar SimpleMovementController
        movementController = playerController?.GetComponent<SimpleMovementController>();
        if (movementController != null)
        {
            Debug.Log($"✅ SimpleMovementController encontrado");
            Debug.Log($"   - Activo: {movementController.enabled}");
            Debug.Log($"   - Velocidad: {movementController.velocidadMovimiento}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ SIMPLEMOVEMENTCONTROLLER");
        }
        
        // 7. Verificar KeyboardPlayerInput (debería estar removido)
        KeyboardPlayerInput keyboardInput = playerController?.GetComponent<KeyboardPlayerInput>();
        if (keyboardInput != null)
        {
            Debug.LogWarning("⚠️ KeyboardPlayerInput aún existe, debería ser removido");
        }
        else
        {
            Debug.Log("✅ KeyboardPlayerInput correctamente removido");
        }
        
        Debug.Log("=== FIN DEL DIAGNÓSTICO ===");
    }
    
    private void MostrarInputEnTiempoReal()
    {
        if (joystick != null)
        {
            float horizontal = joystick.GetHorizontal();
            float vertical = joystick.GetVertical();
            bool isPressed = joystick.IsPressed();
            
            if (isPressed || Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                Debug.Log($"🎮 Joystick Input - H: {horizontal:F2}, V: {vertical:F2}, Pressed: {isPressed}");
            }
        }
        
        if (adaptiveInput != null)
        {
            float horizontal = adaptiveInput.Horizontal;
            float vertical = adaptiveInput.Vertical;
            
            if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                Debug.Log($"🎯 AdaptiveInput - H: {horizontal:F2}, V: {vertical:F2}");
            }
        }
    }
    
    /// <summary>
    /// Método para ejecutar diagnóstico manualmente
    /// </summary>
    [ContextMenu("Ejecutar Diagnóstico de Movimiento")]
    public void EjecutarDiagnosticoMovimiento()
    {
        DiagnosticarMovimiento();
    }
    
    /// <summary>
    /// Método para forzar la configuración correcta
    /// </summary>
    [ContextMenu("Forzar Configuración Correcta")]
    public void ForzarConfiguracionCorrecta()
    {
        Debug.Log("=== FORZANDO CONFIGURACIÓN CORRECTA ===");
        
        if (playerController == null)
        {
            Debug.LogError("❌ No se puede configurar sin PlayerController");
            return;
        }
        
        // Remover KeyboardPlayerInput si existe
        KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
            Debug.Log("✅ KeyboardPlayerInput removido");
        }
        
        // Asegurar que AdaptivePlayerInput existe
        if (adaptiveInput == null)
        {
            adaptiveInput = playerController.gameObject.AddComponent<AdaptivePlayerInput>();
            Debug.Log("✅ AdaptivePlayerInput agregado");
        }
        
        // Asegurar que HybridInputProvider existe
        if (hybridProvider == null)
        {
            hybridProvider = playerController.gameObject.AddComponent<HybridInputProvider>();
            Debug.Log("✅ HybridInputProvider agregado");
        }
        
        // Asegurar que JoystickInputProvider existe
        if (joystickProvider == null)
        {
            joystickProvider = playerController.gameObject.AddComponent<JoystickInputProvider>();
            Debug.Log("✅ JoystickInputProvider agregado");
        }
        
        // Configurar el JoystickInputProvider con el joystick
        if (joystick != null && joystickProvider != null)
        {
            joystickProvider.SetVirtualJoystick(joystick);
            Debug.Log("✅ JoystickInputProvider configurado con VirtualJoystick");
        }
        
        // Forzar que AdaptivePlayerInput use joystick
        if (adaptiveInput != null)
        {
            adaptiveInput.SwitchToJoystick(true);
            Debug.Log("✅ AdaptivePlayerInput configurado para usar joystick");
        }
        
        Debug.Log("=== CONFIGURACIÓN FORZADA COMPLETADA ===");
    }
}
