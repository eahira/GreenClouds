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

    [Tooltip("Радиус, в котором враг вообще видит игрока")]
    public float aggroRange = 12f;

    [Tooltip("Слои, которые считаются стенами/препятствиями")]
    public LayerMask obstacleMask;   // сюда поставим слой Obstacles

    [Header("Damage")]
	public int contactDamage = 10;

	[Header("Attack")]
	public float attackCooldown = 0.7f;   // время между ударами по игроку, в секундах
	private float lastAttackTime = -999f; // когда в последний раз били игрока


    [Header("Loot")]
    public GameObject lootPrefab;

    private void Awake()
    {
        currentHealth = maxHealth;

        // если не настроишь в инспекторе — подстрахуемся
        if (obstacleMask.value == 0)
            obstacleMask = LayerMask.GetMask("Obstacles");
    }

    private void Update()
    {
        MoveTowardsPlayer();
    }

    /// <summary>
    /// Вызывается спавнером, чтобы передать ссылку на игрока
    /// </summary>
    public void Init(Transform playerTransform)
    {
        player = playerTransform;

        float hpMul = 1f;
        float dmgMul = 1f;

        if (GameManager.Instance != null)
        {
            hpMul  = GameManager.Instance.GetEnemyHealthMultiplier();
            dmgMul = GameManager.Instance.GetEnemyDamageMultiplier();
        }

        maxHealth = Mathf.RoundToInt(maxHealth * hpMul);
        currentHealth = maxHealth;

        contactDamage = Mathf.RoundToInt(contactDamage * dmgMul);
    }


    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 toPlayer = (player.position - transform.position);
        float sqrDist = toPlayer.sqrMagnitude;

        // 1) слишком далеко — даже не пытаемся агриться
        if (sqrDist > aggroRange * aggroRange)
            return;

        // 2) проверка на стену между врагом и игроком
        Vector2 dir = toPlayer.normalized;
        float dist = Mathf.Sqrt(sqrDist);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
        // Если луч упёрся во что-то на слое Obstacles ДО игрока — видимость заблокирована
        if (hit.collider != null)
        {
            // есть препятствие, не двигаемся
            return;
        }

        // 3) путь свободен — двигаемся к игроку
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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
    	TryHitPlayer(other);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
	    // пока враг "трётся" об игрока — он тоже может бить, но не чаще, чем раз в attackCooldown
    	TryHitPlayer(other);
	}

	private void TryHitPlayer(Collider2D other)
	{
    	if (!other.CompareTag("Player")) return;

    	// проверяем кд
    	if (Time.time - lastAttackTime < attackCooldown)
        	return;

    	PlayerController player = other.GetComponent<PlayerController>();
    	if (player == null) return;

    	lastAttackTime = Time.time;
    	player.TakeDamage(contactDamage);
	}

}
