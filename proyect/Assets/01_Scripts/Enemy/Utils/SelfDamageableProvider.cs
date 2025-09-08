using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implementación simple de IDamageableProvider que devuelve
/// el IDamageable del mismo GameObject o de sus padres.
/// Útil para UI como EnemyHealthBar.
/// </summary>
public class SelfDamageableProvider : MonoBehaviour, IDamageableProvider
{
    public IDamageable GetDamageable()
    {
        return GetComponentInParent<IDamageable>();
    }
}
