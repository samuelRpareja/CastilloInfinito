using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Boss Pool", fileName = "BossPool")]
public class BossPoolSO : ScriptableObject
{
    [Tooltip("Prefabs de bosses disponibles en esta run")]
    public List<GameObject> bosses = new List<GameObject>();

    /// <summary>
    /// Devuelve un prefab de boss aleatorio usando un RNG reproducible.
    /// </summary>
    public GameObject Roll(System.Random rng)
    {
        if (bosses == null || bosses.Count == 0) return null;
        int i = rng.Next(0, bosses.Count);
        return bosses[i];
    }
}