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

    [Header("Блокеры проходов (DoorBlocks/*)")]
    [SerializeField] private GameObject topBlock;
    [SerializeField] private GameObject bottomBlock;
    [SerializeField] private GameObject leftBlock;
    [SerializeField] private GameObject rightBlock;

    public Vector2Int RoomIndex { get; set; }

    public RoomPortal TopPortal => topPortal;
    public RoomPortal BottomPortal => bottomPortal;
    public RoomPortal LeftPortal => leftPortal;
    public RoomPortal RightPortal => rightPortal;

    public Transform EntryFromTop => entryFromTop;
    public Transform EntryFromBottom => entryFromBottom;
    public Transform EntryFromLeft => entryFromLeft;
    public Transform EntryFromRight => entryFromRight;

    private bool hasTop, hasBottom, hasLeft, hasRight;

    private void Awake()
    {
        // ВАЖНО: по твоему ТЗ — пока не зачищена, порталы и блокеры выключены
        SetCleared(false);
        SnapBlocksToDoorways();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SnapBlocksToDoorways();
    }
#endif

    // RoomManager вызывает это при генерации, чтобы “прорубить” проходы
    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            hasTop = true;
            if (topDoor != null) topDoor.SetActive(true);
        }
        else if (direction == Vector2Int.down)
        {
            hasBottom = true;
            if (bottomDoor != null) bottomDoor.SetActive(true);
        }
        else if (direction == Vector2Int.left)
        {
            hasLeft = true;
            if (leftDoor != null) leftDoor.SetActive(true);
        }
        else if (direction == Vector2Int.right)
        {
            hasRight = true;
            if (rightDoor != null) rightDoor.SetActive(true);
        }

        // НЕ включаем порталы/блокеры тут — они зависят от зачистки
        SetPortalsActive(false);
        SetDoorBlocksActive(false);

        SnapBlocksToDoorways();
    }

    public void SetCleared(bool cleared)
    {
        // твоя логика:
        // не зачищена => порталы OFF, блокеры OFF
        // зачищена   => порталы ON,  блокеры ON
        SetPortalsActive(cleared);
        SetDoorBlocksActive(cleared);
    }

    public void SetPortalsActive(bool active)
    {
        if (topPortal != null) topPortal.gameObject.SetActive(active && hasTop);
        if (bottomPortal != null) bottomPortal.gameObject.SetActive(active && hasBottom);
        if (leftPortal != null) leftPortal.gameObject.SetActive(active && hasLeft);
        if (rightPortal != null) rightPortal.gameObject.SetActive(active && hasRight);
    }

    public void SetDoorBlocksActive(bool active)
    {
        if (topBlock != null) topBlock.SetActive(active && hasTop);
        if (bottomBlock != null) bottomBlock.SetActive(active && hasBottom);
        if (leftBlock != null) leftBlock.SetActive(active && hasLeft);
        if (rightBlock != null) rightBlock.SetActive(active && hasRight);
    }

    private void SnapBlocksToDoorways()
    {
        if (topBlock != null)
        {
            Transform t = (topPortal != null) ? topPortal.transform : (topDoor != null ? topDoor.transform : null);
            if (t != null) topBlock.transform.position = t.position;
        }

        if (bottomBlock != null)
        {
            Transform t = (bottomPortal != null) ? bottomPortal.transform : (bottomDoor != null ? bottomDoor.transform : null);
            if (t != null) bottomBlock.transform.position = t.position;
        }

        if (leftBlock != null)
        {
            Transform t = (leftPortal != null) ? leftPortal.transform : (leftDoor != null ? leftDoor.transform : null);
            if (t != null) leftBlock.transform.position = t.position;
        }

        if (rightBlock != null)
        {
            Transform t = (rightPortal != null) ? rightPortal.transform : (rightDoor != null ? rightDoor.transform : null);
            if (t != null) rightBlock.transform.position = t.position;
        }
    }
}
