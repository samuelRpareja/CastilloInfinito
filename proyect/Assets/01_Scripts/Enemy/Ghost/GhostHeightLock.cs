using UnityEngine;

/// <summary>
/// Script que fuerza a los fantasmas a mantenerse en una altura fija
/// Se puede usar como alternativa o complemento al GhostController
/// </summary>
public class GhostHeightLock : MonoBehaviour
{
    [Header("Configuración de Altura")]
    [SerializeField] private float fixedHeight = 0f;
    [SerializeField] private bool useInitialHeight = true;
    [SerializeField] private float heightTolerance = 0.1f;
    
    private float targetHeight;
    private bool initialized = false;
    
    void Start()
    {
        InitializeHeight();
    }
    
    void InitializeHeight()
    {
        if (useInitialHeight)
        {
            targetHeight = transform.position.y;
        }
        else
        {
            targetHeight = fixedHeight;
        }
        
        initialized = true;
        Debug.Log($"GhostHeightLock: Altura objetivo establecida en {targetHeight}");
    }
    
    void Update()
    {
        if (!initialized) return;
        
        // Verificar si la altura se ha desviado
        float currentHeight = transform.position.y;
        float heightDifference = Mathf.Abs(currentHeight - targetHeight);
        
        if (heightDifference > heightTolerance)
        {
            // Forzar altura correcta
            Vector3 pos = transform.position;
            pos.y = targetHeight;
            transform.position = pos;
            
            // Debug ocasional
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"GhostHeightLock: Corrigiendo altura de {currentHeight:F2} a {targetHeight:F2}");
            }
        }
    }
    
    /// <summary>
    /// Cambiar la altura objetivo del fantasma
    /// </summary>
    public void SetTargetHeight(float newHeight)
    {
        targetHeight = newHeight;
        Debug.Log($"GhostHeightLock: Nueva altura objetivo: {targetHeight}");
    }
    
    /// <summary>
    /// Obtener la altura objetivo actual
    /// </summary>
    public float GetTargetHeight()
    {
        return targetHeight;
    }
    
    void OnDrawGizmosSelected()
    {
        if (initialized)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.position;
            pos.y = targetHeight;
            Gizmos.DrawWireCube(pos, Vector3.one * 0.5f);
            
            // Línea desde la posición actual hasta la altura objetivo
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, pos);
        }
    }
}
