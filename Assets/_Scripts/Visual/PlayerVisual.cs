using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisual : MonoBehaviour
{
    [Header("Skins")]
    public PlayerSkinData survivorSkin;
    public PlayerSkinData robotSkin;
    public PlayerSkinData angelSkin;

    [Header("Tuning")]
    public float moveSpeedThreshold = 0.01f;   // порог движения
    public float walkFps = 8f;                // скорость “шагания” (2 кадра)
    public float flapFps = 6f;                // скорость крыльев

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private PlayerSkinData skin;
    private FacingDir facing = FacingDir.Down;

    private Vector2 prevPos;
    private Vector2 lastMoveDelta;
    private float speedSqr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        prevPos = rb != null ? rb.position : (Vector2)transform.position;
    }

    private void Start()
    {
        ApplySelectedSkin();
        SetIdleSprite();
    }

    private void FixedUpdate()
    {
        UpdateRealSpeedAndDelta();
        UpdateFacingFromDelta();
        UpdateSprite();
    }

    public void ApplySelectedSkin()
    {
        CharacterType type = GameManager.Instance != null ? GameManager.Instance.selectedCharacter : CharacterType.Survivor;

        skin = type switch
        {
            CharacterType.Robot => robotSkin != null ? robotSkin : survivorSkin,
            CharacterType.Angel => angelSkin != null ? angelSkin : survivorSkin,
            _ => survivorSkin
        };
    }

    private void UpdateRealSpeedAndDelta()
    {
        Vector2 curPos = rb != null ? rb.position : (Vector2)transform.position;

        // delta за кадр физики — работает и с MovePosition
        Vector2 delta = curPos - prevPos;
        lastMoveDelta = delta;

        float dt = Time.fixedDeltaTime;
        Vector2 v = dt > 0 ? (delta / dt) : Vector2.zero;
        speedSqr = v.sqrMagnitude;

        prevPos = curPos;
    }

    private void UpdateFacingFromDelta()
    {
        if (speedSqr <= moveSpeedThreshold * moveSpeedThreshold)
            return;

        Vector2 d = lastMoveDelta;
        if (d.sqrMagnitude < 0.0000001f) return;

        // выбираем главную ось
        if (Mathf.Abs(d.x) > Mathf.Abs(d.y))
            facing = d.x >= 0 ? FacingDir.Right : FacingDir.Left;
        else
            facing = d.y >= 0 ? FacingDir.Up : FacingDir.Down;
    }

    private void UpdateSprite()
    {
        if (skin == null) return;

        bool moving = speedSqr > moveSpeedThreshold * moveSpeedThreshold;

        // ANGEL: флап всегда
        if (skin.useFlap && skin.flapFrames != null && skin.flapFrames.Length >= 2)
        {
            int idx = (int)(Time.time * flapFps) % skin.flapFrames.Length;
            sr.sprite = skin.flapFrames[idx];
            return;
        }

        // WALK (Survivor): 2 кадра, если они заданы
        if (moving)
        {
            Sprite[] walk = GetWalkArrayByFacing();
            if (walk != null && walk.Length >= 2)
            {
                int idx = (int)(Time.time * walkFps) % walk.Length;
                sr.sprite = walk[idx];
                return;
            }
        }

        // иначе idle по направлению (в т.ч. Robot)
        SetIdleSprite();
    }

    private void SetIdleSprite()
    {
        if (skin == null) return;

        sr.sprite = facing switch
        {
            FacingDir.Up => skin.idleUp != null ? skin.idleUp : skin.idleDown,
            FacingDir.Left => skin.idleLeft != null ? skin.idleLeft : skin.idleDown,
            FacingDir.Right => skin.idleRight != null ? skin.idleRight : skin.idleDown,
            _ => skin.idleDown
        };
    }

    private Sprite[] GetWalkArrayByFacing()
    {
        return facing switch
        {
            FacingDir.Up => skin.walkUp,
            FacingDir.Left => skin.walkLeft,
            FacingDir.Right => skin.walkRight,
            _ => skin.walkDown
        };
    }
}
