using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private bool isPauseSceneLoaded = false;

    void Update()
    {
        // ESC переключает паузу
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPauseSceneLoaded)
                OpenPause();
            else
                ClosePause();
        }
    }

    public void OpenPause()
    {
        Time.timeScale = 0f;
        SceneManager.LoadSceneAsync("PauseScene", LoadSceneMode.Additive);
        isPauseSceneLoaded = true;
    }

    public void ClosePause()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("PauseScene");
        isPauseSceneLoaded = false;
    }
}
