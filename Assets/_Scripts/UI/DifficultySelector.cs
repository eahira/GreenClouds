using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    // Ссылки на кнопки
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public Button backButton;
    public Button acceptButton;

    private void Start()
    {
        // Привязка кнопок к методам
        easyButton.onClick.AddListener(OnEasySelected);
        mediumButton.onClick.AddListener(OnMediumSelected);
        hardButton.onClick.AddListener(OnHardSelected);
        backButton.onClick.AddListener(OnBackPressed);
        acceptButton.onClick.AddListener(OnAcceptPressed);
    }

    // Логика для легкой сложности
    private void OnEasySelected()
    {
        // Выбираем легкую сложность
        Debug.Log("Легкий режим выбран");
        // Добавить код для выбора легкой сложности, если нужно
    }

    // Логика для средней сложности
    private void OnMediumSelected()
    {
        // Выбираем среднюю сложность
        Debug.Log("Средний режим выбран");
        // Добавить код для выбора средней сложности, если нужно
    }

    // Логика для тяжелой сложности
    private void OnHardSelected()
    {
        // Выбираем тяжелую сложность
        Debug.Log("Тяжелый режим выбран");
        // Добавить код для выбора тяжелой сложности, если нужно
    }

    // Метод для возврата на сцену выбора персонажа
    private void OnBackPressed()
    {
        SceneManager.LoadScene("CharacterSelectorScene");
    }

    // Метод для запуска сцены игры (SampleScene)
    private void OnAcceptPressed()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
