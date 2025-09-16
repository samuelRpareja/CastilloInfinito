using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawnea un prefab de enemigo en un punto dado. Pensado para ser
/// llamado por EnemyDirector u otros sistemas.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Puntos de aparición posibles.")]
    public Transform[] spawnPoints;

    /// <summary>
    /// Instancia un enemigo en un punto aleatorio. Devuelve el GameObject creado.
    /// </summary>
    public GameObject Spawn(GameObject prefab, System.Random rng)
    {
        if (prefab == null || spawnPoints == null || spawnPoints.Length == 0) return null;

        int idx = rng.Next(0, spawnPoints.Length);
        var p = spawnPoints[idx];

        var go = Instantiate(prefab, p.position, p.rotation);
        // Inicializa cualquier componente que lo requiera
        var inits = go.GetComponents<IInitializable>();
        for (int i = 0; i < inits.Length; i++) inits[i].Initialize();

        return go;
    }

    /// <summary>
    /// Instancia N enemigos. Devuelve el arreglo de instancias creadas.
    /// </summary>
    public GameObject[] SpawnMany(GameObject prefab, int count, System.Random rng)
    {
        if (count <= 0) return new GameObject[0];
        var arr = new GameObject[count];
        for (int i = 0; i < count; i++)
            arr[i] = Spawn(prefab, rng);
        return arr;
    }
}
