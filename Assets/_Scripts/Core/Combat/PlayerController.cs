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

    [Header("Ghost Shield")]
    public float ghostShieldHpThreshold = 0.3f; // 30% HP
    private bool ghostShieldReady = true;
    private bool ghostShieldActive = false;

    private ArtifactManager artifactManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (ultimateSystem == null)
            ultimateSystem = GetComponent<UltimateSystem>();

        artifactManager = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;
    }

    private void CheckGhostShield()
    {
        if (artifactManager == null) return;
        if (!artifactManager.HasArtifact(ArtifactEffectType.GhostShield)) return;

        float hpPercent = (maxHealth > 0) ? (float)currentHealth / maxHealth : 1f;

        if (hpPercent <= ghostShieldHpThreshold && ghostShieldReady)
        {
            ghostShieldActive = true;
            ghostShieldReady = false;
            Debug.Log("Ghost Shield activated!");
        }

        if (hpPercent > ghostShieldHpThreshold)
            ghostShieldReady = true;
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

            // ���� ����� "�������" ���� ����� � ����� �������
            float radius = 0.2f;
            Collider2D hit = Physics2D.OverlapCircle(mousePos, radius, enemyLayer);

            if (hit != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int dmg = clickDamage;

                    var effects = GetComponent<ArtifactEffectSystem>();
                    if (effects != null)
                        dmg = effects.ModifyClickDamage(clickDamage);

                    enemy.TakeDamage(dmg);

                    ultimateSystem?.AddCharge(1);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // 🛡️ GhostShield: если активен — блокируем весь урон, но оставляем визуал/ивенты как надо
        if (ghostShieldActive)
        {
            ghostShieldActive = false;

            // Можно показать "0" чтобы игрок понял что удар был заблокирован
            FloatingDamageText.Spawn(transform.position + Vector3.up * 1.0f, 0);

            // ХП не меняется, но можно все равно дернуть эвент, чтобы UI не рассинхронизировался
            PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

            Debug.Log("Ghost Shield absorbed damage!");
            return;
        }

        currentHealth -= damage;

        PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        // Показать урон над игроком
        FloatingDamageText.Spawn(transform.position + Vector3.up * 1.0f, damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Проверяем, надо ли включить щит по порогу HP
        CheckGhostShield();
    }




    void Die()
    {
        GameManager.Instance.PlayerDied();
        gameObject.SetActive(false);
    }
}

