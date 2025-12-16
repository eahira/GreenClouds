using System.Collections.Generic;
using UnityEngine;

public class ArtifactEffectSystem : MonoBehaviour
{
    [Header("SparkOfHope")]
    [Tooltip("Максимальный бонус к урону при HP=0. 1.0 = +100%")]
    public float sparkMaxBonus = 1.0f;

    private bool hasSpark;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        ArtifactManager.OnArtifactsChanged += HandleArtifactsChanged;
    }

    private void OnDisable()
    {
        ArtifactManager.OnArtifactsChanged -= HandleArtifactsChanged;
    }

    private void Start()
    {
        // на случай, если артефакты уже есть
        var am = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;
        if (am != null) HandleArtifactsChanged(am.Artifacts);
    }

    private void HandleArtifactsChanged(IReadOnlyList<ArtifactData> artifacts)
    {
        hasSpark = false;

        if (artifacts == null) return;

        for (int i = 0; i < artifacts.Count; i++)
        {
            var a = artifacts[i];
            if (a == null) continue;

            if (a.effectType == ArtifactEffectType.SparkOfHope)
                hasSpark = true;
        }
    }

    /// <summary>
    /// Модифицируем урон клика с учётом артефактов
    /// </summary>
    public int ModifyClickDamage(int baseDamage)
    {
        if (player == null) return baseDamage;

        float damage = baseDamage;

        if (hasSpark)
        {
            float hpRatio = (player.maxHealth > 0) ? (float)player.currentHealth / player.maxHealth : 1f;
            float missing = 1f - Mathf.Clamp01(hpRatio);        // 0..1
            float mult = 1f + sparkMaxBonus * missing;          // 1..(1+sparkMaxBonus)
            damage *= mult;
        }

        return Mathf.Max(1, Mathf.RoundToInt(damage));
    }
}
