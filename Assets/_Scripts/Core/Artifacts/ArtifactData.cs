using UnityEngine;

public enum ArtifactEffectType
{
    None,
    SparkOfHope,
    Blessing,
    GhostShield,
    ShadowCloud
}

[CreateAssetMenu(menuName = "GreenClouds/Artifact")]
public class ArtifactData : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public Sprite icon;

    public ArtifactEffectType effectType = ArtifactEffectType.None;
}
