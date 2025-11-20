using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnPlayPressed()
    {
        // пока стыкуем к SampleScene — потом заменим на LevelSelect
        SceneManager.LoadScene("SampleScene");
    }

    public void OnShopPressed()
    {
        SceneManager.LoadScene("ShopScene"); // создадим позже
    }

    public void OnSettingsPressed()
    {
        SceneManager.LoadScene("SettingsScene"); // создадим позже
    }

    public void OnExitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}