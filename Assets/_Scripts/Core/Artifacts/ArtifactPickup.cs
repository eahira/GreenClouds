using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    public ArtifactData data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var am = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;
        if (am == null) return;

        am.Pickup(data);
        Destroy(gameObject);
    }

}
