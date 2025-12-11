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

    // Обновляем всё визуально
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

        // Монеты
        UpdateCoinsUI(gm.coins);

        // Робот
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

        // Ангел
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

    // Кнопка "Купить робота"
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

    // Кнопка "Купить ангела"
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

    // Кнопка "В главное меню"
    public void OnBackToMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
