using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy Spawn Table", fileName = "EnemySpawnTable")]
public class EnemySpawnTableSO : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public GameObject prefab;       // Prefab del enemigo
        [Range(0, 1)] public float weight; // Probabilidad relativa
        public int minFloor;            // Piso mínimo en que puede aparecer
    }

    public List<Entry> entries = new List<Entry>();

    /// <summary>
    /// Selecciona un prefab aleatorio válido para el piso actual.
    /// Usa un System.Random (semilla reproducible) en vez de UnityEngine.Random.
    /// </summary>
    public GameObject Roll(int floorIndex, System.Random rng)
    {
        var valid = entries.FindAll(e => floorIndex >= e.minFloor);
        if (valid.Count == 0) return null;

        double total = 0;
        foreach (var e in valid) total += e.weight;

        double r = rng.NextDouble() * total;
        foreach (var e in valid)
        {
            r -= e.weight;
            if (r <= 0) return e.prefab;
        }
        return valid[valid.Count - 1].prefab;
    }
}