using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public Button backButton;
    public Button acceptButton;

    private DifficultyLevel selectedDifficulty = DifficultyLevel.Medium;

    private void Start()
    {
        // выбор сложности
        easyButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Easy));
        mediumButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Medium));
        hardButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Hard));

        // навигация
        backButton.onClick.AddListener(OnBackPressed);
        acceptButton.onClick.AddListener(OnAcceptPressed);

        // по умолчанию Medium
        HighlightButtons();
    }

    private void SelectDifficulty(DifficultyLevel level)
    {
        selectedDifficulty = level;
        HighlightButtons();
    }

    private void HighlightButtons()
    {
        // Очень простой визуал: меняем alpha цвета
        SetButtonState(easyButton,   selectedDifficulty == DifficultyLevel.Easy);
        SetButtonState(mediumButton, selectedDifficulty == DifficultyLevel.Medium);
        SetButtonState(hardButton,   selectedDifficulty == DifficultyLevel.Hard);
    }

    private void SetButtonState(Button btn, bool selected)
    {
        if (btn == null) return;

        Color c = btn.image.color;
        c.a = selected ? 1f : 0.5f;
        btn.image.color = c;
    }

    private void OnBackPressed()
    {
        SceneManager.LoadScene("CharacterSelectorScene");
    }

    private void OnAcceptPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetDifficulty(selectedDifficulty);

        SceneManager.LoadScene("SampleScene");
    }
}
