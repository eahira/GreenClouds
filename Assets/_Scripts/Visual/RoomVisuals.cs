using UnityEngine;

public class RoomVisuals : MonoBehaviour
{
    [Header("Links")]
    public SpriteRenderer backgroundRenderer;

    public SpriteRenderer blockLeft;
    public SpriteRenderer blockRight;
    public SpriteRenderer blockTop;
    public SpriteRenderer blockBottom;

    // 🔹 запоминаем последний фон (глобально для всех комнат)
    private static Sprite lastBackgroundSprite;

    public void ApplyTheme(StageTheme theme)
    {
        if (theme == null) return;

        // ---------- ФОН ----------
        if (backgroundRenderer != null &&
            theme.roomBackgrounds != null &&
            theme.roomBackgrounds.Length > 0)
        {
            Sprite chosen = null;

            // если только 1 вариант — берём его
            if (theme.roomBackgrounds.Length == 1)
            {
                chosen = theme.roomBackgrounds[0];
            }
            else
            {
                // выбираем случайный, но не такой же как прошлый
                int safety = 10; // защита от бесконечного цикла
                do
                {
                    chosen = theme.roomBackgrounds[
                        Random.Range(0, theme.roomBackgrounds.Length)
                    ];
                    safety--;
                }
                while (chosen == lastBackgroundSprite && safety > 0);
            }

            backgroundRenderer.sprite = chosen;
            lastBackgroundSprite = chosen;
        }

        // ---------- БЛОКЕРЫ ДВЕРЕЙ ----------
        Sprite doorSprite = theme.doorBlockerSprite;

        if (blockLeft != null) blockLeft.sprite = doorSprite;
        if (blockRight != null) blockRight.sprite = doorSprite;
        if (blockTop != null) blockTop.sprite = doorSprite;
        if (blockBottom != null) blockBottom.sprite = doorSprite;
    }

    public void SetDoorBlocked(Vector2Int dir, bool blocked)
    {
        if (dir == Vector2Int.left && blockLeft != null) blockLeft.enabled = blocked;
        if (dir == Vector2Int.right && blockRight != null) blockRight.enabled = blocked;
        if (dir == Vector2Int.up && blockTop != null) blockTop.enabled = blocked;
        if (dir == Vector2Int.down && blockBottom != null) blockBottom.enabled = blocked;
    }
}
