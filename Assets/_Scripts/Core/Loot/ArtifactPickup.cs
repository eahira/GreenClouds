using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ArtifactPickup : MonoBehaviour
{
    [Header("Data")]
    public ArtifactData data;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer iconRenderer;

    private void Awake()
    {
        if (iconRenderer == null)
            iconRenderer = GetComponentInChildren<SpriteRenderer>();

        ApplyVisual();
    }

    public void Init(ArtifactData newData)
    {
        data = newData;
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (iconRenderer == null) return;

        if (data != null && data.icon != null)
            iconRenderer.sprite = data.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var am = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;
        if (am == null) return;

        am.Pickup(data);
        AudioManager.Instance?.PlayArtifactPickup();

        Destroy(gameObject);
    }
}
