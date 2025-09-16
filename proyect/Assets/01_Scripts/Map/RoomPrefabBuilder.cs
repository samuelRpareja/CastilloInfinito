using UnityEngine;
using System.Collections.Generic;

public class RoomPrefabBuilder : MonoBehaviour
{
    [Header("Modular Dungeon Pack References")]
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject floorPrefab;
    public GameObject torchPrefab;
    public GameObject pillarPrefab;
    
    [Header("Room Configuration")]
    public Vector2 roomSize = new Vector2(10, 10);
    public float wallHeight = 3f;
    public int torchCount = 4;
    
    [Header("Generated Room")]
    public GameObject generatedRoom;
    
    void Start()
    {
        // Cargar automáticamente los prefabs del Modular Dungeon Pack
        LoadModularPrefabs();
    }
    
    void LoadModularPrefabs()
    {
        // Cargar prefabs del Modular Dungeon Pack
        wallPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Wall_A");
        doorPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Arch_A");
        floorPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Tile_A");
        torchPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Torch_A");
        pillarPrefab = Resources.Load<GameObject>("DungeonModularPack/Prefabs/Pillar_A");
        
        if (wallPrefab == null)
        {
            Debug.LogError("No se encontraron los prefabs del Modular Dungeon Pack. Verifica la ruta.");
        }
    }
    
    [ContextMenu("Build Room")]
    public void BuildRoom()
    {
        if (generatedRoom != null)
        {
            DestroyImmediate(generatedRoom);
        }
        
        // Crear el GameObject principal de la habitación
        generatedRoom = new GameObject("Room_Generated");
        generatedRoom.transform.SetParent(transform);
        
        // Agregar componentes necesarios
        Room roomComponent = generatedRoom.AddComponent<Room>();
        RoomBehaveor roomBehavior = generatedRoom.AddComponent<RoomBehaveor>();
        
        // Crear estructura de la habitación
        CreateFloor(generatedRoom);
        CreateWalls(generatedRoom, roomComponent);
        CreateDoors(generatedRoom, roomComponent);
        CreateDecorations(generatedRoom);
        
        // Configurar el RoomBehaveor
        ConfigureRoomBehavior(roomBehavior, roomComponent);
        
        Debug.Log("Habitación generada exitosamente!");
    }
    
    void CreateFloor(GameObject room)
    {
        if (floorPrefab == null) return;
        
        GameObject floor = Instantiate(floorPrefab, room.transform);
        floor.name = "Floor";
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = new Vector3(roomSize.x, 1, roomSize.y);
    }
    
    void CreateWalls(GameObject room, Room roomComponent)
    {
        if (wallPrefab == null) return;
        
        List<GameObject> walls = new List<GameObject>();
        
        // Pared Norte
        GameObject wallNorth = Instantiate(wallPrefab, room.transform);
        wallNorth.name = "Wall_North";
        wallNorth.transform.localPosition = new Vector3(0, wallHeight/2, roomSize.y/2);
        wallNorth.transform.localScale = new Vector3(roomSize.x, wallHeight, 1);
        walls.Add(wallNorth);
        
        // Pared Sur
        GameObject wallSouth = Instantiate(wallPrefab, room.transform);
        wallSouth.name = "Wall_South";
        wallSouth.transform.localPosition = new Vector3(0, wallHeight/2, -roomSize.y/2);
        wallSouth.transform.localScale = new Vector3(roomSize.x, wallHeight, 1);
        walls.Add(wallSouth);
        
        // Pared Este
        GameObject wallEast = Instantiate(wallPrefab, room.transform);
        wallEast.name = "Wall_East";
        wallEast.transform.localPosition = new Vector3(roomSize.x/2, wallHeight/2, 0);
        wallEast.transform.localScale = new Vector3(1, wallHeight, roomSize.y);
        walls.Add(wallEast);
        
        // Pared Oeste
        GameObject wallWest = Instantiate(wallPrefab, room.transform);
        wallWest.name = "Wall_West";
        wallWest.transform.localPosition = new Vector3(-roomSize.x/2, wallHeight/2, 0);
        wallWest.transform.localScale = new Vector3(1, wallHeight, roomSize.y);
        walls.Add(wallWest);
        
        // Asignar al componente Room
        roomComponent.walls = walls.ToArray();
    }
    
