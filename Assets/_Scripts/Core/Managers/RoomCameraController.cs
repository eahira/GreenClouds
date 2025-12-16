using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance;

    public float smoothSpeed = 5f;
    private Vector3 targetPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    public void SetRoom(Room room)
    {
        Vector3 pos = room.transform.position;
        pos.z = transform.position.z;
        targetPosition = pos;
    }
}
