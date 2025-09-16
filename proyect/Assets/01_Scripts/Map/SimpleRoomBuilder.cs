using UnityEngine;
using System.Collections.Generic;

public class SimpleRoomBuilder : MonoBehaviour
{
    [Header("Room Settings")]
    public Vector2 roomSize = new Vector2(10, 10);
    public float wallHeight = 3f;
    
    [Header("Prefab References")]
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject floorPrefab;
    
    [Header("Generated Room")]
    public GameObject currentRoom;
    
    [ContextMenu("Build Simple Room")]
    public void BuildSimpleRoom()
    {
        if (currentRoom != null)
        {
            DestroyImmediate(currentRoom);
        }
        
        // Crear el GameObject principal de la habitación
        currentRoom = new GameObject("SimpleRoom");
        currentRoom.transform.SetParent(transform);
        
        // Agregar componentes necesarios
        Room roomComponent = currentRoom.AddComponent<Room>();
        RoomBehaveor roomBehavior = currentRoom.AddComponent<RoomBehaveor>();
        
        // Crear estructura básica
        CreateBasicRoomStructure(currentRoom, roomComponent, roomBehavior);
        
        Debug.Log("Habitación simple creada exitosamente!");
    }
    
    void CreateBasicRoomStructure(GameObject room, Room roomComponent, RoomBehaveor roomBehavior)
    {
        // Crear suelo
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(room.transform);
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = new Vector3(roomSize.x, 0.1f, roomSize.y);
        
        // Crear paredes
        List<GameObject> walls = new List<GameObject>();
        List<GameObject> doors = new List<GameObject>();
        
        // Pared Norte
        GameObject wallNorth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallNorth.name = "Wall_North";
        wallNorth.transform.SetParent(room.transform);
        wallNorth.transform.localPosition = new Vector3(0, wallHeight/2, roomSize.y/2);
        wallNorth.transform.localScale = new Vector3(roomSize.x, wallHeight, 0.2f);
        walls.Add(wallNorth);
        
        // Pared Sur
        GameObject wallSouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallSouth.name = "Wall_South";
        wallSouth.transform.SetParent(room.transform);
        wallSouth.transform.localPosition = new Vector3(0, wallHeight/2, -roomSize.y/2);
        wallSouth.transform.localScale = new Vector3(roomSize.x, wallHeight, 0.2f);
        walls.Add(wallSouth);
        
        // Pared Este
        GameObject wallEast = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallEast.name = "Wall_East";
        wallEast.transform.SetParent(room.transform);
        wallEast.transform.localPosition = new Vector3(roomSize.x/2, wallHeight/2, 0);
        wallEast.transform.localScale = new Vector3(0.2f, wallHeight, roomSize.y);
        walls.Add(wallEast);
        
        // Pared Oeste
        GameObject wallWest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallWest.name = "Wall_West";
        wallWest.transform.SetParent(room.transform);
        wallWest.transform.localPosition = new Vector3(-roomSize.x/2, wallHeight/2, 0);
        wallWest.transform.localScale = new Vector3(0.2f, wallHeight, roomSize.y);
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
    
    [ContextMenu("Clear Room")]
    public void ClearRoom()
    {
        if (currentRoom != null)
        {
            DestroyImmediate(currentRoom);
            currentRoom = null;
            Debug.Log("Habitación eliminada");
        }
    }
}
