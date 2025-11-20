using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    public static void Spawn(Vector3 position, int damage)
    {
        Debug.Log("Урон: " + damage + " в точке " + position);
        // Позже здесь будет красивый визуальный эффект
    }
}
