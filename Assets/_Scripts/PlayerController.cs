using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 movement;

    [Header("Combat")]
    public UltimateSystem ultimateSystem;
    public int clickDamage = 1;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (ultimateSystem == null)
            ultimateSystem = GetComponent<UltimateSystem>();
    }

    void Update()
    {
        // Обработка движения
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Обработка кликов (отдельно от движения)
        CheckForEnemyClick();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        // Движение в FixedUpdate для плавности
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Резервный вариант если нет Rigidbody
            transform.position += (Vector3)movement.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void CheckForEnemyClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Наносим урон врагу
                    enemy.TakeDamage(clickDamage);

                    // Заряжаем ультимейт
                    if (ultimateSystem != null)
                        ultimateSystem.AddCharge(1);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Игрок получил урон! Здоровье: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("Игрок умер! Конец забега");
        gameObject.SetActive(false);
    }
}