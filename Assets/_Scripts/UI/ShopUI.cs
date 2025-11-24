using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Coins UI")]
    [SerializeField] private TextMeshProUGUI coinsValueText;

    private void Start()
    {
        int currentCoins = GetPlayerCoinsStub();
        UpdateCoinsUI(currentCoins);
    }

    // ВРЕМЕННЫЙ метод-заглушка.
    private int GetPlayerCoinsStub()
    {
        // Пример: return RunResult.Coins;  // TODO
        return 0;
    }

    public void UpdateCoinsUI(int coins)
    {
        if (coinsValueText != null)
            coinsValueText.text = coins.ToString();
    }

    // Кнопка "В главное меню"
    public void OnBackToMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
