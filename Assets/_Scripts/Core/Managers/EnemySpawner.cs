using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    public List<Enemy> SpawnEnemies(int count)
    {
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject obj = Instantiate(enemyPrefab, point.position, Quaternion.identity);
            Enemy enemy = obj.GetComponent<Enemy>();

            enemies.Add(enemy);
        }

        return enemies;
    }
}
