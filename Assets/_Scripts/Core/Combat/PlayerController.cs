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
    [Range(0.05f, 0.95f)]
    public float ghostShieldHpThreshold = 0.3f;
    private bool ghostShieldReady = true;
    private bool ghostShieldActive = false;

    [Header("Blessing Regen (only when idle)")]
    public float blessingRegenPerSecond = 1.0f;
    public float blessingIdleSpeedThreshold = 0.08f;
    public float blessingStartDelay = 0.25f;
    private float blessingIdleTimer = 0f;
    private float blessingRegenAccumulator = 0f;

    private ArtifactManager artifactManager;

    private Vector2 _prevPos;
    private float _speedSqr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (ultimateSystem == null)
            ultimateSystem = GetComponent<UltimateSystem>();

        artifactManager = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;

        ApplyCharacterStats();

        currentHealth = maxHealth;

        PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        if (ultimateSystem != null)
            UltimateSystem.OnUltimateChargeChanged?.Invoke(ultimateSystem.currentCharge, ultimateSystem.chargeNeeded);
        _prevPos = rb != null ? rb.position : (Vector2)transform.position;
    }


    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        CheckForEnemyClick();
    }

    private void FixedUpdate()
    {
        Vector2 curPos = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 step = movement.normalized * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
            rb.MovePosition(curPos + step);
        else
            transform.position += (Vector3)step;

        Vector2 newPos = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 delta = newPos - _prevPos;
        _speedSqr = (delta / Time.fixedDeltaTime).sqrMagnitude;
        _prevPos = newPos;

        UpdateBlessingRegen();
    }

    private void UpdateBlessingRegen()
    {
        if (artifactManager == null) return;
        if (!artifactManager.HasArtifact(ArtifactEffectType.Blessing)) return;
        if (currentHealth <= 0) return;

        float threshSqr = blessingIdleSpeedThreshold * blessingIdleSpeedThreshold;
        bool isIdle = _speedSqr <= threshSqr;

        if (!isIdle)
        {
            blessingIdleTimer = 0f;
            blessingRegenAccumulator = 0f;
            return;
        }

        blessingIdleTimer += Time.fixedDeltaTime;
        if (blessingIdleTimer < blessingStartDelay)
            return;

        blessingRegenAccumulator += blessingRegenPerSecond * Time.fixedDeltaTime;

        int healInt = Mathf.FloorToInt(blessingRegenAccumulator);
        if (healInt > 0)
        {
            Heal(healInt);
            blessingRegenAccumulator -= healInt;
        }
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

    private void CheckForEnemyClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float radius = 0.2f;
        Collider2D hit = Physics2D.OverlapCircle(mousePos, radius, enemyLayer);

        if (hit == null) return;

        Enemy enemy = hit.GetComponent<Enemy>();
        if (enemy == null) return;

        int dmg = clickDamage;

        var effects = GetComponent<ArtifactEffectSystem>();
        if (effects != null)
            dmg = effects.ModifyClickDamage(clickDamage);

        enemy.TakeDamage(dmg);
        AudioManager.Instance?.PlayEnemyHit();
        int add = 1;
        if (GameManager.Instance != null)
        {
            var stats = GameManager.Instance.GetSelectedStats();
            if (stats != null) add = stats.chargeAddPerClick;
        }
        ultimateSystem?.AddCharge(add);

    }

    public void TakeDamage(int damage)
    {
        if (ghostShieldActive)
        {
            ghostShieldActive = false;

            FloatingDamageText.Spawn(transform.position + Vector3.up * 1.0f, 0);
            PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

            Debug.Log("Ghost Shield absorbed damage!");
            return;
        }

        currentHealth -= damage;
        AudioManager.Instance?.PlayPlayerHit();

        PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
        FloatingDamageText.Spawn(transform.position + Vector3.up * 1.0f, damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        var cloud = GetComponent<ShadowCloudEffect>();
        if (cloud != null)
            cloud.OnPlayerDamaged();

        CheckGhostShield();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        int before = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (currentHealth != before)
            PlayerEvents.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        CheckGhostShield();
    }

    private void Die()
    {
        GameManager.Instance.PlayerDied();
        gameObject.SetActive(false);
    }

    private void ApplyCharacterStats()
    {
        if (GameManager.Instance == null) return;

        var stats = GameManager.Instance.GetSelectedStats();
        if (stats == null) return;

        maxHealth = stats.maxHealth;
        moveSpeed = stats.moveSpeed;
        clickDamage = stats.clickDamage;

        if (ultimateSystem != null)
        {
            ultimateSystem.chargeNeeded = stats.chargeNeeded;
            ultimateSystem.ultimateDamage = stats.ultimateDamage;
            ultimateSystem.ultimateRadius = stats.ultimateRadius;
        }
    }

}
