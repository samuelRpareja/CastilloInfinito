using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironmentQuery
{
    bool TryGetCover(Vector3 from, out Vector3 coverPos);
}
