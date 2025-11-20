using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Получено монет: " + amount + " | Всего: " + coins);
    }

    public void OnEnemyKilled()
    {
        Debug.Log("Враг убит");
        // Позже добавим RoomManager
    }

    public void PlayerDied()
    {
        Debug.Log("Игрок умер!");
        // Позже добавим UI экран смерти
    }
}

