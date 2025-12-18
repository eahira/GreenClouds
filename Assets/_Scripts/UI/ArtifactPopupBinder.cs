using UnityEngine;

public class ArtifactPopupBinder : MonoBehaviour
{
    [SerializeField] private ArtifactPopupUI popup;

    private ArtifactManager am;

    private void Awake()
    {
        if (popup == null)
            popup = Object.FindFirstObjectByType<ArtifactPopupUI>();

        if (GameManager.Instance != null)
            am = GameManager.Instance.GetComponent<ArtifactManager>();
    }

    private void OnEnable()
    {
        if (am != null)
            am.OnArtifactPicked += HandlePicked;
    }

    private void OnDisable()
    {
        if (am != null)
            am.OnArtifactPicked -= HandlePicked;
    }

    private void HandlePicked(ArtifactData data)
    {
        if (popup != null)
            popup.Show(data);
    }
}