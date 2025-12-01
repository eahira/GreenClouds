using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event System.Action<Enemy> OnEnemyDied;

    [Header("Stats")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    private Transform player;

    [Header("Damage")]
    public int contactDamage = 10;

    [Header("Loot")]
    public GameObject lootPrefab;

    private Rigidbody2D rb;  // Добавим Rigidbody для физики
    private Collider2D enemyCollider;  // Для коллайдера

    private void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();  // Инициализируем Rigidbody
        enemyCollider = GetComponent<Collider2D>();  // Инициализируем Collider
    }

    private void Start()
    {
        if (player != null)
        {
            Init(player); // Передаем ссылку на игрока
        }
    }

    private void Update()
    {
        if (player == null) return;

        MoveTowardsPlayer();
    }

    /// <summary>
    /// Инициализация врага с привязкой к игроку
    /// </summary>
    public void Init(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // Направление в сторону игрока
        Vector2 direction = (player.position - transform.position).normalized;

        // Проверка на препятствия перед движением
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Obstacles"));
        if (hit.collider == null)
        {
            // Двигаем врага
            rb.linearVelocity = direction * moveSpeed;  // Используем velocity для движения
        }
        else
        {
            // Если столкновение, останавливаем движение
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Обновляем UI HP (если требуется)
        EnemyEvents.OnEnemyHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            DropLoot();
            Die();
        }
    }

    void DropLoot()
    {
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }
    }

    void Die()
    {
        GameManager.Instance.OnEnemyKilled();
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(contactDamage);
        }
    }

    // Для отладки: визуализация луча, показывающего путь
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
