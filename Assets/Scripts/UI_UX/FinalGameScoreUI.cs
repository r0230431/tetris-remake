using UnityEngine;

public class FinalGameScoreUI : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI FinalGameScoreText;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        int finalScore = PlayerPrefs.GetInt("GameFinalScore", 0);

        
        if (FinalGameScoreText != null)
        {
            FinalGameScoreText.text = $"Your score: {finalScore}";
        }
    }
}
