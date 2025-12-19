using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DefeatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    private void Start()
    {
        AdsService.Instance?.ShowInterstitial("defeat");

        if (coinsText != null && GameManager.Instance != null)
        {
            coinsText.text = $"Вы набрали {GameManager.Instance.coins} монет";
        }
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnRestartPressed()
    {
        SceneManager.LoadScene("CharacterSelectorScene");
    }
}
