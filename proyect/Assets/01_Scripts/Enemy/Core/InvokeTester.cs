using UnityEngine;

/// <summary>
/// Script para probar si el Invoke está funcionando correctamente
/// </summary>
public class InvokeTester : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float testDelay = 1f;
    
    void Start()
    {
        Debug.Log("InvokeTester: Iniciando prueba de Invoke...");
        Debug.Log($"InvokeTester: Tiempo actual: {Time.time:F2}s");
        Debug.Log($"InvokeTester: Programando Invoke para {testDelay}s");
        
        Invoke(nameof(TestInvoke), testDelay);
    }
    
    void TestInvoke()
    {
        Debug.Log($"InvokeTester: ¡INVOKE FUNCIONA! Tiempo actual: {Time.time:F2}s");
    }
    
    [ContextMenu("Test Invoke Now")]
    public void TestInvokeNow()
    {
        Debug.Log("InvokeTester: Probando Invoke manualmente...");
        Invoke(nameof(TestInvoke), testDelay);
    }
}
