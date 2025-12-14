using System.Collections.Generic;
using UnityEngine;

public class RoomCombatController : MonoBehaviour
{
    [Header("Spawner и босс для этой комнаты")]
    public EnemySpawner enemySpawner;
    public GameObject bossPrefab;

    [Header("Количество обычных врагов (базовое)")]
    public int baseMinEnemies = 3;
    public int baseMaxEnemies = 6;

    [Header("Флаги")]
    public bool isBossRoom = false;

    private readonly List<Enemy> enemies = new List<Enemy>();
    private Room room;
    private Transform playerTransform;

    public bool IsCleared { get; private set; } = false;

    private void Awake()
    {
        room = GetComponent<Room>();
    }

    public void Setup(Transform playerTransform, bool bossRoom)
    {
        isBossRoom = bossRoom;
        IsCleared = false;
        this.playerTransform = playerTransform;

        if (enemySpawner != null)
            enemySpawner.player = playerTransform;

        if (room != null)
            room.SetPortalsActive(false);

        if (isBossRoom)
            SpawnBoss();
        else
            SpawnRegularEnemies();
    }

    void SpawnRegularEnemies()
    {
        if (enemySpawner == null) return;

        int min = Mathf.Max(1, baseMinEnemies);
        int max = Mathf.Max(min, baseMaxEnemies);

        // базовый множитель количества врагов
        float countMul = 1f;

        if (GameManager.Instance != null)
        {
            // 1) множитель от сложности (Easy/Medium/Hard)
            countMul *= GameManager.Instance.GetEnemyCountMultiplier();

            // 2) множитель от ЭТАПА героя (1 / 2 / 3)
            switch (GameManager.Instance.CurrentStage)
            {
                case 2:
                    countMul *= 1.3f; // на 2-м уровне чуть больше врагов
                    break;
                case 3:
                    countMul *= 1.6f; // на 3-м ещё больше
                    break;
                // case 1: ничего не умножаем
            }
        }

        // применяем множитель к мин/макс количеству
        min = Mathf.Max(1, Mathf.RoundToInt(min * countMul));
        max = Mathf.Max(min, Mathf.RoundToInt(max * countMul));

        int count = Random.Range(min, max + 1);

        var spawned = enemySpawner.SpawnEnemies(count);
        RegisterEnemies(spawned);
    }



    void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("RoomCombatController: bossPrefab не назначен, босс не заспавнится");
            return;
        }

        Vector3 pos = enemySpawner != null ? enemySpawner.transform.position : transform.position;
        GameObject bossObj = Object.Instantiate(bossPrefab, pos, Quaternion.identity, transform);

        Enemy bossEnemy = bossObj.GetComponent<Enemy>();
        if (bossEnemy != null)
        {
            if (playerTransform != null)
                bossEnemy.Init(playerTransform);

            RegisterEnemy(bossEnemy);
        }
    }

    void RegisterEnemies(List<Enemy> list)
    {
        if (list == null) return;
        foreach (var e in list)
            RegisterEnemy(e);
    }

    void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        enemies.Add(enemy);
        enemy.OnEnemyDied += HandleEnemyDied;
    }

    void HandleEnemyDied(Enemy enemy)
    {
        if (enemy == null) return;

        enemies.Remove(enemy);

        if (enemies.Count > 0)
            return;

        IsCleared = true;
        Debug.Log($"Комната {name} зачищена");

        if (room != null)
            room.SetPortalsActive(true);

        if (isBossRoom && GameManager.Instance != null)
        {
            Debug.Log("Босс побеждён, уровень пройден");
            GameManager.Instance.LevelCompleted();
        }
    }

    private void OnDestroy()
    {
        foreach (var e in enemies)
        {
            if (e != null)
                e.OnEnemyDied -= HandleEnemyDied;
        }
    }
}
