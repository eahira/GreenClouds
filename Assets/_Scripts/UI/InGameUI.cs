using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
	[Header("Avatar")]
	public Image avatarImage;

	[Tooltip("Порядок: Survivor, Robot, Angel")]
	public Sprite[] avatarSprites;
	
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

    [Header("Artifacts (6 slots icons)")]
    public Image[] artifactSlotIcons;

    private ArtifactManager artifactManager;

    private void Awake()
    {
        if (GameManager.Instance != null)
            artifactManager = GameManager.Instance.GetComponent<ArtifactManager>();
    }

    private void OnEnable()
    {
        PlayerEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        UltimateSystem.OnUltimateChargeChanged += OnUltimateChanged;

        if (artifactManager != null)
            artifactManager.OnArtifactsChanged += OnArtifactsChanged;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        UltimateSystem.OnUltimateChargeChanged -= OnUltimateChanged;

        if (artifactManager != null)
            artifactManager.OnArtifactsChanged -= OnArtifactsChanged;

        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(OnPauseClicked);
    }

    private void Start()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

		UpdateAvatar();

        if (levelText != null)
        {
            int stage = 1;
            if (GameManager.Instance != null)
                stage = GameManager.Instance.CurrentStage;

            levelText.text = $"Уровень {stage}";
        }

        if (questText != null)
            questText.text = "Убейте босса";

        if (artifactManager != null)
            OnArtifactsChanged(artifactManager.Artifacts);
        else
            ClearArtifactSlots();
    }
	
	private void UpdateAvatar()
	{
    	if (avatarImage == null) return;
    	if (avatarSprites == null || avatarSprites.Length < 3) return;
    	if (GameManager.Instance == null) return;

    	int idx = 0;
    	switch (GameManager.Instance.selectedCharacter)
    	{
        	case CharacterType.Survivor: idx = 0; break;
        	case CharacterType.Robot:    idx = 1; break;
        	case CharacterType.Angel:    idx = 2; break;
    	}

    	avatarImage.sprite = avatarSprites[idx];
    	avatarImage.preserveAspect = true;
    	avatarImage.enabled = (avatarImage.sprite != null);
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
        var pm = FindFirstObjectByType<PauseManager>();
        if (pm != null)
            pm.OpenPause();
    }

    private void OnArtifactsChanged(System.Collections.Generic.IReadOnlyList<ArtifactData> artifacts)
    {
        if (artifactSlotIcons == null || artifactSlotIcons.Length == 0)
            return;

        for (int i = 0; i < artifactSlotIcons.Length; i++)
        {
            if (artifactSlotIcons[i] == null) continue;
            artifactSlotIcons[i].sprite = null;
            artifactSlotIcons[i].enabled = false;
        }

        if (artifacts == null) return;

        int count = Mathf.Min(artifacts.Count, artifactSlotIcons.Length);
        for (int i = 0; i < count; i++)
        {
            var data = artifacts[i];
            var iconImg = artifactSlotIcons[i];

            if (iconImg == null) continue;
            if (data == null || data.icon == null) continue;

            iconImg.sprite = data.icon;
            iconImg.enabled = true;
        }
    }

    private void ClearArtifactSlots()
    {
        if (artifactSlotIcons == null) return;
        for (int i = 0; i < artifactSlotIcons.Length; i++)
        {
            if (artifactSlotIcons[i] == null) continue;
            artifactSlotIcons[i].sprite = null;
            artifactSlotIcons[i].enabled = false;
        }
    }
}
