using UnityEngine;

[CreateAssetMenu(menuName = "GreenClouds/Stage Theme")]
public class StageTheme : ScriptableObject
{
    public Sprite[] roomBackgrounds;     // 15 фонов
    public Sprite doorBlockerSprite;     // “блок травы” для дверей
}
