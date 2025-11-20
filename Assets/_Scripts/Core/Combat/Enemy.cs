using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event System.Action<Enemy> OnEnemyDied;

    [Header("Stats")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Damage")]
    public int contactDamage = 10;

    [Header("Loot")]
    public GameObject lootPrefab;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Вызов обновления UI HP бара (будет сделано Викой)
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
