using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implementación simple de IEnvironmentQuery:
/// busca posiciones con tag "Cover" dentro de un rango y
/// elige la más cercana que esté entre min/max.
/// </summary>
public class SimpleCoverQuery : MonoBehaviour, IEnvironmentQuery
{
    [SerializeField] private string coverTag = "Cover";
    [SerializeField] private float minDistance = 6f;
    [SerializeField] private float maxDistance = 12f;

    public bool TryGetCover(Vector3 from, out Vector3 coverPos)
    {
        var covers = GameObject.FindGameObjectsWithTag(coverTag);
        coverPos = from;
        float best = Mathf.Infinity;

        for (int i = 0; i < covers.Length; i++)
        {
            var cpos = covers[i].transform.position;
            float d = Vector3.Distance(from, cpos);
            if (d < minDistance || d > maxDistance) continue;

            if (d < best)
            {
                best = d;
                coverPos = cpos;
            }
        }

        return best < Mathf.Infinity;
    }
}
