using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public AudioClip potionPickupSound;
    private AudioSource audioSource;
    public string sceneToLoad = "SampleScene";  // Replace with your actual scene name

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Make sure your player is tagged "Player"
        {
            Debug.Log("Player touched the orb. Loading EndGame scene...");
            if (audioSource && potionPickupSound)
            {
                audioSource.PlayOneShot(potionPickupSound);
            }
            Time.timeScale = 1f;  // Just in case the game was paused
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }
}
