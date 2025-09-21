using UnityEngine;

/// <summary>
/// Script que mantiene al fantasma dentro de los límites de su habitación
/// </summary>
public class GhostRoomBounds : MonoBehaviour
{
    [Header("Configuración de Límites")]
    [SerializeField] private bool useRoomBounds = true;
    [SerializeField] private float roomSize = 8f; // Tamaño de la habitación (asumiendo habitación cuadrada)
    [SerializeField] private float margin = 0.5f; // Margen desde los bordes
    [SerializeField] private bool showDebugBounds = true;
    
    [Header("Límites Personalizados")]
    [SerializeField] private Vector3 roomCenter = Vector3.zero;
    [SerializeField] private Vector3 roomSizeCustom = new Vector3(8f, 0f, 8f);
    
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    
    public Vector3 MinBounds => minBounds;
    public Vector3 MaxBounds => maxBounds;
    private bool initialized = false;
    
    void Start()
    {
        InitializeBounds();
    }
    
    void InitializeBounds()
    {
        if (useRoomBounds)
        {
            // Usar el SpawnRoom del EnemyCommon si está disponible
            var enemyCommon = GetComponent<EnemyCommon>();
            if (enemyCommon != null && enemyCommon.SpawnRoom != null)
            {
                // Obtener límites de la habitación desde el SpawnRoom
                CalculateBoundsFromRoom(enemyCommon.SpawnRoom);
            }
            else
            {
                // Usar límites por defecto basados en la posición inicial
                CalculateDefaultBounds();
            }
        }
        
        initialized = true;
        Debug.Log($"GhostRoomBounds: Límites inicializados - Min: {minBounds}, Max: {maxBounds}");
    }
    
    void CalculateBoundsFromRoom(GameObject room)
    {
        // Buscar el componente Room para obtener información de la habitación
        var roomComponent = room.GetComponent<Room>();
        if (roomComponent != null)
        {
            // Usar la posición de la habitación como centro
            roomCenter = room.transform.position;
            
            // Calcular límites basados en el tamaño de la habitación
            minBounds = roomCenter - roomSizeCustom / 2f + Vector3.one * margin;
            maxBounds = roomCenter + roomSizeCustom / 2f - Vector3.one * margin;
        }
        else
        {
            CalculateDefaultBounds();
        }
    }
    
    void CalculateDefaultBounds()
    {
        // Usar la posición inicial del fantasma como centro
        roomCenter = transform.position;
        roomCenter.y = 0f; // Mantener en el suelo
        
        // Calcular límites por defecto
        minBounds = roomCenter - Vector3.one * (roomSize / 2f - margin);
        maxBounds = roomCenter + Vector3.one * (roomSize / 2f - margin);
        
        // Solo limitar X y Z, no Y
        minBounds.y = transform.position.y;
        maxBounds.y = transform.position.y;
    }
    
    void Update()
    {
        if (!initialized || !useRoomBounds) return;
        
        // Verificar y corregir posición si está fuera de los límites
        Vector3 currentPos = transform.position;
        Vector3 clampedPos = currentPos;
        
        // Limitar solo X y Z (no Y)
        clampedPos.x = Mathf.Clamp(currentPos.x, minBounds.x, maxBounds.x);
        clampedPos.z = Mathf.Clamp(currentPos.z, minBounds.z, maxBounds.z);
        
        // Aplicar corrección si es necesaria
        if (clampedPos != currentPos)
        {
            transform.position = clampedPos;
            
            // Debug ocasional
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"GhostRoomBounds: {gameObject.name} corregido a posición {clampedPos}");
            }
        }
    }
    
    /// <summary>
    /// Verifica si una posición está dentro de los límites
    /// </summary>
    public bool IsPositionInBounds(Vector3 position)
    {
        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.z >= minBounds.z && position.z <= maxBounds.z;
    }
    
    /// <summary>
    /// Obtiene la posición más cercana dentro de los límites
    /// </summary>
    public Vector3 ClampPositionToBounds(Vector3 position)
    {
        Vector3 clamped = position;
        clamped.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        clamped.z = Mathf.Clamp(position.z, minBounds.z, maxBounds.z);
        return clamped;
    }
    
    /// <summary>
    /// Establece límites personalizados
    /// </summary>
    public void SetBounds(Vector3 min, Vector3 max)
    {
        minBounds = min;
        maxBounds = max;
        initialized = true;
        Debug.Log($"GhostRoomBounds: Límites actualizados - Min: {minBounds}, Max: {maxBounds}");
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugBounds) return;
        
        // Dibujar límites de la habitación
        Gizmos.color = Color.red;
        Vector3 center = (minBounds + maxBounds) / 2f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
        
        // Dibujar posición actual
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Dibujar líneas desde la posición actual hasta los límites
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, new Vector3(minBounds.x, transform.position.y, transform.position.z));
        Gizmos.DrawLine(transform.position, new Vector3(maxBounds.x, transform.position.y, transform.position.z));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, minBounds.z));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, maxBounds.z));
    }
}
