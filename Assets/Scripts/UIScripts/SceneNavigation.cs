using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//!!! potrebno poredati scene u File/Build Settings da bi ovo radilo !!!

public class SceneNavigation : MonoBehaviour
{
    [Header("Optional Button References")]
    public Button nextButton;
    public Button backButton;
    public Button quitButton;

    // sljedeca scena (+1)
    public void GoToNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // provjeri jel postoji sljedeca
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("last scene");
        }
    }

    // prosla scena (-1)
    public void GoToPreviousScene()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

        // provjeri jel postoji
        if (previousSceneIndex >= 0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.Log("prva scena");
        }
    }

    // quit
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}