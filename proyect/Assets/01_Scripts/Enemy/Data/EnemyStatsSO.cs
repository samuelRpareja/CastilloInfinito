using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Enemy Stats", fileName = "EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Básicos")]
    public float maxHP = 30f;
    public float moveSpeed = 3f;
    public float damage = 6f;

    [Header("Rangos")]
    public float aggroRange = 10f;
    public float attackRange = 1.5f;

    [Header("Ataque")]
    public float fireRate = 1.0f; // disparos/seg o ataques/seg

    [Header("Recompensas")]
    public int xpReward = 5;
    public int goldReward = 2;
}
