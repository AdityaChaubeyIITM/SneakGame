using UnityEngine;
using UnityEngine.SceneManagement;  // Required to load scenes

public class PlayAgain : MonoBehaviour
{
    // Name of the scene to load again
    public string sceneToLoad = "SampleScene 1";

    public void RestartGame()
    {
        Debug.Log("Loading Scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
