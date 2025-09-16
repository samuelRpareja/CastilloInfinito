using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDissolveEffect : MonoBehaviour
{
    [Header("Renderers del fantasma (los mismos del prefab original)")]
    [SerializeField] private SkinnedMeshRenderer[] meshRenderers;

    [Header("Shader param")]
    [SerializeField] private string dissolveProperty = "_Dissolve";

    [Header("Curva/tiempos")]
    [SerializeField] private float dissolveDuration = 1.2f; // cuánto tarda en disolverse
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private EnemyCommon enemy;
    private bool started;

    private void Awake()
    {
        enemy = GetComponent<EnemyCommon>();
    }

    private void OnEnable()
    {
        if (enemy != null)
            enemy.OnDeath += StartDissolve;
    }

    private void OnDisable()
    {
        if (enemy != null)
            enemy.OnDeath -= StartDissolve;
    }

    private void StartDissolve()
    {
        if (started) return;
        started = true;
        StartCoroutine(DissolveCo());
    }

    private IEnumerator DissolveCo()
    {
        float t = 0f;
        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float v = curve.Evaluate(Mathf.Clamp01(t / dissolveDuration)); // 1→0
            SetDissolve(v);
            yield return null;
        }
        SetDissolve(0f);

        // opcional: desactivar colisiones como hacía el script viejo
        var cc = GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
    }

    public void ResetDissolve(float value = 1f)
    {
        SetDissolve(value);
        started = false;
    }

    private void SetDissolve(float val)
    {
        if (meshRenderers == null) return;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            var mr = meshRenderers[i];
            if (mr == null) continue;
            foreach (var m in mr.materials)
            {
                if (m != null && m.HasProperty(dissolveProperty))
                    m.SetFloat(dissolveProperty, val);
            }
        }
    }
}
