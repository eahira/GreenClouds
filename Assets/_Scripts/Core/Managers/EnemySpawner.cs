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
    [Tooltip("–адиус, в котором враги могут раскидыватьс€ вокруг спавнпоинта")]
    public float spawnJitterRadius = 1.2f;

    [Tooltip("ћинимальна€ дистанци€ между врагами при спавне (чтобы не накладывались)")]
    public float minSpawnDistance = 0.9f;

    [Tooltip("—колько попыток подобрать свободное место на одного врага")]
    public int maxAttemptsPerEnemy = 12;

    [Tooltip("—лой врагов (чтобы OverlapCircle провер€л других врагов)")]
    public LayerMask enemyMask;

    [Tooltip("ќпционально: слой стен/преп€тствий, чтобы не спавнить в стене")]
    public LayerMask obstacleMask;

    private void Awake()
    {
        // если не настроили в инспекторе Ч подстрахуемс€
        if (enemyMask.value == 0)
            enemyMask = LayerMask.GetMask("Enemy");
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

            enemies.Add(enemy);
        }

        return enemies;
    }

    private Vector3 FindFreeSpawnPos(Vector3 center)
    {
        // 1) пытаемс€ найти свободное место
        for (int attempt = 0; attempt < maxAttemptsPerEnemy; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnJitterRadius;
            Vector3 candidate = center + new Vector3(offset.x, offset.y, 0f);

            // не ставим слишком близко к другим врагам
            bool tooCloseToEnemy = Physics2D.OverlapCircle(candidate, minSpawnDistance, enemyMask) != null;
            if (tooCloseToEnemy) continue;

            // опционально: не ставим в стене/преп€тствии
            if (obstacleMask.value != 0)
            {
                bool inWall = Physics2D.OverlapCircle(candidate, 0.2f, obstacleMask) != null;
                if (inWall) continue;
            }

            return candidate;
        }

        // 2) если не нашли Ч возвращаем центр (fallback)
        return center;
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
