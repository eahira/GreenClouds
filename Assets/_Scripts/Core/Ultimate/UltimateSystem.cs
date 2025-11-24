using UnityEngine;
using UnityEngine.UI;

public class UltimateSystem : MonoBehaviour
{
    public Image ultimateFill;
    public Text ultimateText;

    public int currentCharge = 0;
    public int chargeNeeded = 10;

    public float ultimateDamage = 50f;
    public float ultimateRadius = 3f;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ultimateFill != null)
        {
            float fillPercent = (float)currentCharge / chargeNeeded;
            fillPercent = Mathf.Clamp01(fillPercent);

            float maxWidth = 300f; // ширина полоски (можешь помен€ть под свой UI)
            RectTransform rt = ultimateFill.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(maxWidth * fillPercent, rt.sizeDelta.y);
        }

        if (ultimateText != null)
        {
            float percent = (float)currentCharge / chargeNeeded * 100f;
            ultimateText.text = Mathf.RoundToInt(percent) + "%";
        }
    }

    public void AddCharge(int amount)
    {
        currentCharge += amount;

        if (currentCharge >= chargeNeeded)
        {
            ActivateUltimate();

            // —брасываем и усложн€ем ульту дл€ следующего раза
            currentCharge = 0;
            chargeNeeded += 5;
            ultimateDamage += 10f;
        }
    }

    void ActivateUltimate()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, ultimateRadius);
    }
}
