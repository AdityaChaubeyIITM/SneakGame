using UnityEngine;

public class SleepPotionPickup : MonoBehaviour
{
    private bool collected = false;

    public void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Enable potion use for the player
            SleepPotionManager.instance.OnPotionPickup();

            // Destroy potion object
            Destroy(gameObject);
        }
    }
}
