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
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button acceptButton;

    private int currentIndex = 0;

    // Имена и описания только для UI
    private readonly string[] characterNames = { "Выживший", "Робот", "Ангел" };
    private readonly string[] characterDescriptions =
    {
        "Балансный персонаж. Среднее здоровье и урон.",
        "Высокая защита, низкий урон, особые способности.",
        "Высокий урон, меньшее здоровье, навыки контроля."
    };

    private void Start()
    {
        // Подтягиваем выбранного персонажа из GameManager
        if (GameManager.Instance != null)
        {
            switch (GameManager.Instance.selectedCharacter)
            {
                case CharacterType.Survivor:
                    currentIndex = 0;
                    break;
                case CharacterType.Robot:
                    currentIndex = 1;
                    break;
                case CharacterType.Angel:
                    currentIndex = 2;
                    break;
            }
        }

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
        SceneManager.LoadScene("MainMenuScene");
    }

    // Кнопка "Принять" — сохраняем выбор и идём на выбор сложности
    public void OnAcceptPressed()
	{
 	   var gm = GameManager.Instance;
 	   if (gm != null)
  	  {
    	    // запоминаем выбранного персонажа
 	       gm.SelectCharacter((CharacterType)currentIndex);

 	       // начинаем НОВЫЙ забег для этого героя с 1 уровня
     	   gm.ResetCurrentStage();
 	   }

	    // после выбора перса -> экран выбора сложности
	    SceneManager.LoadScene("DifficultySelectorScene");
	}


    // Обновление интерфейса с информацией о персонаже
    private void UpdateCharacterUI()
    {
        CharacterType type = GetCharacterTypeByIndex(currentIndex);
        bool isLocked = IsCharacterLocked(type);

        // Имя
        if (nameText != null)
            nameText.text = characterNames[currentIndex];

        // Описание
        if (descriptionText != null)
        {
            if (!isLocked)
            {
                descriptionText.text = characterDescriptions[currentIndex];
            }
            else
            {
                // Текст для заблокированных
                if (GameManager.Instance != null)
                {
                    if (type == CharacterType.Robot)
                        descriptionText.text = $"Робот заблокирован.\nКупите в магазине за {GameManager.Instance.robotPrice} монет.";
                    else if (type == CharacterType.Angel)
                        descriptionText.text = $"Ангел заблокирован.\nКупите в магазине за {GameManager.Instance.angelPrice} монет.";
                }
                else
                {
                    descriptionText.text = "Персонаж заблокирован.";
                }
            }
        }

        // Подсветка мини-аватарки для выбранного персонажа
        for (int i = 0; i < thumbnails.Length; i++)
        {
            if (thumbnails[i] == null) continue;

            // выбранная — белая, остальные притемнённые
            thumbnails[i].color = (i == currentIndex)
                ? Color.white
                : new Color(0.7f, 0.7f, 0.7f);
        }

        // Если герой заблокирован — нельзя нажать "Принять"
        if (acceptButton != null)
            acceptButton.interactable = !isLocked;

    }

    // Маппинг индекс -> тип персонажа
    private CharacterType GetCharacterTypeByIndex(int index)
    {
        switch (index)
        {
            case 0: return CharacterType.Survivor;
            case 1: return CharacterType.Robot;
            case 2: return CharacterType.Angel;
            default: return CharacterType.Survivor;
        }
    }

    // Проверка, заблокирован ли герой
    private bool IsCharacterLocked(CharacterType type)
    {
        if (GameManager.Instance == null)
            return false; // оффлайн-режим: никого не блокируем

        switch (type)
        {
            case CharacterType.Robot:
                return !GameManager.Instance.robotUnlocked;
            case CharacterType.Angel:
                return !GameManager.Instance.angelUnlocked;
            default:
                return false; // Выживший всегда доступен
        }
    }
}
