using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEliteAffix
{
    void Apply(GameObject enemy);
    string Name { get; }
}
