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
    public List<Enemy> activeEnemys;
    void Start()
    {
        status = GetComponent<Room>().status;
        Player = GameObject.FindGameObjectWithTag("Player");
        enemyRoom = Random.Range(0, 2) == 1 ? true : false;
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
    }
    void SpawnRoomMaterial()
    {
        Material rm = roomMaterials[Random.Range(0, roomMaterials.Count)];
        foreach (GameObject wall in walls)
        {

            wall.GetComponent<Renderer>().material = rm;
        }
    }
    public void DeleteActiveEnemy(Enemy e)
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
                Transform position = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyPrefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
                Enemy enemy = Instantiate(enemyPrefab, position.position, position.rotation).GetComponent<Enemy>();
                enemy.SpawnRoom = this.gameObject;
                activeEnemys.Add(enemy);
                spawnCount++;
                timmer = 0;
            }
            else
            {
                timmer += Time.deltaTime * spawnDelaySpeed;
            }
        }

    }

}
