using UnityEngine;

public class ShadowCloudEffect : MonoBehaviour
{
    [Header("Config")]
    public float radius = 2.5f;
    public float stunDuration = 1.2f;
    public float cooldown = 3.0f;

    [Header("Layers")]
    public LayerMask enemyMask;

    private float lastProcTime = -999f;
    private ArtifactManager am;

    private void Awake()
    {
        if (enemyMask.value == 0)
            enemyMask = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        am = GameManager.Instance != null ? GameManager.Instance.GetComponent<ArtifactManager>() : null;
    }

    public void OnPlayerDamaged()
    {
        if (am == null) return;
        if (!am.HasArtifact(ArtifactEffectType.ShadowCloud)) return;
        if (Time.time - lastProcTime < cooldown) return;

        lastProcTime = Time.time;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        for (int i = 0; i < hits.Length; i++)
        {
            Enemy e = hits[i].GetComponentInParent<Enemy>();
            if (e != null)
                e.Stun(stunDuration);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
