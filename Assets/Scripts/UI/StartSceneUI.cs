using UnityEngine;
using TMPro;

public class StartSceneUI : MonoBehaviour
{
    public TextMeshProUGUI level1HighScoreText;

    void Start()
    {
        // Load the saved values (default to 0 if nothing is saved yet)
        int highScore = PlayerPrefs.GetInt("HighScore1", 0);
        float highTime = PlayerPrefs.GetFloat("HighTime1", 0f);

        // Format the time to mm:ss:msms
        int minutes = Mathf.FloorToInt(highTime / 60F);
        int seconds = Mathf.FloorToInt(highTime - minutes * 60f);
        int milliseconds = Mathf.FloorToInt((highTime - Mathf.FloorToInt(highTime)) * 1000f);
        string timeString = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

        // Update the UI Text
        if (level1HighScoreText != null)
        {
            level1HighScoreText.text = "High Score: " + highScore.ToString("D6") + " Time: " + timeString;
        }
    }
}