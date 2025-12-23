using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}