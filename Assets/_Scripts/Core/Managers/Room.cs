using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Двери (визуальные объекты)")]
    [SerializeField] private GameObject topDoor;
    [SerializeField] private GameObject bottomDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [Header("Порталы внутри дверей")]
    [SerializeField] private RoomPortal topPortal;
    [SerializeField] private RoomPortal bottomPortal;
    [SerializeField] private RoomPortal leftPortal;
    [SerializeField] private RoomPortal rightPortal;

    [Header("Точки входа в комнату")]
    [SerializeField] private Transform entryFromTop;
    [SerializeField] private Transform entryFromBottom;
    [SerializeField] private Transform entryFromLeft;
    [SerializeField] private Transform entryFromRight;

    // индекс комнаты в сетке RoomManager-а
    public Vector2Int RoomIndex { get; set; }

    // геттеры, чтобы RoomManager мог получать доступ
    public RoomPortal TopPortal => topPortal;
    public RoomPortal BottomPortal => bottomPortal;
    public RoomPortal LeftPortal => leftPortal;
    public RoomPortal RightPortal => rightPortal;

    public Transform EntryFromTop => entryFromTop;
    public Transform EntryFromBottom => entryFromBottom;
    public Transform EntryFromLeft => entryFromLeft;
    public Transform EntryFromRight => entryFromRight;

    /// <summary>
    /// Открывает визуальные двери в нужном направлении
    /// (используется при генерации, чтобы прорубать проходы).
    /// </summary>
    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up && topDoor != null)
            topDoor.SetActive(true);

        if (direction == Vector2Int.down && bottomDoor != null)
            bottomDoor.SetActive(true);

        if (direction == Vector2Int.left && leftDoor != null)
            leftDoor.SetActive(true);

        if (direction == Vector2Int.right && rightDoor != null)
            rightDoor.SetActive(true);
    }

    /// <summary>
    /// Включает/выключает сами порталы (триггеры телепортации).
    /// Выключенными будут серыми в иерархии.
    /// </summary>
    public void SetPortalsActive(bool active)
    {
        if (topPortal != null)
            topPortal.gameObject.SetActive(active);
        if (bottomPortal != null)
            bottomPortal.gameObject.SetActive(active);
        if (leftPortal != null)
            leftPortal.gameObject.SetActive(active);
        if (rightPortal != null)
            rightPortal.gameObject.SetActive(active);
    }
}
