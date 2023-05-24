using TMPro;
using UnityEngine;

public class WatchTimer: MonoBehaviour
{
    public TMP_Text timeText;
    private float startTime;

    private void Start()
    {
        // Record the start time
        startTime = Time.time;
    }

    private void Update()
    {
        // Calculate the elapsed time
        float elapsedTime = Time.time - startTime;

        // Format the elapsed time as hours, minutes, and seconds
        string minutes = ((int)elapsedTime / 60 % 60).ToString("00");
        string seconds = (elapsedTime % 60).ToString("00");

        // Update the text of the TMP component
        timeText.text = minutes + ":" + seconds;
    }
}