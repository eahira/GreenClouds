using System.Collections.Generic;
using UnityEngine;

public class RoomCombatController : MonoBehaviour
{
    [Header("Spawner и босс для этой комнаты")]
    public EnemySpawner enemySpawner;   // перетяни из префаба Room
    public GameObject bossPrefab;       // префаб босса (можно оставить пустым, если нет босса)

    [Header("Количество обычных врагов (базовое)")]
    public int baseMinEnemies = 3;
    public int baseMaxEnemies = 6;

    [Header("Флаги")]
    public bool isBossRoom = false;

    private readonly List<Enemy> enemies = new List<Enemy>();
    private Room room;
    public bool IsCleared { get; private set; } = false;

    private void Awake()
    {
        room = GetComponent<Room>();
    }

    /// <summary>
    /// Вызывается RoomManager-ом после генерации уровня.
    /// Настраиваем спавн врагов и закрываем порталы.
    /// </summary>
    public void Setup(Transform playerTransform, bool bossRoom)
    {
        isBossRoom = bossRoom;
        IsCleared = false;

        if (enemySpawner != null)
            enemySpawner.player = playerTransform;

        // перед боем порталы выключены — из комнаты нельзя выйти
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

    float countMul = 1f;
    if (GameManager.Instance != null)
        countMul = GameManager.Instance.GetEnemyCountMultiplier();

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
        GameObject bossObj = Object.Instantiate(bossPrefab, pos, Quaternion.identity);

        Enemy bossEnemy = bossObj.GetComponent<Enemy>();
        if (bossEnemy != null)
            RegisterEnemy(bossEnemy);
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

        enemy.OnEnemyDied -= HandleEnemyDied;
        enemies.Remove(enemy);

        if (enemies.Count > 0)
            return; // ещё не все умерли

        // все враги в комнате мертвы
        IsCleared = true;
        Debug.Log($"Комната {name} зачищена");

        // открываем порталы из комнаты
        if (room != null)
            room.SetPortalsActive(true);

        if (isBossRoom)
        {
            Debug.Log("👑 Босс побеждён, уровень пройден");
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
