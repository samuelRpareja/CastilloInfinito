using UnityEngine;

/// <summary>
/// Script que corrige automáticamente la conexión entre joystick y movimiento
/// </summary>
public class MovementFix : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool corregirAutomaticamente = true;
    [SerializeField] private bool forzarJoystickInput = true;
    
    private void Start()
    {
        if (corregirAutomaticamente)
        {
            // Ejecutar después de un frame para asegurar que todo esté inicializado
            Invoke(nameof(CorregirMovimiento), 0.1f);
        }
    }
    
    private void CorregirMovimiento()
    {
        Debug.Log("MovementFix: Corrigiendo conexión joystick-movimiento...");
        
        // 1. Buscar componentes
        PlayerController playerController = FindObjectOfType<PlayerController>();
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        
        if (playerController == null)
        {
            Debug.LogError("MovementFix: No se encontró PlayerController");
            return;
        }
        
        if (joystick == null)
        {
            Debug.LogError("MovementFix: No se encontró VirtualJoystick");
            return;
        }
        
        // 2. Remover KeyboardPlayerInput si existe
        KeyboardPlayerInput oldInput = playerController.GetComponent<KeyboardPlayerInput>();
        if (oldInput != null)
        {
            DestroyImmediate(oldInput);
            Debug.Log("MovementFix: KeyboardPlayerInput removido");
        }
        
        // 3. Asegurar que AdaptivePlayerInput existe
        AdaptivePlayerInput adaptiveInput = playerController.GetComponent<AdaptivePlayerInput>();
        if (adaptiveInput == null)
        {
            adaptiveInput = playerController.gameObject.AddComponent<AdaptivePlayerInput>();
            Debug.Log("MovementFix: AdaptivePlayerInput agregado");
        }
        
        // 4. Asegurar que HybridInputProvider existe
        HybridInputProvider hybridProvider = playerController.GetComponent<HybridInputProvider>();
        if (hybridProvider == null)
        {
            hybridProvider = playerController.gameObject.AddComponent<HybridInputProvider>();
            Debug.Log("MovementFix: HybridInputProvider agregado");
        }
        
        // 5. Asegurar que JoystickInputProvider existe
        JoystickInputProvider joystickProvider = playerController.GetComponent<JoystickInputProvider>();
        if (joystickProvider == null)
        {
            joystickProvider = playerController.gameObject.AddComponent<JoystickInputProvider>();
            Debug.Log("MovementFix: JoystickInputProvider agregado");
        }
        
        // 6. Configurar el JoystickInputProvider con el joystick
        joystickProvider.SetVirtualJoystick(joystick);
        Debug.Log("MovementFix: JoystickInputProvider configurado con VirtualJoystick");
        
        // 7. Forzar que AdaptivePlayerInput use joystick
        if (forzarJoystickInput)
        {
            adaptiveInput.SwitchToJoystick(true);
            Debug.Log("MovementFix: AdaptivePlayerInput configurado para usar joystick");
        }
        
        // 8. Verificar que SimpleMovementController existe
        SimpleMovementController movementController = playerController.GetComponent<SimpleMovementController>();
        if (movementController == null)
        {
            Debug.LogError("MovementFix: No se encontró SimpleMovementController");
        }
        else
        {
            Debug.Log($"MovementFix: SimpleMovementController encontrado - Velocidad: {movementController.velocidadMovimiento}");
        }
        
        // 9. Verificar que AnimatorDriver existe
        AnimatorDriver animatorDriver = playerController.GetComponent<AnimatorDriver>();
        if (animatorDriver == null)
        {
            Debug.LogWarning("MovementFix: No se encontró AnimatorDriver");
        }
        else
        {
            Debug.Log("MovementFix: AnimatorDriver encontrado");
        }
        
        Debug.Log("MovementFix: Corrección completada");
    }
    
    /// <summary>
    /// Método para ejecutar la corrección manualmente
    /// </summary>
    [ContextMenu("Corregir Movimiento")]
    public void CorregirMovimientoManual()
    {
        CorregirMovimiento();
    }
    
    /// <summary>
    /// Método para verificar el estado actual
    /// </summary>
    [ContextMenu("Verificar Estado")]
    public void VerificarEstado()
    {
        Debug.Log("=== VERIFICACIÓN DEL ESTADO ===");
        
        PlayerController playerController = FindObjectOfType<PlayerController>();
        VirtualJoystick joystick = FindObjectOfType<VirtualJoystick>();
        
        if (playerController != null)
        {
            Debug.Log($"PlayerController: {playerController.name}");
            Debug.Log($"  - AdaptivePlayerInput: {playerController.GetComponent<AdaptivePlayerInput>() != null}");
            Debug.Log($"  - HybridInputProvider: {playerController.GetComponent<HybridInputProvider>() != null}");
            Debug.Log($"  - JoystickInputProvider: {playerController.GetComponent<JoystickInputProvider>() != null}");
            Debug.Log($"  - SimpleMovementController: {playerController.GetComponent<SimpleMovementController>() != null}");
            Debug.Log($"  - KeyboardPlayerInput: {playerController.GetComponent<KeyboardPlayerInput>() != null}");
        }
        
        if (joystick != null)
        {
            Debug.Log($"VirtualJoystick: {joystick.name}");
            Debug.Log($"  - Activo: {joystick.gameObject.activeInHierarchy}");
            Debug.Log($"  - Mostrar Joystick: {joystick.mostrarJoystick}");
        }
        
        Debug.Log("=== FIN DE VERIFICACIÓN ===");
    }
}
