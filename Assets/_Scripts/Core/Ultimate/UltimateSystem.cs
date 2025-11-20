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

        if (currentCharge >= chargeNeeded)
            ActivateUltimate();
    }

    void UpdateUI()
    {
        if (ultimateFill != null)
        {
            float fillPercent = (float)currentCharge / chargeNeeded;
            float maxWidth = 300f; // твой размер
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
        currentCharge = Mathf.Min(currentCharge + amount, chargeNeeded);
    }

    void ActivateUltimate()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, ultimateRadius);
        foreach (var col in enemies)
        {
            if (col.CompareTag("Enemy"))
                col.GetComponent<Enemy>().TakeDamage((int)ultimateDamage);
        }

        currentCharge = 0;
        chargeNeeded += 5;
        ultimateDamage += 10f;
    }
}
