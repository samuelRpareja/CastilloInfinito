using UnityEngine;

/// <summary>
/// Script que asegura que el jugador esté registrado como target para los enemigos.
/// Debe estar en el GameObject del jugador.
/// </summary>
public class PlayerTargetSetup : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform aimRoot; // Punto de mira del jugador (opcional)
    
    private PlayerProxy playerProxy;
    
    void Start()
    {
        SetupPlayerTarget();
    }
    
    void SetupPlayerTarget()
    {
        // Buscar o crear PlayerProxy
        playerProxy = GetComponent<PlayerProxy>();
        if (playerProxy == null)
        {
            playerProxy = gameObject.AddComponent<PlayerProxy>();
            Debug.Log("PlayerTargetSetup: Agregado PlayerProxy al jugador");
        }
        
        // Configurar aimRoot si no está asignado
        if (aimRoot == null)
        {
            aimRoot = transform;
        }
        
        // Asegurar que existe TargetRegistry
        if (TargetRegistry.Instance == null)
        {
            GameObject registryGO = new GameObject("TargetRegistry");
            registryGO.AddComponent<TargetRegistry>();
            Debug.Log("PlayerTargetSetup: Creado TargetRegistry");
        }
        
        // El PlayerProxy se registra automáticamente en OnEnable
        Debug.Log("PlayerTargetSetup: Jugador configurado como target para enemigos");
    }
    
    void OnDrawGizmosSelected()
    {
        if (aimRoot != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(aimRoot.position, 0.5f);
        }
    }
}
