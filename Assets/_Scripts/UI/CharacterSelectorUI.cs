using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image bigPortrait;

    // ВАЖНО: thumbnails[0]=левый (prev), thumbnails[1]=центр (current), thumbnails[2]=правый (next)
    [SerializeField] private Image[] thumbnails;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button acceptButton;

    [Header("Sprites")]
    [Tooltip("Большие портреты по порядку: Survivor, Robot, Angel")]
    [SerializeField] private Sprite[] bigPortraitSprites;

    [Tooltip("Мини-иконки по порядку: Survivor, Robot, Angel")]
    [SerializeField] private Sprite[] thumbnailSprites;

    [Header("Locked Visual (optional)")]
    [SerializeField] private Color lockedTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color unlockedTint = Color.white;

    private int currentIndex = 0;

    private readonly string[] characterNames = { "Выживший", "Робот", "Ангел" };
    private readonly string[] characterDescriptions =
    {
        "Балансный персонаж. Среднее здоровье и урон.",
        "Высокая защита, низкий урон, особые способности.",
        "Высокий урон, меньшее здоровье, навыки контроля."
    };

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            switch (GameManager.Instance.selectedCharacter)
            {
                case CharacterType.Survivor: currentIndex = 0; break;
                case CharacterType.Robot:    currentIndex = 1; break;
                case CharacterType.Angel:    currentIndex = 2; break;
            }
        }

        UpdateCharacterUI();
    }

    public void OnPrevCharacter()
    {
        currentIndex = WrapIndex(currentIndex - 1);
        UpdateCharacterUI();
    }

    public void OnNextCharacter()
    {
        currentIndex = WrapIndex(currentIndex + 1);
        UpdateCharacterUI();
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnAcceptPressed()
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.SelectCharacter(GetCharacterTypeByIndex(currentIndex));
            gm.ResetCurrentStage();
        }

        SceneManager.LoadScene("DifficultySelectorScene");
    }

    private void UpdateCharacterUI()
    {
        var type = GetCharacterTypeByIndex(currentIndex);
        bool isLocked = IsCharacterLocked(type);

        if (nameText != null)
            nameText.text = characterNames[currentIndex];

        if (descriptionText != null)
        {
            if (!isLocked)
            {
                descriptionText.text = characterDescriptions[currentIndex];
            }
            else
            {
                var gm = GameManager.Instance;
                if (gm != null)
                {
                    if (type == CharacterType.Robot)
                        descriptionText.text = $"Робот заблокирован.\nКупите в магазине за {gm.robotPrice} монет.";
                    else if (type == CharacterType.Angel)
                        descriptionText.text = $"Ангел заблокирован.\nКупите в магазине за {gm.angelPrice} монет.";
                    else
                        descriptionText.text = "Персонаж заблокирован.";
                }
                else
                {
                    descriptionText.text = "Персонаж заблокирован.";
                }
            }
        }

        if (bigPortrait != null && bigPortraitSprites != null && bigPortraitSprites.Length >= characterNames.Length)
        {
            bigPortrait.sprite = bigPortraitSprites[currentIndex];
            bigPortrait.preserveAspect = true;

            bigPortrait.color = isLocked ? lockedTint : unlockedTint;
        }

        RefreshThumbnailsCarousel();

        if (acceptButton != null)
            acceptButton.interactable = !isLocked;
    }

    private void RefreshThumbnailsCarousel()
    {
        if (thumbnails == null || thumbnails.Length < 3) return;
        if (thumbnailSprites == null || thumbnailSprites.Length < characterNames.Length) return;

        int prev = WrapIndex(currentIndex - 1);
        int cur  = WrapIndex(currentIndex);
        int next = WrapIndex(currentIndex + 1);

        SetThumb(0, prev,  highlight: false);
        SetThumb(1, cur,   highlight: true);
        SetThumb(2, next,  highlight: false);
    }

    private void SetThumb(int thumbSlot, int characterIndex, bool highlight)
    {
        var img = thumbnails[thumbSlot];
        if (img == null) return;

        img.sprite = thumbnailSprites[characterIndex];
        img.preserveAspect = true;

        img.color = highlight ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);

        var type = GetCharacterTypeByIndex(characterIndex);
        bool locked = IsCharacterLocked(type);
        if (locked)
            img.color = new Color(img.color.r * lockedTint.r, img.color.g * lockedTint.g, img.color.b * lockedTint.b, 1f);
    }

    private int WrapIndex(int i)
    {
        int n = characterNames.Length;
        return (i % n + n) % n;
    }

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

    private bool IsCharacterLocked(CharacterType type)
    {
        if (GameManager.Instance == null)
            return false;

        switch (type)
        {
            case CharacterType.Robot:
                return !GameManager.Instance.robotUnlocked;
            case CharacterType.Angel:
                return !GameManager.Instance.angelUnlocked;
            default:
                return false;
        }
    }
}
