using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Prefabs (random)")]
    public GameObject[] enemyPrefabs;

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

        if (obstacleMask.value == 0)
            obstacleMask = LayerMask.GetMask("Wall");
    }

    public List<Enemy> SpawnEnemies(int count)
    {
        List<Enemy> enemies = new List<Enemy>();

        if (spawnPoints == null || spawnPoints.Length == 0)
            return enemies;

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return enemies;

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPos = FindFreeSpawnPos(point.position);

            GameObject prefab = PickEnemyPrefab();
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null && player != null)
                enemy.Init(player);

            if (enemy != null && myRoom != null)
                enemy.SetRoom(myRoom);

            enemies.Add(enemy);
        }

        return enemies;
    }

    private GameObject PickEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    private Vector3 FindFreeSpawnPos(Vector3 center)
    {
        for (int attempt = 0; attempt < maxAttemptsPerEnemy; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnJitterRadius;
            Vector3 candidate = center + new Vector3(offset.x, offset.y, 0f);

            if (Physics2D.OverlapCircle(candidate, minSpawnDistance, enemyMask) != null)
                continue;

            if (obstacleMask.value != 0)
            {
                if (Physics2D.OverlapCircle(candidate, 0.45f, obstacleMask) != null)
                    continue;
            }

            return candidate;
        }

        return center + new Vector3(0.5f, 0f, 0f);
    }
}
