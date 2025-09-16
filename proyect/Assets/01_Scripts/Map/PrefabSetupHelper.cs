using UnityEngine;
using System.Collections.Generic;

public class PrefabSetupHelper : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string instructions = @"INSTRUCCIONES DE CONFIGURACIÓN:

1. Arrastra los prefabs del Modular Dungeon Pack a la carpeta 02_Prefabs
2. Usa el RoomPrefabBuilder para crear habitaciones
3. Configura el DungeonConfigurator con los tipos de habitaciones
4. Usa el DungeonGenerator para generar el mapa

PREFABS NECESARIOS:
- Wall_A, Wall_B, Wall_C (Paredes)
- Arch_A (Puertas/Arcos)
- Tile_A, Tile_B (Suelos)
- Torch_A, Torch_B (Antorchas)
- Pillar_A, Pillar_B (Pilares)";

    [ContextMenu("Setup Room Prefabs")]
    public void SetupRoomPrefabs()
    {
        Debug.Log("Configurando prefabs de habitaciones...");
        
        // Crear carpeta de prefabs si no existe
        if (!System.IO.Directory.Exists("Assets/02_Prefabs/RoomPrefabs"))
        {
            System.IO.Directory.CreateDirectory("Assets/02_Prefabs/RoomPrefabs");
        }
        
        // Crear prefabs básicos
        CreateBasicRoomPrefab();
        CreateEnemyRoomPrefab();
        CreateSpecialRoomPrefab();
        
        Debug.Log("Prefabs de habitaciones creados en Assets/02_Prefabs/RoomPrefabs/");
    }
    
    void CreateBasicRoomPrefab()
    {
        GameObject room = new GameObject("Room_Basic");
        
        // Agregar componentes
        Room roomComponent = room.AddComponent<Room>();
        RoomBehaveor roomBehavior = room.AddComponent<RoomBehaveor>();
        
        // Configurar como habitación básica
        roomBehavior.enemyRoom = false;
        roomBehavior.enemyMinSpawn = 0;
        roomBehavior.enemyMaxSpawn = 0;
        
        // Crear estructura básica
        CreateRoomStructure(room, roomComponent, roomBehavior);
        
        // Guardar como prefab
        SaveAsPrefab(room, "Room_Basic");
    }
    
    void CreateEnemyRoomPrefab()
    {
        GameObject room = new GameObject("Room_Enemy");
        
        // Agregar componentes
        Room roomComponent = room.AddComponent<Room>();
        RoomBehaveor roomBehavior = room.AddComponent<RoomBehaveor>();
        
        // Configurar como habitación de enemigos
        roomBehavior.enemyRoom = true;
        roomBehavior.enemyMinSpawn = 2;
        roomBehavior.enemyMaxSpawn = 5;
        roomBehavior.timeBtwSpawn = 2f;
        roomBehavior.spawnDelaySpeed = 1f;
        
        // Crear estructura básica
        CreateRoomStructure(room, roomComponent, roomBehavior);
        
        // Guardar como prefab
        SaveAsPrefab(room, "Room_Enemy");
    }
    
    void CreateSpecialRoomPrefab()
    {
        GameObject room = new GameObject("Room_Special");
        
        // Agregar componentes
        Room roomComponent = room.AddComponent<Room>();
        RoomBehaveor roomBehavior = room.AddComponent<RoomBehaveor>();
        
        // Configurar como habitación especial
        roomBehavior.enemyRoom = false;
        roomBehavior.enemyMinSpawn = 0;
        roomBehavior.enemyMaxSpawn = 0;
        
        // Crear estructura básica
        CreateRoomStructure(room, roomComponent, roomBehavior);
        
        // Guardar como prefab
        SaveAsPrefab(room, "Room_Special");
    }
    
    void CreateRoomStructure(GameObject room, Room roomComponent, RoomBehaveor roomBehavior)
    {
        // Crear suelo
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(room.transform);
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = new Vector3(10, 0.1f, 10);
        
        // Crear paredes
        List<GameObject> walls = new List<GameObject>();
        List<GameObject> doors = new List<GameObject>();
        
        // Pared Norte
        GameObject wallNorth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallNorth.name = "Wall_North";
        wallNorth.transform.SetParent(room.transform);
        wallNorth.transform.localPosition = new Vector3(0, 1.5f, 5);
        wallNorth.transform.localScale = new Vector3(10, 3, 0.2f);
        walls.Add(wallNorth);
        
        // Pared Sur
        GameObject wallSouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallSouth.name = "Wall_South";
        wallSouth.transform.SetParent(room.transform);
        wallSouth.transform.localPosition = new Vector3(0, 1.5f, -5);
        wallSouth.transform.localScale = new Vector3(10, 3, 0.2f);
        walls.Add(wallSouth);
        
        // Pared Este
        GameObject wallEast = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallEast.name = "Wall_East";
        wallEast.transform.SetParent(room.transform);
        wallEast.transform.localPosition = new Vector3(5, 1.5f, 0);
        wallEast.transform.localScale = new Vector3(0.2f, 3, 10);
        walls.Add(wallEast);
        
        // Pared Oeste
        GameObject wallWest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallWest.name = "Wall_West";
        wallWest.transform.SetParent(room.transform);
        wallWest.transform.localPosition = new Vector3(-5, 1.5f, 0);
        wallWest.transform.localScale = new Vector3(0.2f, 3, 10);
        walls.Add(wallWest);
        
        // Crear puertas (invisibles por defecto)
        for (int i = 0; i < 4; i++)
        {
            GameObject door = new GameObject("Door_" + i);
            door.transform.SetParent(room.transform);
            door.SetActive(false);
            doors.Add(door);
        }
        
        // Configurar componentes
        roomComponent.walls = walls.ToArray();
        roomComponent.doors = doors.ToArray();
        roomComponent.status = new bool[4];
        
        roomBehavior.walls = walls.ToArray();
        roomBehavior.Doors = doors.ToArray();
        roomBehavior.status = roomComponent.status;
        
        // Crear spawn points
        CreateSpawnPoints(room, roomBehavior);
    }
    
    void CreateSpawnPoints(GameObject room, RoomBehaveor roomBehavior)
    {
        Transform[] spawnPoints = new Transform[4];
        
        for (int i = 0; i < 4; i++)
        {
            GameObject spawnPoint = new GameObject("SpawnPoint_" + i);
            spawnPoint.transform.SetParent(room.transform);
            
            float angle = 90f * i;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 3;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 3;
            
            spawnPoint.transform.localPosition = new Vector3(x, 0, z);
            spawnPoints[i] = spawnPoint.transform;
        }
        
        roomBehavior.spawnPoints = spawnPoints;
    }
    
    void SaveAsPrefab(GameObject room, string prefabName)
    {
        string prefabPath = "Assets/02_Prefabs/RoomPrefabs/" + prefabName + ".prefab";
        
        #if UNITY_EDITOR
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(room, prefabPath);
        #endif
        
        DestroyImmediate(room);
    }
    
    [ContextMenu("Create Dungeon Generator")]
    public void CreateDungeonGenerator()
    {
        GameObject dungeonGen = new GameObject("DungeonGenerator");
        dungeonGen.transform.SetParent(transform);
        
        DungeonGenerator generator = dungeonGen.AddComponent<DungeonGenerator>();
        DungeonConfigurator configurator = dungeonGen.AddComponent<DungeonConfigurator>();
        
        configurator.dungeonGenerator = generator;
        configurator.dungeonSize = new Vector2Int(5, 5);
        configurator.roomOffset = new Vector2(20, 20);
        
        Debug.Log("DungeonGenerator creado con DungeonConfigurator");
    }
}
