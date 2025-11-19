using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public GameObject lootPrefab;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Враг получил урон! Здоровье: " + health);

        if (health <= 0)
        {
            DropLoot();
            Die();
        }
    }

    void DropLoot()
    {
        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
            Debug.Log("Выпал лут!");
        }
    }

    void Die()
    {
        Debug.Log("Враг умер!");
        Destroy(gameObject);
    }
}