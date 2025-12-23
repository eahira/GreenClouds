using UnityEngine;

public class UltimateCircleVFX : MonoBehaviour
{
    public float duration = 0.4f;
    private float timer;

    private void OnEnable()
    {
        timer = duration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            gameObject.SetActive(false);
    }
}
