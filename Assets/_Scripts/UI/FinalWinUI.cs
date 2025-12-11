using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinalWinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI unlockText; // текст про нового героя/характеристику

    private void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (titleText != null)
            titleText.text = "Победа!";

        if (coinsText != null)
            coinsText.text = $"Вы набрали {gm.coins} монет";

        // Здесь можно написать, что открылось:
        // Например: "Доступен новый персонаж для покупки: Робот"
        // Пока можно задать этот текст прямо в инспекторе в unlockText.text
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnShopPressed()
    {
        SceneManager.LoadScene("ShopScene");
    }
}
