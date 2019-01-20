using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public Slider Slider;
    public Image FillImage;
    public Image BackgroundImage;
    public TextMeshProUGUI TimeRemainingText;

    public float TimeRemaining = 60f;
    public float StartingTimeLeft = 60f;
    public float NextLevelBonus = 45f;
    public bool TimeOut;
    
    private bool shouldRun;

    private void Start() {
        TimeRemaining = StartingTimeLeft;
        UpdateUI();
    }

    private void Update() {
        if (shouldRun) {
            TimeRemaining -= Time.deltaTime;
            UpdateUI();
            if (TimeRemaining <= 0) {
                TimeOut = true;
                TimeRemaining = 0;
                StopTimer();
            }
        }
    }

    private void UpdateUI() {
        TimeRemainingText.text = TimeRemaining.ToString("n2") + " seconds left";

        float precentageLeft = TimeRemaining / StartingTimeLeft;

        Color sliderColor = Color.green;
        if (precentageLeft < 0.5f) sliderColor = Color.yellow;
        if (precentageLeft < 0.25f) sliderColor = Color.red;

        sliderColor.a = FillImage.color.a;
        FillImage.color = sliderColor;
        Slider.value = Mathf.Clamp01(precentageLeft);
    }

    public void NewLevel() {
        StartTimer();
        TimeRemaining = 60f;
    }
    
    public void StartTimer() {
        shouldRun = true;
    }

    public void StopTimer() {
        shouldRun = false;
    }
}