using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Boss Config", fileName = "BossConfig")]
public class BossConfigSO : ScriptableObject
{
    [Header("Multiplicadores de dificultad")]
    public float hpMultiplier = 3f;
    public float damageMultiplier = 1.5f;

    [Header("Fases")]
    public int phaseThreshold = 50; // % HP para cambiar de fase
    public float phaseDelay = 1.0f; // segundos de pausa entre fases

    [Header("Recompensas")]
    public int goldReward = 20;
    public int xpReward = 50;

    [Header("Otros")]
    public AudioClip introSFX;
    public AudioClip deathSFX;
}