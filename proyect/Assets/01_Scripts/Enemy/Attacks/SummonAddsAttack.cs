using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonAddsAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Params")]
    [SerializeField] private float cooldown = 6.0f;
    [SerializeField] private int maxAliveAdds = 3;
    [SerializeField] private int summonCount = 2;
    [SerializeField] private float spawnRadius = 4f;

    [Header("Refs")]
    [SerializeField] private GameObject[] addPrefabs; // enemigos a invocar
    [SerializeField] private Transform origin;

    private float lastTime;
    private int aliveAdds;

    public float Cooldown => cooldown;

    private void Reset()
    {
        origin = transform;
    }

    public bool CanAttack()
    {
        return Time.time >= lastTime + cooldown && aliveAdds < maxAliveAdds && addPrefabs != null && addPrefabs.Length > 0;
    }

    public void DoAttack()
    {
        lastTime = Time.time;

        int toSummon = Mathf.Min(summonCount, maxAliveAdds - aliveAdds);
        for (int i = 0; i < toSummon; i++)
        {
            var pf = addPrefabs[Random.Range(0, addPrefabs.Length)];
            Vector3 pos = (origin ? origin.position : transform.position) + Random.insideUnitSphere * spawnRadius;
            pos.y = (origin ? origin.position.y : transform.position.y);

            var go = Instantiate(pf, pos, Quaternion.identity);

            // Inicializar si corresponde
            var inits = go.GetComponents<IInitializable>();
            foreach (var init in inits) init.Initialize();

            aliveAdds++;

            // Suscribirse a OnDeath para llevar conteo
            var dmg = go.GetComponent<IDamageable>();
            if (dmg != null) dmg.OnDeath += () => aliveAdds--;
        }

        // TODO: VFX/SFX de invocación
    }

    public float GetAttackDuration()
    {
        throw new System.NotImplementedException();
    }
}
