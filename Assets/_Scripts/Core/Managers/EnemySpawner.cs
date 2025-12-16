using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("Player")]
    public Transform player;

    [Header("Spawn Anti-Stack")]
    public float spawnJitterRadius = 1.2f;
    public float minSpawnDistance = 0.9f;
    public int maxAttemptsPerEnemy = 12;

    public LayerMask enemyMask;
    public LayerMask obstacleMask;

    private Room myRoom;

    private void Awake()
    {
        myRoom = GetComponentInParent<Room>();

        if (enemyMask.value == 0)
            enemyMask = LayerMask.GetMask("Enemy");

        // чтобы не забывали в инспекторе
        if (obstacleMask.value == 0)
            obstacleMask = LayerMask.GetMask("Wall");
    }

    public List<Enemy> SpawnEnemies(int count)
    {
        List<Enemy> enemies = new List<Enemy>();

        if (spawnPoints == null || spawnPoints.Length == 0 || enemyPrefab == null)
            return enemies;

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPos = FindFreeSpawnPos(point.position);

            GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
            Enemy enemy = obj.GetComponent<Enemy>();

            if (enemy != null && player != null)
                enemy.Init(player);

            if (enemy != null && myRoom != null)
                enemy.SetRoom(myRoom);

            enemies.Add(enemy);
        }

        return enemies;
    }

    private Vector3 FindFreeSpawnPos(Vector3 center)
    {
        for (int attempt = 0; attempt < maxAttemptsPerEnemy; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnJitterRadius;
            Vector3 candidate = center + new Vector3(offset.x, offset.y, 0f);

            // не ставим слишком близко к другим врагам
            if (Physics2D.OverlapCircle(candidate, minSpawnDistance, enemyMask) != null)
                continue;

            // не ставим рядом/в стене
            if (obstacleMask.value != 0)
            {
                if (Physics2D.OverlapCircle(candidate, 0.45f, obstacleMask) != null)
                    continue;
            }

            return candidate;
        }

        // fallback: чуть сдвигаем, чтобы не оказаться ровно в стене
        return center + new Vector3(0.5f, 0f, 0f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (spawnPoints != null)
        {
            foreach (var sp in spawnPoints)
            {
                if (sp == null) continue;
                Gizmos.DrawWireSphere(sp.position, spawnJitterRadius);
            }
        }
    }
#endif
}
