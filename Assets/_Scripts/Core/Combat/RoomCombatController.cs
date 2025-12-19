using System.Collections.Generic;
using UnityEngine;

public class RoomCombatController : MonoBehaviour
{
    [Header("Spawner")]
    public EnemySpawner enemySpawner;

    [Header("Boss Prefabs per Stage (1..3)")]
    public GameObject bossStage1Prefab;
    public GameObject bossStage2Prefab;
    public GameObject bossStage3Prefab;

    [Header("Enemies Count")]
    public int baseMinEnemies = 3;
    public int baseMaxEnemies = 6;

    [Header("Flags")]
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

        // по твоему ТЗ: пока НЕ зачищено — порталы и блокеры OFF
        if (room != null)
            room.SetCleared(false);

        if (isBossRoom) SpawnBoss();
        else SpawnRegularEnemies();

        if (enemies.Count == 0 && room != null)
            room.SetCleared(true);
    }

    void SpawnRegularEnemies()
    {
        if (enemySpawner == null) return;

        int min = Mathf.Max(1, baseMinEnemies);
        int max = Mathf.Max(min, baseMaxEnemies);

        float countMul = 1f;

        if (GameManager.Instance != null)
        {
            countMul *= GameManager.Instance.GetEnemyCountMultiplier();

            switch (GameManager.Instance.CurrentStage)
            {
                case 2: countMul *= 1.3f; break;
                case 3: countMul *= 1.6f; break;
            }
        }

        min = Mathf.Max(1, Mathf.RoundToInt(min * countMul));
        max = Mathf.Max(min, Mathf.RoundToInt(max * countMul));

        int count = Random.Range(min, max + 1);

        var spawned = enemySpawner.SpawnEnemies(count);
        RegisterEnemies(spawned);
    }

    void SpawnBoss()
    {
        GameObject prefab = GetBossPrefabForCurrentStage();
        if (prefab == null)
        {
            Debug.LogWarning("RoomCombatController: boss prefab is not assigned for this stage");
            return;
        }

        Vector3 pos = enemySpawner != null ? enemySpawner.transform.position : transform.position;
        GameObject bossObj = Instantiate(prefab, pos, Quaternion.identity, transform);

        Enemy bossEnemy = bossObj.GetComponent<Enemy>();
        if (bossEnemy != null)
        {
            if (playerTransform != null)
                bossEnemy.Init(playerTransform);

            RegisterEnemy(bossEnemy);
        }
    }

    GameObject GetBossPrefabForCurrentStage()
    {
        int stage = GameManager.Instance != null ? GameManager.Instance.CurrentStage : 1;
        if (stage == 2) return bossStage2Prefab != null ? bossStage2Prefab : bossStage1Prefab;
        if (stage == 3) return bossStage3Prefab != null ? bossStage3Prefab : bossStage1Prefab;
        return bossStage1Prefab;
    }

    void RegisterEnemies(List<Enemy> list)
    {
        if (list == null) return;
        foreach (var e in list) RegisterEnemy(e);
    }

    void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        enemies.Add(enemy);
        enemy.OnEnemyDied += HandleEnemyDied;

        if (room != null)
            enemy.SetRoom(room);
    }

    void HandleEnemyDied(Enemy enemy)
    {
        if (enemy == null) return;

        enemies.Remove(enemy);
        if (enemies.Count > 0) return;

        IsCleared = true;

        // когда зачищено — порталы и блокеры ON
        if (room != null)
            room.SetCleared(true);

        if (isBossRoom && GameManager.Instance != null)
            GameManager.Instance.LevelCompleted();
    }

    private void OnDestroy()
    {
        foreach (var e in enemies)
            if (e != null) e.OnEnemyDied -= HandleEnemyDied;
    }
}
