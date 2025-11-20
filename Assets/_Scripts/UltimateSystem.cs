using UnityEngine;
using UnityEngine.UI;

public class UltimateSystem : MonoBehaviour
{
    [Header("Ultimate UI")]
    public Image ultimateFill;
    public Text ultimateText; // Обычный Text вместо TextMeshPro

    [Header("Settings")]
    public int currentCharge = 0;
    public int chargeNeeded = 10;
    public float ultimateDamage = 50f;
    public float ultimateRadius = 3f;

    private float maxWidth = 300f;

    void Update()
    {
        UpdateUI();

        if (currentCharge >= chargeNeeded)
        {
            ActivateUltimate();
        }
    }

    void UpdateUI()
    {
        if (ultimateFill != null && ultimateText != null)
        {
            // Вычисляем проценты
            float fillPercent = (float)currentCharge / chargeNeeded;
            int percent = Mathf.RoundToInt(fillPercent * 100);

            // Меняем ширину заполнения
            float newWidth = maxWidth * fillPercent;
            RectTransform fillRect = ultimateFill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(newWidth, fillRect.sizeDelta.y);

            // Обновляем текст
            ultimateText.text = percent + "%";
        }
    }

    public void AddCharge(int amount)
    {
        currentCharge = Mathf.Min(currentCharge + amount, chargeNeeded);
    }

    void ActivateUltimate()
    {
        Debug.Log("УЛЬТИМЕЙТ АКТИВИРОВАН!");

        // AoE урон по врагам в радиусе
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, ultimateRadius);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)ultimateDamage);
                    Debug.Log("AoE урон по врагу: " + ultimateDamage);
                }
            }
        }

        // Сбрасываем и увеличиваем сложность
        currentCharge = 0;
        chargeNeeded += 5;
        ultimateDamage += 10f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ultimateRadius);
    }
}