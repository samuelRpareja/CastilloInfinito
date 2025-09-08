using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCoverSeeker : MonoBehaviour
{
    [SerializeField] private float coverCheckInterval = 2f;
    [SerializeField] private float minDistance = 6f;
    [SerializeField] private float maxDistance = 12f;

    private float lastCheck;
    private EnemyCommon enemy;
    private IEnvironmentQuery query;

    private void Awake()
    {
        enemy = GetComponent<EnemyCommon>();
        query = GetComponent<IEnvironmentQuery>();
    }

    private void Update()
    {
        if (Time.time < lastCheck + coverCheckInterval) return;
        lastCheck = Time.time;

        if (query != null && query.TryGetCover(enemy.transform.position, out var coverPos))
        {
            Vector3 dir = coverPos - enemy.transform.position;
            enemy.MoveTowards(dir, Time.deltaTime);
        }
    }
}
