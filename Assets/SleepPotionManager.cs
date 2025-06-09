using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SleepPotionManager : MonoBehaviour
{
    public static SleepPotionManager instance;

    [Header("Potion Settings")]
    public float sleepDuration = 10f;
    public AudioClip potionPickupSound;

    [Header("Noise Settings")]
    public float noiseDecayRate = 1f; // Decrease per 0.5 seconds
    public float maxNoise = 100f;
    public float crouchNoiseRate = 5f;
    public float walkNoiseRate = 10f;
    public float sprintNoiseRate = 20f;

    private float currentNoise = 0f;
    private bool isPotionActive = false;
    private float potionTimer = 0f;

    private ThirdPersonController player;
    private AudioSource audioSource;

    private bool canUsePotion = false;

    [Header("UI Elements")]
    public Slider noiseSlider;
    public TextMeshProUGUI timerText;
    public GameObject potionUICanvas;

    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        player = FindObjectOfType<ThirdPersonController>();
        if (player == null)
        {
            Debug.LogError("ThirdPersonController not found in scene.");
        }

        // Initialize UI
        if (noiseSlider != null)
        {
            noiseSlider.maxValue = maxNoise;
            noiseSlider.value = 0f;
        }

        if (timerText != null)
        {
            timerText.text = "";
        }

        if (potionUICanvas != null)
        {
            potionUICanvas.SetActive(false); // Hide UI at start
        }
    }

    void Update()
    {
        if (!isPotionActive && canUsePotion && Input.GetKeyDown(KeyCode.G))
        {
            ActivatePotion();
        }

        if (isPotionActive)
        {
            potionTimer -= Time.deltaTime;
            UpdateNoise();
            UpdateUI();

            if (potionTimer <= 0 || currentNoise >= maxNoise)
            {
                EndPotionEffect();
            }
        }
    }

    private void UpdateNoise()
    {
        float noiseRate = 0f;

        if (player.IsSprinting)
            noiseRate = sprintNoiseRate;
        else if (player.IsCrouching)
            noiseRate = crouchNoiseRate;
        else
            noiseRate = walkNoiseRate;

        if (Mathf.Abs(player.MovementInput.magnitude) > 0.01f)
        {
            currentNoise += noiseRate * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);
        }

        // Decay noise every 0.5s
        if (Time.frameCount % 30 == 0)
        {
            currentNoise -= noiseDecayRate;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);
        }

        Debug.Log("Noise: " + currentNoise.ToString("F0"));
    }

    private void UpdateUI()
    {
        if (noiseSlider != null)
        {
            noiseSlider.value = currentNoise;
        }

        if (timerText != null)
        {
            timerText.text = "Timer: " + potionTimer.ToString("F1") + "s";
        }
    }

    private void ActivatePotion()
    {
        if (!canUsePotion) return;

        isPotionActive = true;
        potionTimer = sleepDuration;
        currentNoise = 0f;
        EnemySleepController.SetAllSleeping(true);

        if (potionUICanvas != null)
            potionUICanvas.SetActive(true);

        Debug.Log("Sleep potion activated!");
    }

    private void EndPotionEffect()
    {
        isPotionActive = false;
        canUsePotion = false;
        EnemySleepController.SetAllSleeping(false);

        if (timerText != null)
            timerText.text = "";

        if (potionUICanvas != null)
            potionUICanvas.SetActive(false);

        Debug.Log("Sleep potion effect ended.");
    }

    public void EnablePotionUse()
    {
        canUsePotion = true;
    }

    public bool IsPotionActive()
    {
        return isPotionActive;
    }

    public void OnPotionPickup()
    {
        EnablePotionUse();

        if (audioSource && potionPickupSound)
        {
            audioSource.PlayOneShot(potionPickupSound);
        }

        Debug.Log("Potion picked up.");
    }
}
