using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner enemySpawner;

    [Header("Room Settings")]
    public int minEnemies = 3;
    public int maxEnemies = 6;

    [Header("Events")]
    public UnityEvent OnRoomCleared;

    private List<Enemy> aliveEnemies = new List<Enemy>();
    private bool roomStarted = false;
    private bool roomCleared = false;

    void Start()
    {
        StartRoom();
    }

    public void StartRoom()
    {
        if (roomStarted) return;

        roomStarted = true;

        int count = Random.Range(minEnemies, maxEnemies + 1);

        // Спавним врагов
        aliveEnemies = enemySpawner.SpawnEnemies(count);

        // Подписываемся на смерть каждого  
        foreach (Enemy e in aliveEnemies)
        {
            e.OnEnemyDied += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);

        if (aliveEnemies.Count == 0 && !roomCleared)
        {
            roomCleared = true;
            Debug.Log("Комната очищена!");
            OnRoomCleared?.Invoke();
        }
    }
}
