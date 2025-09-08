using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Params")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float radius = 3.5f;
    [SerializeField] private float windupTime = 0.7f; // telegrafía
    [SerializeField] private float cooldown = 3.0f;
    [SerializeField] private bool stickToTargetAtCast = true;

    [Header("Refs")]
    [SerializeField] private Transform castOrigin; // si null, usa transform

    private float lastTime;
    private Coroutine castCo;

    public float Cooldown => cooldown;

    private void Reset()
    {
        castOrigin = transform;
    }

    public bool CanAttack()
    {
        return Time.time >= lastTime + cooldown && castCo == null;
    }

    public void DoAttack()
    {
        lastTime = Time.time;
        castCo = StartCoroutine(CastCo());
    }

    private IEnumerator CastCo()
    {
        Vector3 center = (castOrigin ? castOrigin.position : transform.position);

        // Si quieres que el círculo "siga" al target durante la telegrafía
        ITarget tgt = TargetRegistry.Instance?.CurrentTarget;
        float t = 0f;

        // TODO: Instanciar VFX telegráfico (círculo en el suelo)

        while (t < windupTime)
        {
            t += Time.deltaTime;
            if (stickToTargetAtCast && tgt != null && tgt.IsValid)
                center = tgt.AimRoot.position;
            yield return null;
        }

        // Daño en área
        var hits = Physics.OverlapSphere(center, radius);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(damage);

                // Vampírico (si existe en el caster)
                var vamp = GetComponent<EliteVampiric>();
                if (vamp != null) vamp.OnDealDamage(damage);
            }
        }

        // TODO: VFX/SFX de impacto
        castCo = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.35f);
        Vector3 c = (castOrigin ? castOrigin.position : transform.position);
        Gizmos.DrawSphere(c, radius);
    }
#endif
}
