using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estado de persecución específico para fantasmas que solo se mueve en X y Z
/// </summary>
public class GhostChaseState : IState
{
    private readonly EnemyCommon enemy;
    private readonly float aggroRange;

    public GhostChaseState(EnemyCommon enemy, float aggroRange)
    {
        this.enemy = enemy;
        this.aggroRange = aggroRange;
    }

    public void Enter() 
    { 
        Debug.Log($"GhostChaseState: {enemy.name} comenzando persecución");
        
        var tgt = enemy.Target;
        if (tgt != null && tgt.IsValid)
        {
            float dist = Vector3.Distance(enemy.transform.position, tgt.AimRoot.position);
            Debug.Log($"GhostChaseState: Target válido a distancia {dist:F1}");
        }
        else
        {
            Debug.LogWarning($"GhostChaseState: No hay target válido al entrar en persecución");
        }
    }

    public void Tick(float dt)
    {
        var tgt = enemy.Target;
        if (tgt == null || !tgt.IsValid)
        {
            enemy.fsm.Set(new IdleStateDebug(enemy, aggroRange));
            return;
        }

        // Calcular dirección solo horizontal (X y Z)
        Vector3 targetPos = tgt.AimRoot.position;
        Vector3 currentPos = enemy.transform.position;
        
        // Forzar altura fija del fantasma (sin movimiento vertical)
        float fixedHeight = currentPos.y; // Mantener altura actual
        targetPos.y = fixedHeight;
        
        // Verificar límites de habitación - pero seguir persiguiendo al target
        var roomBounds = enemy.GetComponent<GhostRoomBounds>();
        if (roomBounds != null)
        {
            // Si el target está fuera de los límites, perseguir de todas formas
            // pero mantener al ghost dentro de los límites
            if (!roomBounds.IsPositionInBounds(targetPos))
            {
                // Perseguir al target pero limitar la posición del ghost
                Vector3 dirToTarget = (targetPos - currentPos);
                enemy.MoveTowards(dirToTarget, dt);
                
                // Aplicar límites después del movimiento
                Vector3 limitedPos = enemy.transform.position;
                limitedPos = roomBounds.ClampPositionToBounds(limitedPos);
                limitedPos.y = fixedHeight; // Mantener altura fija
                enemy.transform.position = limitedPos;
                
                // Debug ocasional
                if (Time.frameCount % 300 == 0)
                {
                    Debug.Log($"GhostChaseState: {enemy.name} persiguiendo target fuera de límites - Posición limitada: {limitedPos}");
                }
                
                return; // Salir después de aplicar límites
            }
        }
        
        Vector3 dir = (targetPos - currentPos);
        float dist = dir.magnitude;

        // Perseguir solo en plano horizontal
        enemy.MoveTowards(dir, dt);
        
        // FORZAR altura fija después del movimiento
        Vector3 newPos = enemy.transform.position;
        newPos.y = fixedHeight;
        enemy.transform.position = newPos;
        
        // Debug ocasional para verificar que está persiguiendo
        if (Time.frameCount % 300 == 0) // Cada 5 segundos aprox
        {
            Debug.Log($"GhostChaseState: {enemy.name} persiguiendo a distancia {dist:F1}");
        }

        // Si está muerto o demasiado lejos → volver a Idle
        if (dist > aggroRange * 1.5f)
        {
            enemy.fsm.Set(new IdleStateDebug(enemy, aggroRange));
            return;
        }

        // Si puede atacar, cambiar a AttackState
        var attack = enemy.GetComponent<IEnemyAttack>();
        if (attack != null)
        {
            if (attack.CanAttack())
            {
                Debug.Log($"<color=yellow>[GhostChaseState] {enemy.name} puede atacar - Cambiando a AttackState</color>");
                enemy.fsm.Set(new AttackState(enemy, aggroRange, attack));
            }
        }
        else
        {
            Debug.LogWarning($"   ⚠️ No se encontró componente IEnemyAttack en {enemy.name} - Agregando SimpleGhostAttack");
            // Agregar SimpleGhostAttack automáticamente
            var simpleAttack = enemy.gameObject.AddComponent<SimpleGhostAttack>();
            if (simpleAttack.CanAttack())
            {
                Debug.Log($"   ✅ SimpleGhostAttack puede atacar - Cambiando a AttackState");
                enemy.fsm.Set(new AttackState(enemy, aggroRange, simpleAttack));
            }
        }
    }

    public void Exit() 
    { 
        Debug.Log($"GhostChaseState: {enemy.name} terminando persecución");
    }
}
