using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Provee una semilla de run y utilidades para obtener seeds reproducibles
/// por piso y por sala. Útil para mantener aleatoriedad consistente al reintentar.
/// </summary>
public class RunSeedProvider : MonoBehaviour
{
    public static RunSeedProvider Instance { get; private set; }

    [Header("Config")]
    [Tooltip("Si es true, usa 'fixedSeed' en lugar de generar una nueva.")]
    public bool useFixedSeed = false;

    [Tooltip("Semilla fija para depuración/repetibilidad.")]
    public int fixedSeed = 123456789;

    public int RunSeed { get; private set; }

    // Constantes de mezcla (primos/arbitrarios para minimizar colisiones)
    private const int FLOOR_MIX = 97;
    private const int ROOM_MIX = 131;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeRunSeed();
    }

    /// <summary>
    /// Inicializa la semilla de la run (fija o aleatoria).
    /// </summary>
    public void InitializeRunSeed()
    {
        RunSeed = useFixedSeed
            ? fixedSeed
            : UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Seed derivada para un piso concreto.
    /// </summary>
    public int GetSeedForFloor(int floorIndex)
    {
        unchecked
        {
            return RunSeed + floorIndex * FLOOR_MIX;
        }
    }

    /// <summary>
    /// Seed derivada para una sala dentro de un piso.
    /// </summary>
    public int GetSeedForRoom(int floorIndex, int roomIndex)
    {
        unchecked
        {
            return RunSeed + floorIndex * FLOOR_MIX + roomIndex * ROOM_MIX;
        }
    }

    /// <summary>
    /// Genera un System.Random con la seed de piso/sala.
    /// </summary>
    public System.Random GetRngFor(int seed) => new System.Random(seed);

    /// <summary>
    /// Atajo: RNG para piso.
    /// </summary>
    public System.Random GetRngForFloor(int floorIndex)
        => GetRngFor(GetSeedForFloor(floorIndex));

    /// <summary>
    /// Atajo: RNG para sala de un piso.
    /// </summary>
    public System.Random GetRngForRoom(int floorIndex, int roomIndex)
        => GetRngFor(GetSeedForRoom(floorIndex, roomIndex));
}
