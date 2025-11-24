using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("Player")]
    public Transform player;  // сюда в инспекторе перетащишь Player

    public List<Enemy> SpawnEnemies(int count)
    {
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < count; i++)
        {
            // если вдруг нет точек спавна или префаба — выходим
            if (spawnPoints == null || spawnPoints.Length == 0 || enemyPrefab == null)
                break;

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject obj = Instantiate(enemyPrefab, point.position, Quaternion.identity);
            Enemy enemy = obj.GetComponent<Enemy>();

            if (enemy != null && player != null)
            {
                enemy.Init(player);
            }

            enemies.Add(enemy);
        }

        return enemies;
    }
}
