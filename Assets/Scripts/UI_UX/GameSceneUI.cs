using UnityEngine;
using System.Collections;

public class GameSceneUI : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI ScoreText;
    [SerializeField]
    private TMPro.TextMeshProUGUI LevelText;
    [SerializeField]
    private TMPro.TextMeshProUGUI LinesText;
    [SerializeField]
    public TMPro.TextMeshProUGUI GameMessageText;
    private Coroutine currentMessageCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if(GameMessageText != null)
            GameMessageText.text = "";
        UpdateUI();
    }

    public void ShowGameMessage(string message, Color color, float duration = 2f, bool stayUntilUnpause = false)
    {
        // Stop eventuele vorige boodschap als die er is
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        // Start nieuwe boodschap
        currentMessageCoroutine = StartCoroutine(ShowMessageCoroutine(message, color, duration, stayUntilUnpause));
    }

    private IEnumerator ShowMessageCoroutine(string text, Color color, float duration = 2f, bool stayUntilUnpause = false)
    {
        GameMessageText.text = text;
        GameMessageText.color = color;
        GameMessageText.gameObject.SetActive(true); // Zorg dat de boodschap zichtbaar is

        if (stayUntilUnpause)
        {
            // Wacht totdat de boodschap handmatig wordt verwijderd (bijvoorbeeld bij hervatten van het spel) - hier enkel bij pauzeren
            yield break;
        } else
        {
            // Wacht de opgegeven duur voordat de boodschap wordt verwijderd (meestal)
            yield return new WaitForSeconds(duration);
        }

        GameMessageText.text = "";
        GameMessageText.gameObject.SetActive(false); // Verberg de boodschap
    }

    public void UpdateUI()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            ScoreText.text = scoreManager.CurrentScore.ToString();
            LevelText.text = scoreManager.CurrentLevel.ToString();
            LinesText.text = scoreManager.CurrentLinesCleared.ToString();
        }
        else
        {
            if (ScoreText != null) ScoreText.text = "0";
            if (LevelText != null) LevelText.text = "1";
            if (LinesText != null) LinesText.text = "0";
        }
    }
}