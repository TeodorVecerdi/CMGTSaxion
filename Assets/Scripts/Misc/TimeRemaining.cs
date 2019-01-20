using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeRemaining : MonoBehaviour {
    public Slider Slider;
    public Image FillImage;
    public Image BackgroundImage;
    public TextMeshProUGUI TimeRemainingText;

    public float TimeLeft = 60f;
    public float StartingTimeLeft = 60f;
    public float OnCompletionBonus = 60f;

    private bool shouldRun;

    private void Start() {
        TimeLeft = StartingTimeLeft;
        UpdateUI();
    }

    private void Update() {
        if (shouldRun) {
            TimeLeft -= Time.deltaTime;
            UpdateUI();
        }
    }

    private void UpdateUI() {
        TimeRemainingText.text = TimeLeft.ToString("n2") + " seconds left";

        float precentageLeft = TimeLeft / StartingTimeLeft;

        Color sliderColor = Color.green;
        if (precentageLeft < 0.5f) sliderColor = Color.yellow;
        if (precentageLeft < 0.25f) sliderColor = Color.red;

        sliderColor.a = FillImage.color.a;
        FillImage.color = sliderColor;
        Slider.value = Mathf.Clamp01(precentageLeft);
    }
    
    public void StartTimer() {
        shouldRun = true;
    }

    public void StopTimer() {
        shouldRun = false;
    }
}