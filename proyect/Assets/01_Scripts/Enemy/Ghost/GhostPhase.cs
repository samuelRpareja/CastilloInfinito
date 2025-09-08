using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPhase : MonoBehaviour
{
    [SerializeField] private float phaseTime = 0.8f;
    [SerializeField] private float cooldown = 4f;
    private float last;
    private int originalLayer;
    private Renderer[] rends;

    public bool CanPhase => Time.time >= last + cooldown;

    private void Awake()
    {
        originalLayer = gameObject.layer;
        rends = GetComponentsInChildren<Renderer>();
    }

    public void DoPhase()
    {
        if (!CanPhase) return;
        last = Time.time;
        StartCoroutine(PhaseCo());
    }

    private IEnumerator PhaseCo()
    {
        gameObject.layer = LayerMask.NameToLayer("GhostPhase");
        SetAlpha(0.4f);
        yield return new WaitForSeconds(phaseTime);
        SetAlpha(1f);
        gameObject.layer = originalLayer;
    }

    private void SetAlpha(float a)
    {
        foreach (var r in rends)
        {
            foreach (var m in r.materials)
            {
                if (m.HasProperty("_Color"))
                {
                    var c = m.color; c.a = a; m.color = c;
                }
            }
        }
    }
}
