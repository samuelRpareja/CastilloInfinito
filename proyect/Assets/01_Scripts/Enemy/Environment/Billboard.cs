using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Alinea el forward del objeto hacia la cámara (para UI world-space).
/// Úsalo en barras de vida, indicadores, etc.
/// </summary>
public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform target; // si es null, usa Camera.main
    private Camera _cam;

    private void OnEnable()
    {
        if (target == null) _cam = Camera.main;
    }

    private void LateUpdate()
    {
        var t = target != null ? target : (_cam != null ? _cam.transform : null);
        if (t == null) return;
        transform.forward = (transform.position - t.position).normalized;
    }
}