    void CreateDoors(GameObject room, Room roomComponent)
    {
        if (doorPrefab == null) return;
        
        List<GameObject> doors = new List<GameObject>();
        
        // Puerta Norte
        GameObject doorNorth = Instantiate(doorPrefab, room.transform);
        doorNorth.name = "Door_North";
        doorNorth.transform.localPosition = new Vector3(0, 0, roomSize.y/2);
        doors.Add(doorNorth);
        
        // Puerta Sur
        GameObject doorSouth = Instantiate(doorPrefab, room.transform);
        doorSouth.name = "Door_South";
        doorSouth.transform.localPosition = new Vector3(0, 0, -roomSize.y/2);
        doors.Add(doorSouth);
        
        // Puerta Este
        GameObject doorEast = Instantiate(doorPrefab, room.transform);
        doorEast.name = "Door_East";
        doorEast.transform.localPosition = new Vector3(roomSize.x/2, 0, 0);
        doors.Add(doorEast);
        
        // Puerta Oeste
        GameObject doorWest = Instantiate(doorPrefab, room.transform);
        doorWest.name = "Door_West";
        doorWest.transform.localPosition = new Vector3(-roomSize.x/2, 0, 0);
        doors.Add(doorWest);
        
        // Asignar al componente Room
        roomComponent.doors = doors.ToArray();
    }
    
    void CreateDecorations(GameObject room)
    {
        if (torchPrefab == null) return;
        
        // Crear antorchas en las esquinas
        for (int i = 0; i < torchCount; i++)
        {
            GameObject torch = Instantiate(torchPrefab, room.transform);
            torch.name = "Torch_" + i;
            
            // Posicionar en las esquinas
            float angle = (360f / torchCount) * i;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * (roomSize.x/2 - 1);
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * (roomSize.y/2 - 1);
            
            torch.transform.localPosition = new Vector3(x, 0, z);
        }
    }
    
    void ConfigureRoomBehavior(RoomBehaveor roomBehavior, Room roomComponent)
    {
        // Configurar referencias
        roomBehavior.walls = roomComponent.walls;
        roomBehavior.Doors = roomComponent.doors;
        
        // Configurar spawn points (centro de la habitación)
        Transform[] spawnPoints = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject spawnPoint = new GameObject("SpawnPoint_" + i);
            spawnPoint.transform.SetParent(roomBehavior.transform);
            
            float angle = 90f * i;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 2;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 2;
            
            spawnPoint.transform.localPosition = new Vector3(x, 0, z);
            spawnPoints[i] = spawnPoint.transform;
        }
        
        roomBehavior.spawnPoints = spawnPoints;
        
        // Configurar materiales (usar los del Modular Dungeon Pack)
        List<Material> materials = new List<Material>();
        Material wallMaterial = Resources.Load<Material>("DungeonModularPack/Materials/M_Wall");
        if (wallMaterial != null)
        {
            materials.Add(wallMaterial);
        }
        roomBehavior.roomMaterials = materials;
    }
    
    [ContextMenu("Save as Prefab")]
    public void SaveAsPrefab()
    {
        if (generatedRoom == null)
        {
            Debug.LogError("No hay habitación generada para guardar.");
            return;
        }
        
        #if UNITY_EDITOR
        // Crear prefab en la carpeta de prefabs
        string prefabPath = "Assets/02_Prefabs/RoomPrefabs/Room_Generated.prefab";
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(generatedRoom, prefabPath);
        Debug.Log("Prefab guardado en: " + prefabPath);
        #else
        Debug.Log("Guardar prefab solo disponible en el editor.");
        #endif
    }
}
