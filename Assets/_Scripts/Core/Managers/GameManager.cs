using UnityEngine;
using UnityEngine.SceneManagement;

/// Уровни сложности игры
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

/// Типы персонажей
public enum CharacterType
{
    Survivor,   // Выживший
    Robot,
    Angel
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ----------------- Экономика -----------------
    [Header("Economy")]
    public int coins = 0;

    // ----------------- Разблокировка персонажей -----------------
    [Header("Characters unlock")]
    public bool robotUnlocked = false;
    public bool angelUnlocked = false;
    public int robotPrice = 500;
    public int angelPrice = 800;

    // ----------------- Выбор персонажа и прогресс -----------------
    [Header("Character selection & progress")]
    public CharacterType selectedCharacter = CharacterType.Survivor;

    [Range(1, 3)] public int survivorStage = 1; // 1..3
    [Range(1, 3)] public int robotStage = 1;
    [Range(1, 3)] public int angelStage = 1;

    public const int MaxStage = 3;

    /// Текущий уровень (1..3) для выбранного героя
    public int CurrentStage
    {
        get
        {
            switch (selectedCharacter)
            {
                case CharacterType.Robot: return robotStage;
                case CharacterType.Angel: return angelStage;
                default: return survivorStage;
            }
        }
        set
        {
            int v = Mathf.Clamp(value, 1, MaxStage);
            switch (selectedCharacter)
            {
                case CharacterType.Robot: robotStage = v; break;
                case CharacterType.Angel: angelStage = v; break;
                default: survivorStage = v; break;
            }
        }
    }

    // ----------------- Сложность -----------------
    [Header("Difficulty")]
    public DifficultyLevel CurrentDifficulty { get; private set; } = DifficultyLevel.Medium;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================
    //      ВЫБОР ПЕРСОНАЖА
    // =========================
    public void SelectCharacter(CharacterType type)
    {
        selectedCharacter = type;
    }

    public void ResetCurrentStage()
    {
        CurrentStage = 1;
    }

    public void AdvanceStage()
    {
        if (CurrentStage < MaxStage)
            CurrentStage++;
    }

    // =========================
    //          МОНЕТЫ
    // =========================
    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) coins = 0;
        Debug.Log($"Coins: {coins}");
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount <= 0) return true;
        if (coins < amount) return false;

        coins -= amount;
        return true;
    }

    public bool TryBuyRobot()
    {
        if (robotUnlocked) return false;
        if (!TrySpendCoins(robotPrice)) return false;

        robotUnlocked = true;
        return true;
    }

    public bool TryBuyAngel()
    {
        if (angelUnlocked) return false;
        if (!TrySpendCoins(angelPrice)) return false;

        angelUnlocked = true;
        return true;
    }

    // =========================
    //         DIFFICULTY
    // =========================
    public void SetDifficulty(DifficultyLevel level)
    {
        CurrentDifficulty = level;
        Debug.Log($"Difficulty set to: {CurrentDifficulty}");
    }

    // множитель здоровья врагов по сложности
    public float GetEnemyHealthMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 0.8f;
            case DifficultyLevel.Hard: return 1.4f;
            default: return 1.0f;
        }
    }

    // множитель количества врагов по сложности
    public float GetEnemyCountMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 0.8f;
            case DifficultyLevel.Hard: return 1.5f;
            default: return 1.0f;
        }
    }

    // множитель количества врагов по ЭТАПУ (1/2/3)
    public float GetEnemyDamageMultiplier()
	{
    	switch (CurrentDifficulty)
    	{
        	case DifficultyLevel.Easy:  return 0.8f;  // на лёгком враги бьют слабее
        	case DifficultyLevel.Hard:  return 1.3f;  // на сложном — сильнее
        	default:                    return 1.0f;  // Medium
    	}
	}
	

    // =========================
    //        GAME FLOW
    // =========================
    public void OnEnemyKilled()
    {
        Debug.Log("Enemy killed");
        // сюда потом добавим статистику, квесты и т.п.
    }

    /// Игрок умер → уровень героя сбрасываем на 1 и показываем экран поражения
    public void PlayerDied()
    {
        Debug.Log("Player died");
        ResetCurrentStage();                 // сбрасываем прогресс героя
        SceneManager.LoadScene("DefeatScene");
    }

    /// Босс убит → либо LevelComplete, либо FinalWin
    public void LevelCompleted()
{
    Debug.Log($"Level {CurrentStage} completed");

    if (CurrentStage < MaxStage)
    {
        // Прошли уровень 1 или 2
        SceneManager.LoadScene("LevelCompleteScene");
    }
    else
    {
        // Прошли финальный (3) уровень для текущего персонажа
        // Забег для этого героя должен начаться с 1 уровня в следующий раз
        ResetCurrentStage();

        SceneManager.LoadScene("FinalWinScene");
    }
}

}
