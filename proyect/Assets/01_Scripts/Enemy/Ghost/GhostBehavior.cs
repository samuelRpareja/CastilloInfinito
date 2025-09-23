using UnityEngine;

/// <summary>
/// Script unificado que maneja todo el comportamiento del fantasma
/// Reemplaza a GhostController, GhostHeightLock, GhostRoomBounds y GhostRoomBoundsSetup
/// </summary>
public class GhostBehavior : MonoBehaviour, IInitializable
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float aggroRange = 12f;
    [SerializeField] private float disengageRange = 15f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float accel = 8f;
    [SerializeField] private float decel = 12f;
    [SerializeField] private float turnSpeed = 12f;
    
    [Header("Configuración de Altura")]
    [SerializeField] private bool useInitialHeight = true;
    [SerializeField] private float fixedHeight = 0f;
    [SerializeField] private float heightTolerance = 0.1f;
    
    [Header("Configuración de Límites")]
    [SerializeField] private bool useRoomBounds = true;
    [SerializeField] private float roomMargin = 0.5f;
    [SerializeField] private bool showDebugBounds = true;
    
    // Componentes
    private EnemyCommon enemy;
    private IEnemyAttack attack;
    private GhostPhase phase;
    private CharacterController characterController;
    
    // Estado
    private bool initialized = false;
    private float targetHeight;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private bool boundsInitialized = false;
    
    void Start() 
    { 
        if (!initialized) Initialize(); 
    }
    
    public void Initialize()
    {
        Debug.Log($"GhostBehavior: Inicializando {name}");
        
        // Obtener componentes
        enemy = GetComponent<EnemyCommon>();
        attack = GetComponent<IEnemyAttack>();
        phase = GetComponent<GhostPhase>();
        characterController = GetComponent<CharacterController>();
        
        if (enemy == null) 
        { 
            Debug.LogError("GhostBehavior: No se encontró EnemyCommon"); 
            return; 
        }
        
        // Configurar altura
        if (useInitialHeight)
        {
            targetHeight = transform.position.y;
        }
        else
        {
            targetHeight = fixedHeight;
        }
        
        // Configurar movimiento
        enemy.movementMode = MovementMode.Flying;
        enemy.moveSpeed = moveSpeed;
        enemy.accel = accel;
        enemy.decel = decel;
        enemy.turnSpeed = turnSpeed;
        enemy.verticalSpeedLimit = 0f;
        enemy.verticalAccel = 0f;
        
        // Configurar CharacterController
        if (characterController != null)
        {
            characterController.slopeLimit = 90f;
            characterController.stepOffset = 0f;
            characterController.minMoveDistance = 0f;
            characterController.skinWidth = Mathf.Max(0.02f, characterController.skinWidth);
        }
        
        // Inicializar límites de habitación
        InitializeRoomBounds();
        
        // Inicializar EnemyCommon
        enemy.Initialize();
        enemy.fsm.Set(new IdleStateDebug(enemy, aggroRange));
        enemy.OnDeath += () => enemy.fsm.Set(new DeadState(enemy));
        
        initialized = true;
        Debug.Log($"GhostBehavior: Inicialización completada para {name}");
    }
    
    void InitializeRoomBounds()
    {
        if (!useRoomBounds) return;
        
        // Obtener la habitación donde spawneo el fantasma
        GameObject spawnRoom = enemy?.SpawnRoom;
        if (spawnRoom != null)
        {
            SetupBoundsFromRoom(spawnRoom);
        }
        else
        {
            SetupDefaultBounds();
        }
        
        boundsInitialized = true;
        Debug.Log($"GhostBehavior: Límites inicializados - Min: {minBounds}, Max: {maxBounds}");
    }
    
    void SetupBoundsFromRoom(GameObject room)
    {
        Vector3 roomPos = room.transform.position;
        Vector3 roomScale = room.transform.localScale;
        
        // Calcular límites basados en la escala de la habitación
        Vector3 roomSize = new Vector3(roomScale.x, 0f, roomScale.z);
        
        // Calcular límites con margen
        minBounds = roomPos - roomSize / 2f + Vector3.one * roomMargin;
        maxBounds = roomPos + roomSize / 2f - Vector3.one * roomMargin;
        
        // Mantener la altura Y del fantasma
        minBounds.y = targetHeight;
        maxBounds.y = targetHeight;
        
        Debug.Log($"GhostBehavior: Límites configurados desde habitación {room.name}");
        Debug.Log($"  Posición habitación: {roomPos}");
        Debug.Log($"  Escala habitación: {roomScale}");
        Debug.Log($"  Límites: Min {minBounds}, Max {maxBounds}");
    }
    
    void SetupDefaultBounds()
    {
        // Usar límites por defecto basados en la posición del fantasma
        Vector3 center = transform.position;
        center.y = 0f;
        
        Vector3 roomSize = Vector3.one * 8f; // Tamaño por defecto
        minBounds = center - roomSize / 2f + Vector3.one * roomMargin;
        maxBounds = center + roomSize / 2f - Vector3.one * roomMargin;
        
        // Mantener la altura Y del fantasma
        minBounds.y = targetHeight;
        maxBounds.y = targetHeight;
        
        Debug.Log($"GhostBehavior: Límites por defecto configurados");
        Debug.Log($"  Centro: {center}");
        Debug.Log($"  Límites: Min {minBounds}, Max {maxBounds}");
    }
    
    void Update()
    {
        if (!initialized || enemy == null || enemy.IsDead) return;
        
        // Manejar phase aleatorio
        if (phase != null && phase.CanPhase && Random.value < 0.01f)
            phase.DoPhase();
        
        // Forzar altura fija
        ForceFixedHeight();
        
        // Aplicar límites de habitación
        if (boundsInitialized && useRoomBounds)
        {
            ApplyRoomBounds();
        }
    }
    
    void ForceFixedHeight()
    {
        float currentHeight = transform.position.y;
        float heightDifference = Mathf.Abs(currentHeight - targetHeight);
        
        if (heightDifference > heightTolerance)
        {
            Vector3 pos = transform.position;
            pos.y = targetHeight;
            transform.position = pos;
            
            // Debug ocasional
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"GhostBehavior: Corrigiendo altura de {currentHeight:F2} a {targetHeight:F2}");
            }
        }
    }
    
    void ApplyRoomBounds()
    {
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
                Debug.Log($"GhostBehavior: {gameObject.name} corregido a posición {clampedPos}");
            }
        }
    }
    
    /// <summary>
    /// Verifica si una posición está dentro de los límites
    /// </summary>
    public bool IsPositionInBounds(Vector3 position)
    {
        if (!boundsInitialized) return true;
        
        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.z >= minBounds.z && position.z <= maxBounds.z;
    }
    
    /// <summary>
    /// Obtiene la posición más cercana dentro de los límites
    /// </summary>
    public Vector3 ClampPositionToBounds(Vector3 position)
    {
        if (!boundsInitialized) return position;
        
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
        boundsInitialized = true;
        Debug.Log($"GhostBehavior: Límites actualizados - Min: {minBounds}, Max: {maxBounds}");
    }
    
    /// <summary>
    /// Cambiar la altura objetivo del fantasma
    /// </summary>
    public void SetTargetHeight(float newHeight)
    {
        targetHeight = newHeight;
        Debug.Log($"GhostBehavior: Nueva altura objetivo: {targetHeight}");
    }
    
    /// <summary>
    /// Obtener la altura objetivo actual
    /// </summary>
    public float GetTargetHeight()
    {
        return targetHeight;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugBounds || !boundsInitialized) return;
        
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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
