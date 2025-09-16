using UnityEngine;

public class EnemyAutoInit : MonoBehaviour
{
    void Start()
    {
        var arr = GetComponentsInChildren<IInitializable>();
        for (int i = 0; i < arr.Length; i++) arr[i].Initialize();
    }
}
