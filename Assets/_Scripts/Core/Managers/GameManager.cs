using UnityEngine;
using UnityEngine.SceneManagement;

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public enum CharacterType
{
    Survivor,
    Robot,
    Angel
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int coins = 0;

    [Header("Characters unlock")]
    public bool robotUnlocked = false;
    public bool angelUnlocked = false;
    public int robotPrice = 500;
    public int angelPrice = 500;

    [Header("Character selection & progress")]
    public CharacterType selectedCharacter = CharacterType.Survivor;

    [Header("Character Stats")]
    public CharacterStats survivorStats;
    public CharacterStats robotStats;
    public CharacterStats angelStats;

    public CharacterStats GetSelectedStats()
    {
        switch (selectedCharacter)
        {
            case CharacterType.Robot: return robotStats != null ? robotStats : survivorStats;
            case CharacterType.Angel: return angelStats != null ? angelStats : survivorStats;
            default: return survivorStats;
        }
    }

    [Range(1, 3)] public int survivorStage = 1;
    [Range(1, 3)] public int robotStage = 1;
    [Range(1, 3)] public int angelStage = 1;

    public const int MaxStage = 3;

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

    [Header("Difficulty")]
    public DifficultyLevel CurrentDifficulty { get; private set; } = DifficultyLevel.Medium;

    private ArtifactManager _artifactManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _artifactManager = GetComponent<ArtifactManager>();
        if (_artifactManager == null)
            _artifactManager = gameObject.AddComponent<ArtifactManager>();
    }

    public void SelectCharacter(CharacterType type) => selectedCharacter = type;

    public void StartNewRun()
    {
        _artifactManager?.ResetRun();
        ResetCurrentStageOnly();
    }

    public void ResetCurrentStage()
    {
        _artifactManager?.ResetRun();
        ResetCurrentStageOnly();
    }

    private void ResetCurrentStageOnly()
    {
        CurrentStage = 1;
    }

    public void AdvanceStage()
    {
        if (CurrentStage < MaxStage)
            CurrentStage++;
    }

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

    public void SetDifficulty(DifficultyLevel level)
    {
        CurrentDifficulty = level;
        Debug.Log($"Difficulty set to: {CurrentDifficulty}");
    }

    public float GetEnemyHealthMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 0.8f;
            case DifficultyLevel.Hard: return 1.2f;
            default: return 1.0f;
        }
    }

    public float GetEnemyCountMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 0.8f;
            case DifficultyLevel.Hard: return 1.3f;
            default: return 1.0f;
        }
    }

    public float GetEnemyDamageMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 0.8f;
            case DifficultyLevel.Hard: return 1.2f;
            default: return 1.0f;
        }
    }

    public void OnEnemyKilled()
    {
        Debug.Log("Enemy killed");
    }

    public void PlayerDied()
    {
        Debug.Log("Player died");

        _artifactManager?.ResetRun();
        ResetCurrentStageOnly();

        SceneManager.LoadScene("DefeatScene");
    }

    public void LevelCompleted()
    {
        Debug.Log($"Level {CurrentStage} completed");

        if (CurrentStage < MaxStage)
        {
            SceneManager.LoadScene("LevelCompleteScene");
        }
        else
        {
            _artifactManager?.ResetRun();
            ResetCurrentStageOnly();

            SceneManager.LoadScene("FinalWinScene");
        }
    }
}
