using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;

    int roomWidth = 20;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    public Transform player;  // Ссылка на игрока

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
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("RoomCount was less than the minimum amount of rooms. Trying again");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;

            // Вызовем метод настройки боёвки и босса
            SetupRoomsCombatAndBoss();
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        var roomScript = initialRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms)
            return false;

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        return true;
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // ЛЕВЫЙ СОСЕД
        if (x > 0 && roomGrid[x - 1, y] != 0 && leftRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);

            if (newRoomScript.LeftPortal != null && leftRoomScript.EntryFromRight != null)
                newRoomScript.LeftPortal.targetSpawnPoint = leftRoomScript.EntryFromRight;

            if (leftRoomScript.RightPortal != null && newRoomScript.EntryFromLeft != null)
                leftRoomScript.RightPortal.targetSpawnPoint = newRoomScript.EntryFromLeft;
        }

        // ПРАВЫЙ СОСЕД
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0 && rightRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);

            if (newRoomScript.RightPortal != null && rightRoomScript.EntryFromLeft != null)
                newRoomScript.RightPortal.targetSpawnPoint = rightRoomScript.EntryFromLeft;

            if (rightRoomScript.LeftPortal != null && newRoomScript.EntryFromRight != null)
                rightRoomScript.LeftPortal.targetSpawnPoint = newRoomScript.EntryFromRight;
        }

        // НИЖНИЙ СОСЕД
        if (y > 0 && roomGrid[x, y - 1] != 0 && bottomRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);

            if (newRoomScript.BottomPortal != null && bottomRoomScript.EntryFromTop != null)
                newRoomScript.BottomPortal.targetSpawnPoint = bottomRoomScript.EntryFromTop;

            if (bottomRoomScript.TopPortal != null && newRoomScript.EntryFromBottom != null)
                bottomRoomScript.TopPortal.targetSpawnPoint = newRoomScript.EntryFromBottom;
        }

        // ВЕРХНИЙ СОСЕД
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0 && topRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);

            if (newRoomScript.TopPortal != null && topRoomScript.EntryFromBottom != null)
                newRoomScript.TopPortal.targetSpawnPoint = topRoomScript.EntryFromBottom;

            if (topRoomScript.BottomPortal != null && newRoomScript.EntryFromTop != null)
                topRoomScript.BottomPortal.targetSpawnPoint = newRoomScript.EntryFromTop;
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++; 
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; 
        if (y > 0 && roomGrid[x, y - 1] != 0) count++; 
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; 

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }

    // Новый метод для настройки боёвки и босс-рума
    void SetupRoomsCombatAndBoss()
    {
        if (roomObjects == null || roomObjects.Count == 0)
            return;

        // ищем игрока, если не назначен в инспекторе
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

        // считаем, что первая созданная комната — стартовая
        Room startRoom = roomObjects[0].GetComponent<Room>();
        if (startRoom == null)
            return;

        Vector2Int startIndex = startRoom.RoomIndex;

        if (RoomCameraController.Instance != null)
            RoomCameraController.Instance.SetRoom(startRoom);

        // ищем самую дальнюю комнату от стартовой по манхэттенскому расстоянию
        GameObject bossRoomObj = null;
        int maxDistance = -1;

        foreach (var roomObj in roomObjects)
        {
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

        // настраиваем боёвку во всех комнатах
        foreach (var roomObj in roomObjects)
        {
            RoomCombatController combat = roomObj.GetComponent<RoomCombatController>();
            if (combat == null) continue;

            bool isBoss = (roomObj == bossRoomObj);
            combat.Setup(player, isBoss);
        }

        if (bossRoomObj != null)
            Debug.Log("Boss room: " + bossRoomObj.name);
    }
}
