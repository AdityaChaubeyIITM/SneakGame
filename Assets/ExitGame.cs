using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // This method will be called by the button's OnClick event
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // This will show in the editor for testing
        Application.Quit();     // Quits the game when built
    }
}
