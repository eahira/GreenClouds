using UnityEngine;
using UnityEngine.UI;

public class UltimateSystem : MonoBehaviour
{
    [Header("UI (старый локальный бар, можно не использовать)")]
    public Image ultimateFill;
    public Text ultimateText;

    [Header("Заряд ульты")]
    public int currentCharge = 0;
    public int chargeNeeded = 10;

    [Header("Параметры ульты")]
    public float ultimateDamage = 50f;
    public float ultimateRadius = 3f;


    public static System.Action<int, int> OnUltimateChargeChanged;

    private void Start()
    {
        UpdateUI();
        NotifyUI();
    }

    private void Update()
    {
        UpdateUI();
    }


    public void AddCharge(int amount)
    {
        currentCharge += amount;

        if (currentCharge > chargeNeeded)
            currentCharge = chargeNeeded;

        if (currentCharge >= chargeNeeded)
        {
            ActivateUltimate();

            currentCharge = 0;
            chargeNeeded += 5;
            ultimateDamage += 10f;
        }

        UpdateUI();
        NotifyUI();
    }


    private void ActivateUltimate()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, ultimateRadius);

        foreach (var col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage((int)ultimateDamage);
            }
        }
    }


    private void UpdateUI()
    {
        float fillPercent = 0f;

        if (chargeNeeded > 0)
            fillPercent = Mathf.Clamp01((float)currentCharge / chargeNeeded);

        if (ultimateFill != null)
            ultimateFill.fillAmount = fillPercent;

        if (ultimateText != null)
        {
            int percent = Mathf.RoundToInt(fillPercent * 100f);
            ultimateText.text = percent + "%";
        }
    }


    private void NotifyUI()
    {
        OnUltimateChargeChanged?.Invoke(currentCharge, chargeNeeded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ultimateRadius);
    }
}
