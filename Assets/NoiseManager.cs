using UnityEngine;
using UnityEngine.UI;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager instance;

    [Header("Noise Settings")]
    public float maxNoise = 100f;
    public float noiseDecreaseRate = 1f; // per half second

    private float currentNoise = 0f;
    private float noiseDecreaseTimer = 0.5f;

    [Header("UI")]
    public Slider noiseSlider;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (noiseSlider != null)
        {
            noiseSlider.maxValue = maxNoise;
            noiseSlider.value = currentNoise;
        }
    }

    private void Update()
    {
        if (currentNoise > 0f)
        {
            noiseDecreaseTimer -= Time.deltaTime;
            if (noiseDecreaseTimer <= 0f)
            {
                currentNoise = Mathf.Max(0f, currentNoise - noiseDecreaseRate);
                noiseDecreaseTimer = 0.5f;
                UpdateUI();

                if (SleepPotionManager.instance != null && SleepPotionManager.instance.IsPotionActive() && currentNoise >= maxNoise)
                {
                    SleepPotionManager.instance.SendMessage("EndPotionEffect", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    public void AddNoise(float amount)
    {
        currentNoise = Mathf.Clamp(currentNoise + amount, 0f, maxNoise);
        UpdateUI();
    }

    public void ResetNoise()
    {
        currentNoise = 0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (noiseSlider != null)
        {
            noiseSlider.value = currentNoise;
        }
    }

    public float GetCurrentNoise()
    {
        return currentNoise;
    }
}
