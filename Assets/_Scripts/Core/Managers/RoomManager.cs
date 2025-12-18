using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Room Prefab")]
    [SerializeField] private GameObject roomPrefab;

    [Header("Rooms Count")]
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;

    [Header("Grid")]
    [SerializeField] private int gridSizeX = 10;
    [SerializeField] private int gridSizeY = 10;

    [Header("Room World Size (must match your room prefab)")]
    [SerializeField] private int roomWidth = 20;
    [SerializeField] private int roomHeight = 12;

    [Header("Player")]
    public Transform player;

    [Header("Stage Themes (optional)")]
    public StageTheme stage1Theme;
    public StageTheme stage2Theme;
    public StageTheme stage3Theme;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;

    private int roomCount;
    private bool generationComplete = false;

    private void Start()
    {
        ApplyStageSettings();

        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void ApplyStageSettings()
    {
        if (GameManager.Instance == null) return;

        int stage = GameManager.Instance.CurrentStage;

        switch (stage)
        {
            case 1:
                minRooms = 8;
                maxRooms = 12;
                break;
            case 2:
                minRooms = 12;
                maxRooms = 18;
                break;
            case 3:
                minRooms = 16;
                maxRooms = 22;
                break;
        }
    }

    private void Update()
    {
        if (!generationComplete && roomQueue.Count > 0 && roomCount < maxRooms)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (!generationComplete && roomCount < minRooms)
        {
            Debug.Log("RoomCount was less than the minimum amount of rooms. Trying again");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;

            SetupRoomsCombatAndBoss();
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        if (!IsInsideGrid(roomIndex)) return;

        roomQueue.Enqueue(roomIndex);
        roomGrid[roomIndex.x, roomIndex.y] = 1;
        roomCount++;

        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";

        var roomScript = initialRoom.GetComponent<Room>();
        if (roomScript != null)
            roomScript.RoomIndex = roomIndex;

        ApplyThemeToRoom(initialRoom);

        roomObjects.Add(initialRoom);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        if (!IsInsideGrid(roomIndex))
            return false;

        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms)
            return false;

        // уже есть комната
        if (roomGrid[x, y] != 0)
            return false;

        // шанс не создавать комнату (чтобы лабиринт был “дырявый”)
        if (Random.value < 0.5f)
            return false;

        // чтобы не было слишком много связей (как у тебя)
        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.name = $"Room-{roomCount}";

        var roomScript = newRoom.GetComponent<Room>();
        if (roomScript != null)
            roomScript.RoomIndex = roomIndex;

        ApplyThemeToRoom(newRoom);

        roomObjects.Add(newRoom);

        // ВАЖНО: двери открываем после добавления в список, чтобы GetRoomScriptAt нашёл соседей
        OpenDoors(newRoom, x, y);

        return true;
    }

    private void RegenerateRooms()
    {
        for (int i = 0; i < roomObjects.Count; i++)
        {
            if (roomObjects[i] != null)
                Destroy(roomObjects[i]);
        }

        roomObjects.Clear();
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        roomGrid = new int[gridSizeX, gridSizeY];

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void OpenDoors(GameObject roomObj, int x, int y)
    {
        Room newRoom = roomObj.GetComponent<Room>();
        if (newRoom == null) return;

        Room leftRoom = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoom = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoom = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoom = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // LEFT neighbor
        if (leftRoom != null)
        {
            newRoom.OpenDoor(Vector2Int.left);
            leftRoom.OpenDoor(Vector2Int.right);

            if (newRoom.LeftPortal != null && leftRoom.EntryFromRight != null)
                newRoom.LeftPortal.targetSpawnPoint = leftRoom.EntryFromRight;

            if (leftRoom.RightPortal != null && newRoom.EntryFromLeft != null)
                leftRoom.RightPortal.targetSpawnPoint = newRoom.EntryFromLeft;
        }

        // RIGHT neighbor
        if (rightRoom != null)
        {
            newRoom.OpenDoor(Vector2Int.right);
            rightRoom.OpenDoor(Vector2Int.left);

            if (newRoom.RightPortal != null && rightRoom.EntryFromLeft != null)
                newRoom.RightPortal.targetSpawnPoint = rightRoom.EntryFromLeft;

            if (rightRoom.LeftPortal != null && newRoom.EntryFromRight != null)
                rightRoom.LeftPortal.targetSpawnPoint = newRoom.EntryFromRight;
        }

        // BOTTOM neighbor
        if (bottomRoom != null)
        {
            newRoom.OpenDoor(Vector2Int.down);
            bottomRoom.OpenDoor(Vector2Int.up);

            if (newRoom.BottomPortal != null && bottomRoom.EntryFromTop != null)
                newRoom.BottomPortal.targetSpawnPoint = bottomRoom.EntryFromTop;

            if (bottomRoom.TopPortal != null && newRoom.EntryFromBottom != null)
                bottomRoom.TopPortal.targetSpawnPoint = newRoom.EntryFromBottom;
        }

        // TOP neighbor
        if (topRoom != null)
        {
            newRoom.OpenDoor(Vector2Int.up);
            topRoom.OpenDoor(Vector2Int.down);

            if (newRoom.TopPortal != null && topRoom.EntryFromBottom != null)
                newRoom.TopPortal.targetSpawnPoint = topRoom.EntryFromBottom;

            if (topRoom.BottomPortal != null && newRoom.EntryFromTop != null)
                topRoom.BottomPortal.targetSpawnPoint = newRoom.EntryFromTop;
        }
    }

    private Room GetRoomScriptAt(Vector2Int index)
    {
        for (int i = 0; i < roomObjects.Count; i++)
        {
            var obj = roomObjects[i];
            if (obj == null) continue;

            Room r = obj.GetComponent<Room>();
            if (r != null && r.RoomIndex == index)
                return r;
        }
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        int count = 0;

        if (IsInsideGrid(new Vector2Int(x - 1, y)) && roomGrid[x - 1, y] != 0) count++;
        if (IsInsideGrid(new Vector2Int(x + 1, y)) && roomGrid[x + 1, y] != 0) count++;
        if (IsInsideGrid(new Vector2Int(x, y - 1)) && roomGrid[x, y - 1] != 0) count++;
        if (IsInsideGrid(new Vector2Int(x, y + 1)) && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private bool IsInsideGrid(Vector2Int idx)
    {
        return idx.x >= 0 && idx.x < gridSizeX && idx.y >= 0 && idx.y < gridSizeY;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(
            roomWidth * (gridX - gridSizeX / 2),
            roomHeight * (gridY - gridSizeY / 2),
            0f
        );
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = new Vector3(
                    roomWidth * (x - gridSizeX / 2),
                    roomHeight * (y - gridSizeY / 2),
                    0f
                );
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }

    // -------------------- THEMES --------------------
    private StageTheme GetCurrentTheme()
    {
        int s = GameManager.Instance != null ? GameManager.Instance.CurrentStage : 1;
        if (s == 2) return stage2Theme != null ? stage2Theme : stage1Theme;
        if (s == 3) return stage3Theme != null ? stage3Theme : stage1Theme;
        return stage1Theme;
    }

    private void ApplyThemeToRoom(GameObject roomObj)
    {
        var visuals = roomObj.GetComponent<RoomVisuals>();
        if (visuals == null) return;

        StageTheme theme = GetCurrentTheme();
        visuals.ApplyTheme(theme);
    }

    // -------------------- COMBAT + BOSS --------------------
    private void SetupRoomsCombatAndBoss()
    {
        if (roomObjects == null || roomObjects.Count == 0)
            return;

        // ищем игрока, если не назначен
        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("RoomManager: player не назначен, комбат в комнатах не будет настроен");
            return;
        }

        // стартовая — первая созданная
        Room startRoom = roomObjects[0].GetComponent<Room>();
        if (startRoom == null)
            return;

        Vector2Int startIndex = startRoom.RoomIndex;

        if (RoomCameraController.Instance != null)
            RoomCameraController.Instance.SetRoom(startRoom);

        // находим самую дальнюю комнату
        GameObject bossRoomObj = null;
        int maxDistance = -1;

        foreach (var roomObj in roomObjects)
        {
            if (roomObj == null) continue;

            Room r = roomObj.GetComponent<Room>();
            if (r == null) continue;

            int dist = Mathf.Abs(r.RoomIndex.x - startIndex.x) +
                       Mathf.Abs(r.RoomIndex.y - startIndex.y);

            if (dist > maxDistance)
            {
                maxDistance = dist;
                bossRoomObj = roomObj;
            }
        }

        // настраиваем боёвку в каждой комнате
        foreach (var roomObj in roomObjects)
        {
            if (roomObj == null) continue;

            RoomCombatController combat = roomObj.GetComponent<RoomCombatController>();
            if (combat == null) continue;

            bool isBoss = (roomObj == bossRoomObj);
            combat.Setup(player, isBoss);
        }

        if (bossRoomObj != null)
            Debug.Log("Boss room: " + bossRoomObj.name);
    }
}
