using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
public class RoomBehaveor : MonoBehaviour
{
    [Header("Stats")]
    public bool enemyRoom;
    public int enemyMinSpawn;
    public int enemyMaxSpawn;
    public float timmer = 0;
    public float timeBtwSpawn;
    public float spawnDelaySpeed;
    public int totalToSpawn = 0;
    [Header("References")]
    public List<Material> roomMaterials;
    public GameObject[] walls;
    public Transform[] spawnPoints;
    public GameObject[] EnemyPrefabs;
    public GameObject Player;
    public GameObject[] Doors;
    [Header("internal Values")]
    public bool isPlayerOnRoom = false;
    public bool isRoomComplete = false;
    public bool isOpended = true;
    public int spawnCount = 0;
    public bool[] status;
    public List<EnemyCommon> activeEnemys;
    void Start()
    {
        status = GetComponent<Room>().status;
        Player = GameObject.FindGameObjectWithTag("Player");
        // Forzar enemyRoom = true para que siempre spawneen enemigos
        enemyRoom = true;
        if (enemyRoom)
        {
            totalToSpawn = Random.Range(enemyMinSpawn, enemyMaxSpawn + 1);
        }
        SpawnRoomMaterial();
    }
    void CloseDoors(bool status)
    {
        for (int i = 0; i < Doors.Length; i++)
        {

            if (this.status[i])
            {
                Collider doorCollider = Doors[i].GetComponentInParent<Collider>();
                doorCollider.enabled = status;
                Doors[i].SetActive(status);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Debug para diagnosticar problemas de spawn
        if (isPlayerOnRoom && enemyRoom && !isRoomComplete)
        {
            if (isOpended)
            {
                CloseDoors(true);
                isOpended = false;
            }
            SpawnEnemy();
        }
        else if (isPlayerOnRoom && !enemyRoom && !isRoomComplete)
        {
            isRoomComplete = true;
        }
        
        // Debug info
        if (isPlayerOnRoom)
        {
            Debug.Log($"Room Debug - isPlayerOnRoom: {isPlayerOnRoom}, enemyRoom: {enemyRoom}, isRoomComplete: {isRoomComplete}, spawnCount: {spawnCount}/{totalToSpawn}");
        }
    }
    void SpawnRoomMaterial()
    {
        if (roomMaterials == null || roomMaterials.Count == 0)
        {
            Debug.LogWarning("RoomBehaveor: No hay materiales configurados en roomMaterials");
            return;
        }
        
        Material rm = roomMaterials[Random.Range(0, roomMaterials.Count)];
        foreach (GameObject wall in walls)
        {
            if (wall != null && wall.GetComponent<Renderer>() != null)
            {
                wall.GetComponent<Renderer>().material = rm;
            }
        }
    }
    public void DeleteActiveEnemy(EnemyCommon e)
    {
        activeEnemys.Remove(e);
    }
    void SpawnEnemy()
    {
        if (spawnCount == totalToSpawn)
        {
            if (activeEnemys.Count == 0)
            {
                if (!isOpended)
                {
                    CloseDoors(false);
                    isOpended = true;
                }

                Debug.Log("Completado");
                isRoomComplete = true;
            }
        }
        else
        {
            if (timmer >= timeBtwSpawn)
            {
                // Verificar que tenemos spawnPoints y EnemyPrefabs
                if (spawnPoints == null || spawnPoints.Length == 0)
                {
                    Debug.LogError("RoomBehaveor: No hay spawnPoints configurados!");
                    return;
                }
                
                if (EnemyPrefabs == null || EnemyPrefabs.Length == 0)
                {
                    Debug.LogError("RoomBehaveor: No hay EnemyPrefabs configurados!");
                    return;
                }
                
                Transform position = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyPrefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
                
                if (enemyPrefab == null)
                {
                    Debug.LogError("RoomBehaveor: EnemyPrefab es null!");
                    return;
                }
                
                Debug.Log($"Spawneando enemigo en posición: {position.position}");
                EnemyCommon enemy = Instantiate(enemyPrefab, position.position, position.rotation).GetComponent<EnemyCommon>();
                enemy.SpawnRoom = this.gameObject;
                activeEnemys.Add(enemy);
                
                // Configurar límites de habitación para el fantasma
                SetupGhostRoomBounds(enemy);
                
                spawnCount++;
                timmer = 0;
            }
            else
            {
                timmer += Time.deltaTime * spawnDelaySpeed;
            }
        }
    }
    
    void SetupGhostRoomBounds(EnemyCommon enemy)
    {
        // Verificar si es un fantasma
        var ghostBehavior = enemy.GetComponent<GhostBehavior>();
        if (ghostBehavior == null) return;
        
        // Configurar límites basados en esta habitación
        Vector3 roomPos = transform.position;
        Vector3 roomScale = transform.localScale;
        
        // Calcular límites de la habitación
        Vector3 roomSize = new Vector3(roomScale.x, 0f, roomScale.z);
        Vector3 minBounds = roomPos - roomSize / 2f + Vector3.one * 0.5f; // margen de 0.5
        Vector3 maxBounds = roomPos + roomSize / 2f - Vector3.one * 0.5f;
        
        // Mantener altura del fantasma
        minBounds.y = enemy.transform.position.y;
        maxBounds.y = enemy.transform.position.y;
        
        // Aplicar límites
        ghostBehavior.SetBounds(minBounds, maxBounds);
        
        Debug.Log($"RoomBehaveor: Límites configurados para fantasma en habitación {gameObject.name}");
        Debug.Log($"  Límites: Min {minBounds}, Max {maxBounds}");
    }

}
