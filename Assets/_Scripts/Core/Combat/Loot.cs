using UnityEngine;

public class Loot : MonoBehaviour
{
    public int coins = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(coins);
            Destroy(gameObject);
        }
    }
}
