using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private const string PauseSceneName = "PauseScene";

    private bool IsPauseLoaded =>
        SceneManager.GetSceneByName(PauseSceneName).isLoaded;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPauseLoaded)
                OpenPause();
            else
                ClosePause();
        }
    }

    public void OpenPause()
    {
        if (IsPauseLoaded) return;

        Time.timeScale = 0f;
        SceneManager.LoadSceneAsync(PauseSceneName, LoadSceneMode.Additive);
    }

    public void ClosePause()
    {
        if (!IsPauseLoaded) return;

        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(PauseSceneName);
    }
}
