using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    Transform AimRoot { get; }  // punto para apuntar/seguir
    bool IsValid { get; }       // vivo/activo
}