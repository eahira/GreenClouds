using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image bigPortrait;
    [SerializeField] private Image[] thumbnails;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private int currentIndex = 0;

    // Заглушка: имена и описания персонажей
    private string[] characterNames = { "Выживший", "Робот", "Ангел" };
    private string[] characterDescriptions =
    {
        "Балансный персонаж. Среднее здоровье и урон.",
        "Высокая защита, низкий урон, особые способности.",
        "Высокий урон, меньшее здоровье, навыки контроля."
    };

    private void Start()
    {
        UpdateCharacterUI();
    }

    // Кнопка "Предыдущий персонаж"
    public void OnPrevCharacter()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = characterNames.Length - 1;
        UpdateCharacterUI();
    }

    // Кнопка "Следующий персонаж"
    public void OnNextCharacter()
    {
        currentIndex++;
        if (currentIndex >= characterNames.Length) currentIndex = 0;
        UpdateCharacterUI();
    }

    // Кнопка "Назад" — возвращаем в главное меню
    public void OnBackPressed()
    {
        // Переход в главное меню
        SceneManager.LoadScene("MainMenuScene");
    }

    // Кнопка "Принять" — сохраняем выбор и начинаем игру
    public void OnAcceptPressed()
    {
        // Заглушка: вывести в консоль выбранного персонажа
        Debug.Log("Selected character: " + characterNames[currentIndex]);

        // Здесь будет логика для передачи выбранного персонажа в игровую механику

        // После принятия — начинаем сцену игры
        SceneManager.LoadScene("SampleScene");
    }

    // Обновление интерфейса с информацией о персонаже
    private void UpdateCharacterUI()
    {
        // Изменение описания персонажа
        if (descriptionText != null)
        {
            descriptionText.text = characterDescriptions[currentIndex];
        }

        // Подсветка мини-аватарки для выбранного персонажа
        for (int i = 0; i < thumbnails.Length; i++)
        {
            if (thumbnails[i] == null) continue;
            thumbnails[i].color = (i == currentIndex) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
        }
    }
}
