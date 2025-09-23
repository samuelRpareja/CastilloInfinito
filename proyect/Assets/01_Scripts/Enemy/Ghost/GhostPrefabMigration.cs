using UnityEngine;

/// <summary>
/// Script de migración para actualizar el prefab del fantasma
/// Reemplaza los scripts antiguos con el nuevo GhostBehavior
/// </summary>
public class GhostPrefabMigration : MonoBehaviour
{
    [ContextMenu("Migrar Prefab de Fantasma")]
    public void MigrateGhostPrefab()
    {
        Debug.Log("Iniciando migración del prefab de fantasma...");
        
        // Remover scripts antiguos
        RemoveOldScripts();
        
        // Agregar nuevo script unificado
        AddNewScript();
        
        Debug.Log("Migración completada. El fantasma ahora usa GhostBehavior unificado.");
    }
    
    void RemoveOldScripts()
    {
        // Remover GhostController
        var ghostController = GetComponent<GhostController>();
        if (ghostController != null)
        {
            DestroyImmediate(ghostController);
            Debug.Log("Removido GhostController");
        }
        
        // Remover GhostHeightLock
        var ghostHeightLock = GetComponent<GhostHeightLock>();
        if (ghostHeightLock != null)
        {
            DestroyImmediate(ghostHeightLock);
            Debug.Log("Removido GhostHeightLock");
        }
        
        // Remover GhostRoomBounds
        var ghostRoomBounds = GetComponent<GhostRoomBounds>();
        if (ghostRoomBounds != null)
        {
            DestroyImmediate(ghostRoomBounds);
            Debug.Log("Removido GhostRoomBounds");
        }
        
        // Remover GhostRoomBoundsSetup
        var ghostRoomBoundsSetup = GetComponent<GhostRoomBoundsSetup>();
        if (ghostRoomBoundsSetup != null)
        {
            DestroyImmediate(ghostRoomBoundsSetup);
            Debug.Log("Removido GhostRoomBoundsSetup");
        }
    }
    
    void AddNewScript()
    {
        // Agregar GhostBehavior si no existe
        var ghostBehavior = GetComponent<GhostBehavior>();
        if (ghostBehavior == null)
        {
            ghostBehavior = gameObject.AddComponent<GhostBehavior>();
            Debug.Log("Agregado GhostBehavior");
        }
        else
        {
            Debug.Log("GhostBehavior ya existe");
        }
    }
}
