using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Comprueba línea de visión (line of sight) con un target usando raycast.
/// Para ahorrar rendimiento, la comprobación se hace cada X segundos
/// en lugar de cada frame.
/// </summary>
public class LOSChecker : MonoBehaviour
{
    [SerializeField] private Transform eye;           // origen del raycast (si null, usa transform)
    [SerializeField] private float checkInterval = 0.25f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    private float lastCheck;
    private bool lastResult;

    private void Reset()
    {
        eye = transform;
    }

    /// <summary>
    /// Devuelve true si hay visión clara al target.
    /// </summary>
    public bool HasLOS(Transform target)
    {
        if (target == null) return false;

        if (Time.time < lastCheck + checkInterval) return lastResult;

        lastCheck = Time.time;

        Vector3 from = eye ? eye.position : transform.position;
        Vector3 to = target.position;
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        if (Physics.Raycast(from, dir.normalized, dist, obstacleMask))
        {
            lastResult = false;
        }
        else
        {
            lastResult = true;
        }

        return lastResult;
    }
}
