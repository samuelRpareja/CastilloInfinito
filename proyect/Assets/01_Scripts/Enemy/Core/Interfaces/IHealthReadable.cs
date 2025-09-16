using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthReadable 
{
    float MaxHP { get; }
    float CurrentHP { get; }

    event System.Action<float, float> OnHealthChanged;
}
    
