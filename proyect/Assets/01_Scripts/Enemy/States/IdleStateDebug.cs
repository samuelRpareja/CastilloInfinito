using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateDebug : IState
{
    private readonly EnemyCommon enemy;
    private readonly float aggroRange;

    public IdleStateDebug(EnemyCommon enemy, float aggroRange)
    {
        this.enemy = enemy;
        this.aggroRange = aggroRange;
    }

    public void Enter() 
    { 
        Debug.Log($"IdleState: Entrando en estado Idle para {enemy.name}");
        
        // Verificar que el TargetRegistry existe
        if (TargetRegistry.Instance == null)
        {
            Debug.LogError($"IdleState: TargetRegistry.Instance es null para {enemy.name}");
        }
        else
        {
            var target = TargetRegistry.Instance.CurrentTarget;
            Debug.Log($"IdleState: TargetRegistry existe. Target actual: {(target != null ? target.ToString() : "null")}");
            if (target != null)
            {
                Debug.Log($"   - Target IsValid: {target.IsValid}");
                Debug.Log($"   - Target AimRoot: {(target.AimRoot != null ? target.AimRoot.name : "NULL")}");
                if (target.AimRoot != null)
                {
                    float dist = Vector3.Distance(enemy.transform.position, target.AimRoot.position);
                    Debug.Log($"   - Distancia al target: {dist:F2}");
                }
            }
        }
    }

    public void Tick(float dt)
    {
        var tgt = enemy.Target;
        if (tgt != null && tgt.IsValid)
        {
            float dist = Vector3.Distance(enemy.transform.position, tgt.AimRoot.position);
            if (dist <= aggroRange)
            {
                Debug.Log($"IdleState: Target detectado a distancia {dist:F1}, cambiando a GhostChaseState");
                // Cambiar a persecución específica de fantasmas
                enemy.fsm.Set(new GhostChaseState(enemy, aggroRange));
            }
        }
        else
        {
            // Debug ocasional para verificar que no hay target
            if (Time.frameCount % 300 == 0) // Cada 5 segundos aprox
            {
                Debug.Log($"IdleState: No hay target válido. Target: {(tgt != null ? "existe" : "null")}, Válido: {(tgt?.IsValid ?? false)}");
            }
        }
    }

    public void Exit() 
    { 
        Debug.Log($"IdleState: Saliendo del estado Idle para {enemy.name}");
    }
}
