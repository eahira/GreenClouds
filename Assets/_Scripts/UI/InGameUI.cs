using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("Health")]
    public Image healthFill;
    public TextMeshProUGUI healthText;

    [Header("Ultimate")]
    public Image ultimateFill;
    public TextMeshProUGUI ultimateText;

    [Header("Level & Quest")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI questText;

    [Header("Coins & Pause")]
    public TextMeshProUGUI coinsText;
    public Button pauseButton;

    private void OnEnable()
    {
        PlayerEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        UltimateSystem.OnUltimateChargeChanged += OnUltimateChanged;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        UltimateSystem.OnUltimateChargeChanged -= OnUltimateChanged;

        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseClicked);
    }

    private void Start()
	{
    	if (pauseButton != null)
        	pauseButton.onClick.AddListener(OnPauseClicked);

	    if (levelText != null)
    	{
        	int stage = 1;
        	if (GameManager.Instance != null)
            	stage = GameManager.Instance.CurrentStage;

    	    levelText.text = $"Уровень {stage}";
	    }

    	if (questText != null)
    	    questText.text = "Убейте босса";
	}



    private void Update()
    {
        if (coinsText != null && GameManager.Instance != null)
            coinsText.text = $"{GameManager.Instance.coins}";
    }

    private void OnPlayerHealthChanged(int current, int max)
    {
        if (healthFill != null)
            healthFill.fillAmount = max > 0 ? (float)current / max : 0f;

        if (healthText != null)
            healthText.text = $"{current}/{max}";
    }

    private void OnUltimateChanged(int current, int needed)
    {
        float fill = (needed > 0) ? (float)current / needed : 0f;

        if (ultimateFill != null)
            ultimateFill.fillAmount = fill;

        if (ultimateText != null)
            ultimateText.text = $"{Mathf.RoundToInt(fill * 100f)} %";
    }

    private void OnPauseClicked()
    {
        var pm = FindObjectOfType<PauseManager>();
        if (pm != null)
        {
            pm.OpenPause();
        }
    }
}
