using UnityEngine;

/// <summary>
/// Script que configura automáticamente los límites de habitación para el fantasma
/// Se ejecuta cuando el fantasma spawnea
/// </summary>
public class GhostRoomBoundsSetup : MonoBehaviour
{
    [Header("Configuración Automática")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private float defaultRoomSize = 8f;
    [SerializeField] private float margin = 0.5f;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupRoomBounds();
        }
    }
    
    [ContextMenu("Setup Room Bounds")]
    public void SetupRoomBounds()
    {
        var enemyCommon = GetComponent<EnemyCommon>();
        var roomBounds = GetComponent<GhostRoomBounds>();
        
        if (enemyCommon == null)
        {
            Debug.LogError("GhostRoomBoundsSetup: No se encontró EnemyCommon");
            return;
        }
        
        if (roomBounds == null)
        {
            Debug.LogError("GhostRoomBoundsSetup: No se encontró GhostRoomBounds");
            return;
        }
        
        // Obtener la habitación donde spawneo el fantasma
        GameObject spawnRoom = enemyCommon.SpawnRoom;
        if (spawnRoom != null)
        {
            SetupBoundsFromRoom(spawnRoom, roomBounds);
        }
        else
        {
            SetupDefaultBounds(roomBounds);
        }
    }
    
    void SetupBoundsFromRoom(GameObject room, GhostRoomBounds roomBounds)
    {
        // Obtener la posición y tamaño de la habitación
        Vector3 roomPos = room.transform.position;
        Vector3 roomScale = room.transform.localScale;
        
        // Calcular límites basados en la escala de la habitación
        Vector3 roomSize = new Vector3(roomScale.x, 0f, roomScale.z);
        
        // Calcular límites con margen
        Vector3 minBounds = roomPos - roomSize / 2f + Vector3.one * margin;
        Vector3 maxBounds = roomPos + roomSize / 2f - Vector3.one * margin;
        
        // Mantener la altura Y del fantasma
        minBounds.y = transform.position.y;
        maxBounds.y = transform.position.y;
        
        // Aplicar límites
        roomBounds.SetBounds(minBounds, maxBounds);
        
        Debug.Log($"GhostRoomBoundsSetup: Límites configurados desde habitación {room.name}");
        Debug.Log($"  Posición habitación: {roomPos}");
        Debug.Log($"  Escala habitación: {roomScale}");
        Debug.Log($"  Límites: Min {minBounds}, Max {maxBounds}");
    }
    
    void SetupDefaultBounds(GhostRoomBounds roomBounds)
    {
        // Usar límites por defecto basados en la posición del fantasma
        Vector3 center = transform.position;
        center.y = 0f;
        
        Vector3 roomSize = Vector3.one * defaultRoomSize;
        Vector3 minBounds = center - roomSize / 2f + Vector3.one * margin;
        Vector3 maxBounds = center + roomSize / 2f - Vector3.one * margin;
        
        // Mantener la altura Y del fantasma
        minBounds.y = transform.position.y;
        maxBounds.y = transform.position.y;
        
        // Aplicar límites
        roomBounds.SetBounds(minBounds, maxBounds);
        
        Debug.Log($"GhostRoomBoundsSetup: Límites por defecto configurados");
        Debug.Log($"  Centro: {center}");
        Debug.Log($"  Límites: Min {minBounds}, Max {maxBounds}");
    }
    
    void OnDrawGizmos()
    {
        // Dibujar límites de la habitación
        var roomBounds = GetComponent<GhostRoomBounds>();
        if (roomBounds != null)
        {
            Gizmos.color = Color.red;
            Vector3 center = (roomBounds.MinBounds + roomBounds.MaxBounds) / 2f;
            Vector3 size = roomBounds.MaxBounds - roomBounds.MinBounds;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
