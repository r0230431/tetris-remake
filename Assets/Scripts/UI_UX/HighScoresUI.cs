using UnityEngine;

public class HighScoresUI : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI TopScoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI TopLevelText;
    [SerializeField]
    private TMPro.TextMeshProUGUI LinesText;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (TopScoreText != null)
        {
            TopScoreText.text = PlayerPrefs.GetInt("TopScore", 0).ToString();
        }

        if (TopLevelText != null)
        {
            TopLevelText.text = PlayerPrefs.GetInt("TopLevel", 0).ToString();
        }

        if (LinesText != null)
        {
            LinesText.text = PlayerPrefs.GetInt("MaxLines", 0).ToString();
        }
    }
    
    public void ClearHighScores()
    {
        PlayerPrefs.SetInt("TopScore", 0);
        PlayerPrefs.SetInt("TopLevel", 0);
        PlayerPrefs.SetInt("MaxLines", 0);
        PlayerPrefs.Save();
        UpdateUI();
    }
}
