using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI upgradesText;

    private void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        int level = gm.CurrentStage;

        if (titleText != null)
            titleText.text = $"Уровень {level} пройден";

        if (coinsText != null)
            coinsText.text = $"Вы набрали {gm.coins} монет";
        
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnNextLevelPressed()
    {
        var gm = GameManager.Instance;
        if (gm != null)
            gm.AdvanceStage();

        SceneManager.LoadScene("SampleScene");
    }
}
