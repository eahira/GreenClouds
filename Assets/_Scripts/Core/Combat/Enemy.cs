using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Room myRoom;
    private Transform player;

    private Vector2 desiredVelocity;
    private bool isDead = false;

    public event System.Action<Enemy> OnEnemyDied;

    [Header("Stats")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Tooltip("Радиус, в котором враг видит игрока")]
    public float aggroRange = 6f;

    [Tooltip("Слои стен/препятствий (обычно Wall)")]
    public LayerMask obstacleMask;

    [Header("Separation")]
    public float separationRadius = 0.8f;
    public float separationStrength = 2.0f;
    public LayerMask enemyMask;

    [Header("Damage")]
    public int contactDamage = 10;

    [Header("Attack")]
    public float attackCooldown = 0.7f;
    private float lastAttackTime = -999f;

    [Header("Loot Prefabs")]
    public GameObject coinPrefab;
    public GameObject healPrefab;
    public GameObject artifactPickupPrefab;

    [Header("Drop Chances")]
    [Range(0f, 1f)] public float coinChance = 1.0f;
    [Range(0f, 1f)] public float healChance = 0.12f;
    [Range(0f, 1f)] public float artifactChance = 0.08f;

    [Header("Artifact Pool")]
    public ArtifactData[] artifactPool;

    [Header("Drop Scatter")]
    public float dropScatterRadius = 0.6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // IMPORTANT: иногда myRoom может быть null при Awake (зависит от иерархии/спавна)
        myRoom = GetComponentInParent<Room>();

        if (obstacleMask.value == 0)
            obstacleMask = LayerMask.GetMask("Wall");

        if (enemyMask.value == 0)
            enemyMask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        // страховка: если Init не успел / игрок не проставился
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        // страховка: если вдруг не нашли комнату в Awake (часто это и даёт “стоячих”)
        if (myRoom == null)
            myRoom = GetComponentInParent<Room>();

        ComputeDesiredVelocity();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // двигаем физикой
        rb.MovePosition(rb.position + desiredVelocity * Time.fixedDeltaTime);
    }

    public void Init(Transform playerTransform)
    {
        player = playerTransform;

        float hpMul = 1f;
        float dmgMul = 1f;

        if (GameManager.Instance != null)
        {
            hpMul = GameManager.Instance.GetEnemyHealthMultiplier();
            dmgMul = GameManager.Instance.GetEnemyDamageMultiplier();
        }

        maxHealth = Mathf.RoundToInt(maxHealth * hpMul);
        currentHealth = maxHealth;

        contactDamage = Mathf.RoundToInt(contactDamage * dmgMul);
    }

    // ВАЖНО: задаём комнату снаружи (из EnemySpawner / RoomCombatController)
    public void SetRoom(Room room)
    {
        myRoom = room;
    }

    private void ComputeDesiredVelocity()
    {
        desiredVelocity = Vector2.zero;
        if (player == null) return;

        // Главный фикс "не агриться через комнаты":
        // если не в текущей комнате игрока — стоим
        if (CurrentRoomTracker.CurrentRoom != null)
        {
            // если myRoom не определился, лучше НЕ двигаться вообще (чтобы не бегали через стены)
            if (myRoom == null) return;

            if (CurrentRoomTracker.CurrentRoom != myRoom)
                return;
        }

        Vector2 toPlayer = (Vector2)(player.position - transform.position);
        float sqrDist = toPlayer.sqrMagnitude;

        if (sqrDist > aggroRange * aggroRange)
            return;

        Vector2 dir = toPlayer.normalized;
        float dist = Mathf.Sqrt(sqrDist);

        // Проверка препятствия (игнорируем trigger)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
        if (hit.collider != null && !hit.collider.isTrigger)
            return;

        Vector2 separation = GetSeparationVector();
        Vector2 finalDir = (dir + separation).normalized;

        desiredVelocity = finalDir * moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        EnemyEvents.OnEnemyHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            DropLoot();
            Die();
        }
    }

    private Vector3 GetDropPos(Vector3 center)
    {
        Vector2 offset = Random.insideUnitCircle * dropScatterRadius;
        return center + new Vector3(offset.x, offset.y, 0f);
    }

    private void DropLoot()
    {
        Vector3 pos = transform.position;

        if (coinPrefab != null && Random.value < coinChance)
            Instantiate(coinPrefab, GetDropPos(pos), Quaternion.identity);

        if (healPrefab != null && Random.value < healChance)
            Instantiate(healPrefab, GetDropPos(pos), Quaternion.identity);

        if (artifactPickupPrefab != null &&
            artifactPool != null &&
            artifactPool.Length > 0 &&
            Random.value < artifactChance)
        {
            var am = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;

            ArtifactData data = PickNonDuplicateArtifact(am);
            if (data == null) return;

            // помечаем при дропе, чтобы на полу тоже не дублировалось
            am?.MarkObtainedThisRun(data);

            var obj = Instantiate(artifactPickupPrefab, GetDropPos(pos), Quaternion.identity);
            var pickup = obj.GetComponent<ArtifactPickup>();
            if (pickup != null) pickup.data = data;
        }
    }

    private ArtifactData PickNonDuplicateArtifact(ArtifactManager am)
    {
        if (am == null)
            return artifactPool[Random.Range(0, artifactPool.Length)];

        List<ArtifactData> candidates = new List<ArtifactData>();

        foreach (var a in artifactPool)
        {
            if (a == null) continue;
            if (am.WasObtainedThisRun(a)) continue;
            candidates.Add(a);
        }

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    private void Die()
    {
        GameManager.Instance.OnEnemyKilled();
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHitPlayer(other);
    private void OnTriggerStay2D(Collider2D other) => TryHitPlayer(other);

    private void TryHitPlayer(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        lastAttackTime = Time.time;
        pc.TakeDamage(contactDamage);
    }

    private Vector2 GetSeparationVector()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyMask);

        Vector2 push = Vector2.zero;
        int count = 0;

        foreach (var c in cols)
        {
            if (c == null) continue;
            if (c.gameObject == gameObject) continue;

            Vector2 away = (Vector2)(transform.position - c.transform.position);
            float dist = away.magnitude;
            if (dist <= 0.0001f) continue;

            float t = 1f - Mathf.Clamp01(dist / separationRadius);
            push += away.normalized * t;
            count++;
        }

        if (count == 0) return Vector2.zero;

        push /= count;
        return push.normalized * (separationStrength * 0.35f);
    }
}
