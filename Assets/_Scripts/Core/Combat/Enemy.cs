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

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        MoveTowardsPlayer();
    }

    /// <summary>
    /// Вызывается после спавна врага, чтобы передать ему ссылку на игрока
    /// </summary>
    public void Init(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Обновление UI HP врага (если будет нужно)
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
}
