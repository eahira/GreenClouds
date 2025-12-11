using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI upgradesText; // можно оставить пустым / заполнять в инспекторе

    private void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        int level = gm.CurrentStage;

        if (titleText != null)
            titleText.text = $"Уровень {level} пройден";

        if (coinsText != null)
            coinsText.text = $"Вы набрали {gm.coins} монет";

        // upgradesText можешь заполнять статикой, типа:
        // "- +10% урон\n- +5% скорость" или просто оставить пустым
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnNextLevelPressed()
    {
        var gm = GameManager.Instance;
        if (gm != null)
            gm.AdvanceStage();           // переходим к 2 или 3 уровню

        SceneManager.LoadScene("SampleScene");
    }
}
