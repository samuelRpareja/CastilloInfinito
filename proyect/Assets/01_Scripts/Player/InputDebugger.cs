using UnityEngine;

/// <summary>
/// Script de diagnóstico completo para verificar por qué no funciona el input
/// </summary>
public class InputDebugger : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool mostrarDebugEnTiempoReal = true;
    [SerializeField] private bool mostrarDebugCadaFrame = false;
    
    private PlayerController playerController;
    private IPlayerInput playerInput;
    private SimpleMovementController movementController;
    private VirtualJoystick joystick;
    
    private void Start()
    {
        DiagnosticarSistemaCompleto();
    }
    
    private void Update()
    {
        if (mostrarDebugEnTiempoReal)
        {
            DebugInputEnTiempoReal();
        }
    }
    
    private void DiagnosticarSistemaCompleto()
    {
        Debug.Log("=== DIAGNÓSTICO COMPLETO DEL SISTEMA ===");
        
        // 1. Verificar PlayerController
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Debug.Log($"✅ PlayerController encontrado: {playerController.name}");
            Debug.Log($"   - Activo: {playerController.gameObject.activeInHierarchy}");
            Debug.Log($"   - Habilitado: {playerController.enabled}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ PLAYERCONTROLLER");
            return;
        }
        
        // 2. Verificar IPlayerInput
        playerInput = playerController.GetComponent<IPlayerInput>();
        if (playerInput != null)
        {
            Debug.Log($"✅ IPlayerInput encontrado: {playerInput.GetType().Name}");
            Debug.Log($"   - Habilitado: {((MonoBehaviour)playerInput).enabled}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ COMPONENTE QUE IMPLEMENTE IPLAYERINPUT");
        }
        
        // 3. Verificar SimpleMovementController
        movementController = playerController.GetComponent<SimpleMovementController>();
        if (movementController != null)
        {
            Debug.Log($"✅ SimpleMovementController encontrado");
            Debug.Log($"   - Habilitado: {movementController.enabled}");
            Debug.Log($"   - Velocidad: {movementController.velocidadMovimiento}");
        }
        else
        {
            Debug.LogError("❌ NO SE ENCONTRÓ SIMPLEMOVEMENTCONTROLLER");
        }
        
        // 4. Verificar VirtualJoystick
        joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick != null)
        {
            Debug.Log($"✅ VirtualJoystick encontrado: {joystick.name}");
            Debug.Log($"   - Activo: {joystick.gameObject.activeInHierarchy}");
            Debug.Log($"   - Mostrar Joystick: {joystick.mostrarJoystick}");
        }
        else
        {
            Debug.LogWarning("⚠️ NO SE ENCONTRÓ VIRTUALJOYSTICK");
        }
        
        // 5. Verificar otros componentes
        AnimatorDriver animatorDriver = playerController.GetComponent<AnimatorDriver>();
        if (animatorDriver != null)
        {
            Debug.Log($"✅ AnimatorDriver encontrado");
        }
        else
        {
            Debug.LogWarning("⚠️ NO SE ENCONTRÓ ANIMATORDRIVER");
        }
        
        SimpleAttacker attacker = playerController.GetComponent<SimpleAttacker>();
        if (attacker != null)
        {
            Debug.Log($"✅ SimpleAttacker encontrado");
        }
        else
        {
            Debug.LogWarning("⚠️ NO SE ENCONTRÓ SIMPLEATTACKER");
        }
        
        Health health = playerController.GetComponent<Health>();
        if (health != null)
        {
            Debug.Log($"✅ Health encontrado");
            Debug.Log($"   - Está vivo: {health.IsAlive()}");
        }
        else
        {
            Debug.LogWarning("⚠️ NO SE ENCONTRÓ HEALTH");
        }
        
        Debug.Log("=== FIN DEL DIAGNÓSTICO ===");
    }
    
    private void DebugInputEnTiempoReal()
    {
        if (playerInput == null) return;
        
        // Obtener input actual
        float horizontal = playerInput.Horizontal;
        float vertical = playerInput.Vertical;
        bool attackPressed = playerInput.AttackPressedThisFrame;
        
        // Mostrar debug si hay input o si está configurado para mostrar cada frame
        if (mostrarDebugCadaFrame || Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f || attackPressed)
        {
            Debug.Log($"🎮 INPUT - H: {horizontal:F2}, V: {vertical:F2}, Attack: {attackPressed}");
        }
        
        // Verificar si el movimiento se está aplicando
        if (movementController != null && (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f))
        {
            Debug.Log($"🏃 MOVIMIENTO - Velocidad: {movementController.velocidadMovimiento}");
        }
    }
    
    /// <summary>
    /// Método para ejecutar diagnóstico manualmente
    /// </summary>
    [ContextMenu("Ejecutar Diagnóstico")]
    public void EjecutarDiagnostico()
    {
        DiagnosticarSistemaCompleto();
    }
    
    /// <summary>
    /// Método para probar input manualmente
    /// </summary>
    [ContextMenu("Probar Input Manual")]
    public void ProbarInputManual()
    {
        Debug.Log("=== PRUEBA MANUAL DE INPUT ===");
        
        if (playerInput == null)
        {
            Debug.LogError("❌ No hay IPlayerInput para probar");
            return;
        }
        
        // Forzar refresh del input
        playerInput.Refresh();
        
        float horizontal = playerInput.Horizontal;
        float vertical = playerInput.Vertical;
        bool attackPressed = playerInput.AttackPressedThisFrame;
        
        Debug.Log($"Input después de Refresh:");
        Debug.Log($"  - Horizontal: {horizontal}");
        Debug.Log($"  - Vertical: {vertical}");
        Debug.Log($"  - Attack: {attackPressed}");
        
        // Probar input de teclado directamente
        float keyboardH = Input.GetAxis("Horizontal");
        float keyboardV = Input.GetAxis("Vertical");
        bool keyboardAttack = Input.GetKeyDown(KeyCode.Return);
        
        Debug.Log($"Input de teclado directo:");
        Debug.Log($"  - Horizontal: {keyboardH}");
        Debug.Log($"  - Vertical: {keyboardV}");
        Debug.Log($"  - Attack: {keyboardAttack}");
        
        // Probar joystick si existe
        if (joystick != null)
        {
            float joystickH = joystick.GetHorizontal();
            float joystickV = joystick.GetVertical();
            bool joystickPressed = joystick.IsPressed();
            
            Debug.Log($"Input de joystick directo:");
            Debug.Log($"  - Horizontal: {joystickH}");
            Debug.Log($"  - Vertical: {joystickV}");
            Debug.Log($"  - Pressed: {joystickPressed}");
        }
        
        Debug.Log("=== FIN DE PRUEBA MANUAL ===");
    }
    
    /// <summary>
    /// Método para forzar movimiento de prueba
    /// </summary>
    [ContextMenu("Forzar Movimiento de Prueba")]
    public void ForzarMovimientoDePrueba()
    {
        Debug.Log("=== FORZANDO MOVIMIENTO DE PRUEBA ===");
        
        if (movementController == null)
        {
            Debug.LogError("❌ No hay SimpleMovementController para probar");
            return;
        }
        
        // Forzar movimiento directo
        movementController.Move(1f, 0f, true);
        Debug.Log("✅ Movimiento forzado aplicado (derecha)");
        
        // Esperar un frame y aplicar movimiento opuesto
        StartCoroutine(AplicarMovimientoOpuesto());
    }
    
    private System.Collections.IEnumerator AplicarMovimientoOpuesto()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (movementController != null)
        {
            movementController.Move(-1f, 0f, true);
            Debug.Log("✅ Movimiento opuesto aplicado (izquierda)");
        }
    }
}
