using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SceneManager.LoadScene("CharacterSelectorScene");
    }

    public void OnShopPressed()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void OnTutorialPressed()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnSettingsPressed()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void OnExitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}