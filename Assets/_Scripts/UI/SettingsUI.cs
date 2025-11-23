using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}