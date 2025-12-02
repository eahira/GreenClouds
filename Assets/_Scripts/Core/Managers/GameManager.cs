using UnityEngine;

/// Уровни сложности игры
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int coins = 0;

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

    // =========================================
    //          COINS
    // =========================================
    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) coins = 0;

        Debug.Log($"Coins: {coins}");
        // сюда потом можно прикрутить обновление UI
    }

    // =========================================
    //          DIFFICULTY
    // =========================================
    public void SetDifficulty(DifficultyLevel level)
    {
        CurrentDifficulty = level;
        Debug.Log($"Difficulty set to: {CurrentDifficulty}");
    }

    // сколько здоровья у врагов
    public float GetEnemyHealthMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy:   return 0.8f;
            case DifficultyLevel.Hard:   return 1.4f;
            default:                     return 1.0f; // Medium
        }
    }

    // сколько врагов спавнить
    public float GetEnemyCountMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy:   return 0.8f;
            case DifficultyLevel.Hard:   return 1.5f;
            default:                     return 1.0f;
        }
    }

    // сколько урона наносят враги
    public float GetEnemyDamageMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy:   return 0.8f;
            case DifficultyLevel.Hard:   return 1.3f;
            default:                     return 1.0f;
        }
    }

    // =========================================
    //          GAME FLOW HOOKS
    // =========================================
    public void OnEnemyKilled()
    {
        // сюда можно будет повесить счётчик, статистику и т.п.
        Debug.Log("Enemy killed");
    }

    public void PlayerDied()
    {
        Debug.Log("Player died");
        // позже сделаем экран поражения / рестарт
    }

	public void LevelCompleted()
	{
    Debug.Log("Level completed");
    // TODO: тут позже сделаем экран победы, загрузку следующего уровня и т.п.
	}

}
