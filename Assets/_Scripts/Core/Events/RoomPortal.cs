using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    [Header("Куда телепортировать игрока")]
    public Transform targetSpawnPoint;

    [Header("Анти-пинг-понг (глобально для всех порталов)")]
    public float teleportCooldown = 0.2f;

    private static float globalLastTeleportTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (targetSpawnPoint == null)
        {
            Debug.LogWarning($"RoomPortal ({name}): targetSpawnPoint не назначен");
            return;
        }

        // защита на случай, если статика пережила Play (Time.time снова с нуля)
        if (Time.time < globalLastTeleportTime)
            globalLastTeleportTime = -999f;

        // глобальный кд, чтобы не телепортировать сразу назад/туда же
        if (Time.time - globalLastTeleportTime < teleportCooldown)
            return;

        other.transform.position = targetSpawnPoint.position;
        globalLastTeleportTime = Time.time;

        Room targetRoom = targetSpawnPoint.GetComponentInParent<Room>();
        if (targetRoom != null && RoomCameraController.Instance != null)
            RoomCameraController.Instance.SetRoom(targetRoom);
    }
}
