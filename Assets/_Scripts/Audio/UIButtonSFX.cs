using UnityEngine;

public class UIButtonSFX : MonoBehaviour
{
    public void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();
    }
}
