using System;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public const int MaxSlots = 6;

    [SerializeField] private List<ArtifactData> artifacts = new List<ArtifactData>(MaxSlots);
    public IReadOnlyList<ArtifactData> Artifacts => artifacts;

    // “Уже выпадало/получали в этом забеге”
    private readonly HashSet<ArtifactData> obtainedThisRun = new HashSet<ArtifactData>();

    // Чтобы эффекты/UI могли обновляться
    public static Action<IReadOnlyList<ArtifactData>> OnArtifactsChanged;

    public void ResetRun()
    {
        artifacts.Clear();
        obtainedThisRun.Clear();
        OnArtifactsChanged?.Invoke(artifacts);
    }

    // Вызываем при ДРОПЕ (чтобы не дублировалось на полу)
    public void MarkObtainedThisRun(ArtifactData data)
    {
        if (data != null) obtainedThisRun.Add(data);
    }

    public bool WasObtainedThisRun(ArtifactData data)
    {
        return data != null && obtainedThisRun.Contains(data);
    }

    // Вызывается при ПОДБОРЕ
    public void Pickup(ArtifactData data)
    {
        if (data == null) return;

        obtainedThisRun.Add(data);

        if (artifacts.Count < MaxSlots)
        {
            artifacts.Add(data);
        }
        else
        {
            artifacts.RemoveAt(0);
            artifacts.Add(data);
        }

#if UNITY_EDITOR
        Debug.Log($"[Artifacts] Now: {artifacts.Count}. Last picked: {data.title}");
#endif

        OnArtifactsChanged?.Invoke(artifacts);
    }
}
