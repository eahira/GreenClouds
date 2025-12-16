using UnityEngine;

public class CurrentRoomTracker : MonoBehaviour
{
    public static Room CurrentRoom { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CurrentRoom = GetComponentInParent<Room>();

        // можно сразу обновлять камеру, если нужно
        if (CurrentRoom != null && RoomCameraController.Instance != null)
            RoomCameraController.Instance.SetRoom(CurrentRoom);
    }
}
