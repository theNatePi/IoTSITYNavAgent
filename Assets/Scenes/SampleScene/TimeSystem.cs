using UnityEngine;
using UnityEngine.UI;
using System;
//using UnityEngine.UIElements;

public class TimeSystem : MonoBehaviour
{
    public float timeScale = 1f;  // 1:1 time
    public string initialTime = "08:00:00";
    private float elapsedTime = 0f;
    public DateTime simulatedTime;

    private Text timeText;

    private Image backgroundImage;

    void Start()
    {
        if (!DateTime.TryParse(initialTime, out simulatedTime))
        {
            Debug.LogWarning("Invalid initial time format. Defaulting to 08:00:00.");
            simulatedTime = new DateTime(1, 1, 1, 8, 0, 0);
        }
        
        
        CreateTimeText();
        UpdateTimeDisplay();
    }

    void Update()
    {
        // Update the simulated time based on the elapsed real-time and time scale
        elapsedTime += Time.deltaTime * timeScale;
        simulatedTime = simulatedTime.AddSeconds(Time.deltaTime * timeScale);
        UpdateTimeDisplay();
    }

    void CreateTimeText()
    {
        // make a new GameObject for the Text
        GameObject canvasObject = new GameObject("TimeCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        // bg image
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(canvasObject.transform);

        // the image component stuff
        backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = Color.black;

        // image properties
        RectTransform backgroundRectTransform = backgroundImage.GetComponent<RectTransform>();
        backgroundRectTransform.anchorMin = new Vector2(1, 1);
        backgroundRectTransform.anchorMax = new Vector2(1, 1);
        backgroundRectTransform.pivot = new Vector2(1, 1);
        backgroundRectTransform.anchoredPosition = new Vector2(-10, -10);
        backgroundRectTransform.sizeDelta = new Vector2(100, 30);  // size?


        // create a new GameObject for the Text
        GameObject textObject = new GameObject("TimeText");
        textObject.transform.SetParent(canvasObject.transform);

        // adding the text component
        timeText = textObject.AddComponent<Text>();
        timeText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        timeText.fontSize = 24;
        timeText.color = Color.white;

        // Set the Text component's RectTransform properties
        RectTransform rectTransform = timeText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-10, -10);
    }

    void UpdateTimeDisplay()
    {
        // Display the time in HH:mm:ss format
        timeText.text = simulatedTime.ToString("HH:mm:ss");
    }

    // Method to set the time scale
    public void SetTimeScale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }

    // Method to set the time
     public void SetSimulatedTime(int hours, int minutes, int seconds)
    {
        simulatedTime = new DateTime(1, 1, 1, hours, minutes, seconds);
        elapsedTime = 0f;
        UpdateTimeDisplay();
    }

}
