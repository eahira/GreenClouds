using UnityEngine;

public enum FacingDir { Down, Up, Left, Right }

[CreateAssetMenu(menuName = "GreenClouds/Player Skin")]
public class PlayerSkinData : ScriptableObject
{
    public CharacterType characterType;

    [Header("Idle sprites (обязательные)")]
    public Sprite idleDown;
    public Sprite idleUp;
    public Sprite idleLeft;
    public Sprite idleRight;

    [Header("Walk 2-frames (для Survivor, опционально)")]
    public Sprite[] walkDown;   // 2 спрайта
    public Sprite[] walkUp;     // 2 спрайта
    public Sprite[] walkLeft;   // 2 спрайта
    public Sprite[] walkRight;  // 2 спрайта

    [Header("Angel flap (2 кадра)")]
    public bool useFlap;
    public Sprite[] flapFrames; // 2 спрайта: closed/open
}
