using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UltimateSystem : MonoBehaviour
{
    [Header("Charge")]
    public int currentCharge = 0;
    public int chargeNeeded = 10;

    [Header("Ultimate Params")]
    public float ultimateDamage = 50f;
    public float ultimateRadius = 3f;

    [Header("VFX")]
    public GameObject ultimateCircle; 
    public float circleShowTime = 0.25f;

    [Header("Optional local UI (можно не использовать)")]
    public Image ultimateFill;
    public Text ultimateText;

    public static System.Action<int, int> OnUltimateChargeChanged;

    private Coroutine circleRoutine;

    private void Start()
    {
        if (ultimateCircle != null)
            ultimateCircle.SetActive(false);

        RefreshUI();
    }

    public void AddCharge(int amount)
    {
        if (amount <= 0) return;

        currentCharge += amount;
        if (currentCharge > chargeNeeded)
            currentCharge = chargeNeeded;

        RefreshUI();

        if (currentCharge >= chargeNeeded)
        {
            ActivateUltimate();

            currentCharge = 0;
            chargeNeeded += 5;
            ultimateDamage += 10f;

            RefreshUI();
        }
    }

    private void ActivateUltimate()
    {
        ShowCircle();

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, ultimateRadius);

        for (int i = 0; i < cols.Length; i++)
        {
            if (!cols[i].CompareTag("Enemy")) continue;

            Enemy e = cols[i].GetComponent<Enemy>();
            if (e != null)
                e.TakeDamage((int)ultimateDamage);
        }

        AudioManager.Instance?.PlayUltimate();
    }

    private void ShowCircle()
    {
        if (ultimateCircle == null) return;

        ultimateCircle.transform.position = transform.position;

        if (circleRoutine != null)
            StopCoroutine(circleRoutine);

        circleRoutine = StartCoroutine(CircleRoutine());
    }

    private IEnumerator CircleRoutine()
    {
        ultimateCircle.SetActive(true);
        yield return new WaitForSeconds(circleShowTime);

        if (ultimateCircle != null)
            ultimateCircle.SetActive(false);

        circleRoutine = null;
    }

    private void RefreshUI()
    {
        float fill = (chargeNeeded > 0) ? Mathf.Clamp01((float)currentCharge / chargeNeeded) : 0f;

        if (ultimateFill != null)
            ultimateFill.fillAmount = fill;

        if (ultimateText != null)
            ultimateText.text = Mathf.RoundToInt(fill * 100f) + "%";

        OnUltimateChargeChanged?.Invoke(currentCharge, chargeNeeded);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ultimateRadius);
    }
#endif
}
