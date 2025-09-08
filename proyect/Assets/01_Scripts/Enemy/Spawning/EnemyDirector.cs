using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Controla la oleada de enemigos por piso/sala, aplica progresión,
/// elige jefes, y puede aplicar affixes élite aleatorios.
/// </summary>
public class EnemyDirector : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private EnemySpawnTableSO spawnTable;
    [SerializeField] private BossPoolSO bossPool;
    [SerializeField] private RunSeedProvider runSeed;

    [Header("Tuning por piso")]
    [SerializeField] private int baseCount = 2;             // enemigos base por sala
    [SerializeField] private int addEveryNFloors = 2;       // +1 enemigo cada N pisos
    [SerializeField] private int maxSimultaneous = 6;       // límite para móvil

    [Header("Élites")]
    [SerializeField] private bool allowElites = true;
    [SerializeField] private float eliteBaseChance = 0.05f; // 5%
    [SerializeField] private float elitePerFloor = 0.02f;   // +2% por piso
    [SerializeField] private float eliteMaxChance = 0.35f;

    [Header("Jefes")]
    [SerializeField] private int bossEveryNFloors = 5;
    [SerializeField] private int secondBossStartFloor = 10;
    [SerializeField] private float secondBossChance = 0.10f; // 10%

    [Header("Debug / Estado")]
    [SerializeField] private int floorIndex = 1;  // set por tu flujo de juego
    [SerializeField] private int roomIndex = 0;

    private System.Random _rng;

    private void Reset()
    {
        spawner = GetComponent<EnemySpawner>();
        if (runSeed == null) runSeed = FindObjectOfType<RunSeedProvider>();
    }

    private void Start()
    {
        if (runSeed == null)
        {
            runSeed = FindObjectOfType<RunSeedProvider>();
            if (runSeed == null)
            {
                Debug.LogWarning("EnemyDirector: RunSeedProvider no encontrado en escena. Se creará uno temporal.");
                var go = new GameObject("RunSeedProvider_Auto");
                runSeed = go.AddComponent<RunSeedProvider>();
            }
        }

        _rng = runSeed.GetRngForRoom(floorIndex, roomIndex);

        SpawnWave();
        TrySpawnBosses();
    }

    public void SetFloorAndRoom(int floor, int room)
    {
        floorIndex = floor;
        roomIndex = room;
        _rng = runSeed != null ? runSeed.GetRngForRoom(floorIndex, roomIndex) : new System.Random(floor * 97 + room * 131);
    }

    public void SpawnWave()
    {
        if (spawner == null || spawnTable == null)
        {
            Debug.LogWarning("EnemyDirector: faltan referencias (spawner o spawnTable).");
            return;
        }

        int add = addEveryNFloors > 0 ? floorIndex / addEveryNFloors : 0;
        int count = Mathf.Clamp(baseCount + add, baseCount, maxSimultaneous);

        for (int i = 0; i < count; i++)
        {
            var pf = spawnTable.Roll(floorIndex, _rng);
            if (pf == null) continue;

            var go = spawner.Spawn(pf, _rng);

            // Aplica affix élite aleatorio
            if (allowElites && RollElite())
                ApplyRandomElite(go);
        }
    }

    public void TrySpawnBosses()
    {
        if (bossPool == null || bossEveryNFloors <= 0) return;

        if (floorIndex % bossEveryNFloors != 0) return;

        // Uno garantizado
        SpawnBoss();

        // Chance de segundo a partir del piso definido
        if (floorIndex >= secondBossStartFloor && _rng.NextDouble() < secondBossChance)
            StartCoroutine(SpawnBossDelayed(6f)); // respawn diferido para no saturar
    }

    private void SpawnBoss()
    {
        var bossPf = bossPool.Roll(_rng);
        if (bossPf == null) return;
        spawner.Spawn(bossPf, _rng);
    }

    private IEnumerator SpawnBossDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBoss();
    }

    private bool RollElite()
    {
        float chance = Mathf.Clamp01(eliteBaseChance + elitePerFloor * floorIndex);
        chance = Mathf.Min(chance, eliteMaxChance);
        return _rng.NextDouble() < chance;
    }

    private void ApplyRandomElite(GameObject enemy)
    {
        // Lista de affixes disponibles: añade/quita según tus scripts
        var choices = new System.Type[]
        {
            typeof(EliteVampiric),
            typeof(EliteSwift),
            typeof(EliteExplosive),
            typeof(EliteEthereal)
        };

        int index = _rng.Next(0, choices.Length);
        var type = choices[index];
        var affix = enemy.GetComponent(type) as IEliteAffix;
        if (affix == null)
        {
            affix = enemy.AddComponent(type) as IEliteAffix;
        }
        if (affix != null)
        {
            affix.Apply(enemy);
            // Opcional: marcar visualmente al élite (cambio de material/glow).
        }
    }
}
