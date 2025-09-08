using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool simple para proyectiles. Evita Instantiate/Destroy en runtime.
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public GameObject prefab;
        public int prewarm = 5;
    }

    [Header("Prefabs a precargar")]
    public List<Entry> prewarmed = new List<Entry>();

    private readonly Dictionary<GameObject, Stack<EnemyProjectile>> _free =
        new Dictionary<GameObject, Stack<EnemyProjectile>>();

    private readonly Dictionary<EnemyProjectile, GameObject> _origin =
        new Dictionary<EnemyProjectile, GameObject>();

    private void Awake()
    {
        foreach (var e in prewarmed)
        {
            if (e.prefab == null) continue;
            for (int i = 0; i < e.prewarm; i++)
            {
                var inst = CreateNew(e.prefab);
                Store(e.prefab, inst);
                inst.gameObject.SetActive(false);
            }
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        EnemyProjectile inst = null;

        if (_free.TryGetValue(prefab, out var stack) && stack.Count > 0)
        {
            inst = stack.Pop();
            var go = inst.gameObject;
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(true);
            return go;
        }

        inst = CreateNew(prefab);
        var goNew = inst.gameObject;
        goNew.transform.SetPositionAndRotation(pos, rot);
        return goNew;
    }

    public void Despawn(EnemyProjectile proj)
    {
        if (proj == null) return;

        if (_origin.TryGetValue(proj, out var prefab))
        {
            proj.gameObject.SetActive(false);
            Store(prefab, proj);
        }
        else
        {
            Destroy(proj.gameObject);
        }
    }

    private EnemyProjectile CreateNew(GameObject prefab)
    {
        var go = Instantiate(prefab, transform);
        var proj = go.GetComponent<EnemyProjectile>();
        if (proj == null) proj = go.AddComponent<EnemyProjectile>();

        _origin[proj] = prefab;
        return proj;
    }

    private void Store(GameObject prefab, EnemyProjectile inst)
    {
        if (!_free.TryGetValue(prefab, out var stack))
        {
            stack = new Stack<EnemyProjectile>();
            _free[prefab] = stack;
        }
        stack.Push(inst);
    }
}
