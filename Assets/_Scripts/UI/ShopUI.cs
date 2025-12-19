using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Coins UI")]
    [SerializeField] private TextMeshProUGUI coinsValueText;

    [Header("Robot UI")]
    [SerializeField] private TextMeshProUGUI robotPriceText;
    [SerializeField] private Button robotBuyButton;

    [Header("Angel UI")]
    [SerializeField] private TextMeshProUGUI angelPriceText;
    [SerializeField] private Button angelBuyButton;

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            UpdateCoinsUI(0);
            if (robotPriceText != null) robotPriceText.text = "-";
            if (angelPriceText != null) angelPriceText.text = "-";
            if (robotBuyButton != null) robotBuyButton.interactable = false;
            if (angelBuyButton != null) angelBuyButton.interactable = false;
            return;
        }

        UpdateCoinsUI(gm.coins);

        if (robotPriceText != null)
        {
            robotPriceText.text = gm.robotUnlocked
                ? "Куплен"
                : gm.robotPrice.ToString();
        }

        if (robotBuyButton != null)
        {
            robotBuyButton.interactable = !gm.robotUnlocked && gm.coins >= gm.robotPrice;
        }

        if (angelPriceText != null)
        {
            angelPriceText.text = gm.angelUnlocked
                ? "Куплен"
                : gm.angelPrice.ToString();
        }

        if (angelBuyButton != null)
        {
            angelBuyButton.interactable = !gm.angelUnlocked && gm.coins >= gm.angelPrice;
        }
    }

    public void UpdateCoinsUI(int coins)
    {
        if (coinsValueText != null)
            coinsValueText.text = coins.ToString();
    }

    public void OnBuyRobotPressed()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.TryBuyRobot())
        {
            Debug.Log("Robot bought from shop.");
            RefreshUI();
        }
        else
        {
            Debug.Log("Not enough coins for robot or already unlocked.");
            RefreshUI();
        }
    }

    public void OnBuyAngelPressed()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.TryBuyAngel())
        {
            Debug.Log("Angel bought from shop.");
            RefreshUI();
        }
        else
        {
            Debug.Log("Not enough coins for angel or already unlocked.");
            RefreshUI();
        }
    }

    public void OnGet100CoinsByAdPressed()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (AdsService.Instance == null)
        {
            Debug.LogWarning("AdsService not found. Make sure SDK object with AdsService exists in MainMenuScene.");
            return;
        }

        AdsService.Instance.ShowRewarded(() =>
        {
            gm.AddCoins(100);
            RefreshUI();
            Debug.Log("Rewarded: +100 coins");
        }, "shop_100");
    }

    public void OnBackToMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
