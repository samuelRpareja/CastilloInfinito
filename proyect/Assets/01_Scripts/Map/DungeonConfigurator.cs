using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomType
{
    public string name;
    public GameObject prefab;
    public float spawnWeight = 1f;
    public bool canSpawnAtStart = false;
    public bool canSpawnAtEnd = false;
    public Vector2Int minPosition = new Vector2Int(0, 0);
    public Vector2Int maxPosition = new Vector2Int(10, 10);
}

public class DungeonConfigurator : MonoBehaviour
{
    [Header("Room Types")]
    public RoomType[] roomTypes;
    
    [Header("Dungeon Settings")]
    public Vector2Int dungeonSize = new Vector2Int(10, 10);
    public Vector2 roomOffset = new Vector2(20, 20);
    
    [Header("References")]
    public DungeonGenerator dungeonGenerator;
    
    void Start()
    {
        if (dungeonGenerator == null)
        {
            dungeonGenerator = GetComponent<DungeonGenerator>();
        }
        
        ConfigureDungeonGenerator();
    }
    
    [ContextMenu("Configure Dungeon")]
    public void ConfigureDungeonGenerator()
    {
        if (dungeonGenerator == null)
        {
            Debug.LogError("DungeonGenerator no encontrado!");
            return;
        }
        
        // Configurar tamaño y offset
        dungeonGenerator.size = dungeonSize;
        dungeonGenerator.offset = roomOffset;
        
        // Convertir RoomTypes a Rules
        List<DungeonGenerator.Rule> rules = new List<DungeonGenerator.Rule>();
        
        foreach (RoomType roomType in roomTypes)
        {
            if (roomType.prefab != null)
            {
                DungeonGenerator.Rule rule = new DungeonGenerator.Rule();
                rule.room = roomType.prefab;
                rule.minPosition = roomType.minPosition;
                rule.maxPosition = roomType.maxPosition;
                rule.obligatory = false;
                
                rules.Add(rule);
            }
        }
        
        dungeonGenerator.rooms = rules.ToArray();
        
        Debug.Log($"Dungeon configurado con {rules.Count} tipos de habitaciones");
    }
    
    [ContextMenu("Create Default Room Types")]
    public void CreateDefaultRoomTypes()
    {
        List<RoomType> defaultRooms = new List<RoomType>();
        
        // Habitación básica
        RoomType basicRoom = new RoomType();
        basicRoom.name = "Basic Room";
        basicRoom.spawnWeight = 3f;
        basicRoom.canSpawnAtStart = true;
        basicRoom.canSpawnAtEnd = true;
        basicRoom.minPosition = new Vector2Int(0, 0);
        basicRoom.maxPosition = new Vector2Int(dungeonSize.x, dungeonSize.y);
        defaultRooms.Add(basicRoom);
        
        // Habitación de enemigos
        RoomType enemyRoom = new RoomType();
        enemyRoom.name = "Enemy Room";
        enemyRoom.spawnWeight = 2f;
        enemyRoom.canSpawnAtStart = false;
        enemyRoom.canSpawnAtEnd = false;
        enemyRoom.minPosition = new Vector2Int(1, 1);
        enemyRoom.maxPosition = new Vector2Int(dungeonSize.x - 1, dungeonSize.y - 1);
        defaultRooms.Add(enemyRoom);
        
        // Habitación especial
        RoomType specialRoom = new RoomType();
        specialRoom.name = "Special Room";
        specialRoom.spawnWeight = 0.5f;
        specialRoom.canSpawnAtStart = false;
        specialRoom.canSpawnAtEnd = true;
        specialRoom.minPosition = new Vector2Int(dungeonSize.x - 2, dungeonSize.y - 2);
        specialRoom.maxPosition = new Vector2Int(dungeonSize.x, dungeonSize.y);
        defaultRooms.Add(specialRoom);
        
        roomTypes = defaultRooms.ToArray();
        
        Debug.Log("Tipos de habitaciones por defecto creados");
    }
    
    [ContextMenu("Load Modular Dungeon Prefabs")]
    public void LoadModularDungeonPrefabs()
    {
        // Cargar prefabs del Modular Dungeon Pack
        GameObject basicRoomPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Room_Basic");
        GameObject enemyRoomPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Room_Enemy");
        GameObject specialRoomPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Room_Special");
        
        if (roomTypes == null || roomTypes.Length == 0)
        {
            CreateDefaultRoomTypes();
        }
        
        // Asignar prefabs a los tipos de habitaciones
        if (roomTypes.Length > 0 && basicRoomPrefab != null)
        {
            roomTypes[0].prefab = basicRoomPrefab;
        }
        
        if (roomTypes.Length > 1 && enemyRoomPrefab != null)
        {
            roomTypes[1].prefab = enemyRoomPrefab;
        }
        
        if (roomTypes.Length > 2 && specialRoomPrefab != null)
        {
            roomTypes[2].prefab = specialRoomPrefab;
        }
        
        Debug.Log("Prefabs del Modular Dungeon Pack cargados");
    }
    
    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        ConfigureDungeonGenerator();
        
        if (dungeonGenerator != null)
        {
            // Limpiar dungeon anterior
            foreach (Transform child in dungeonGenerator.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            
            // Generar nuevo dungeon
            dungeonGenerator.Start();
        }
    }
}
