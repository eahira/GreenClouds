using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask enemyLayer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int clickDamage = 1;

    [Header("Ultimate")]
    public UltimateSystem ultimateSystem;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (ultimateSystem == null)
            ultimateSystem = GetComponent<UltimateSystem>();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        CheckForEnemyClick();
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        else
            transform.position += (Vector3)movement.normalized * moveSpeed * Time.deltaTime;
    }

    void CheckForEnemyClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast ÒÎËÜÊÎ ןמ גנאדאל
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0.1f, enemyLayer);

            if (hit.collider != null)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(clickDamage);
                    ultimateSystem?.AddCharge(1);
                }
            }
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        GameManager.Instance.PlayerDied();
        gameObject.SetActive(false);
    }
}
