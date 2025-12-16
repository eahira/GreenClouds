using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + healAmount);
        PlayerEvents.OnPlayerHealthChanged?.Invoke(player.currentHealth, player.maxHealth);

        Destroy(gameObject);
    }
}
