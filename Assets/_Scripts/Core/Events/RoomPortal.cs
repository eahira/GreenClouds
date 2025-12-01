using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    [Header("Куда телепортировать игрока")]
    public Transform targetSpawnPoint;

    // анти-пинг-понг
    private static float lastTeleportTime = -999f;
    public float teleportCooldown = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // если только что уже телепортились — выходим
        if (Time.time - lastTeleportTime < teleportCooldown)
            return;

        if (targetSpawnPoint == null)
        {
            Debug.LogWarning("RoomPortal: targetSpawnPoint не назначен у " + name);
            return;
        }

        other.transform.position = targetSpawnPoint.position;
        lastTeleportTime = Time.time;

        Debug.Log($"RoomPortal: телепортируем игрока через {name} в {targetSpawnPoint.position}");
    }
}