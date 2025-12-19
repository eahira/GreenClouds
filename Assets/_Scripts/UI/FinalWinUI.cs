using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinalWinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI unlockText;

    private void Start()
    {
        AdsService.Instance?.ShowInterstitial("final_win");

        var gm = GameManager.Instance;
        if (gm == null) return;

        if (titleText != null)
            titleText.text = "Победа!";

        if (coinsText != null)
            coinsText.text = $"Вы набрали {gm.coins} монет";
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