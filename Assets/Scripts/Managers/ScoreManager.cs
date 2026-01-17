using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int CurrentScore = 0;
    public int CurrentLevel = 1;
    public int CurrentLinesCleared = 0;

    public int DefaultMaxLevel = 10;
    public int DefaultLinesPerLevel = 10;
    private int linesPerLevel;
    private int maxLevel;

    private void Awake()
    {
        ResetScore();
        // Laden van instellingen uit PlayerPrefs die via de game settings kunnen worden ingesteld
        linesPerLevel = PlayerPrefs.GetInt("LinesPerLevel", DefaultLinesPerLevel);
        maxLevel = PlayerPrefs.GetInt("MaxLevel", DefaultMaxLevel);
    }

    public void CalculateScore(int lines)
    {
        int points = 0;
        switch (lines)
        {
            case 1:
                points = 40;
                break;
            case 2:
                points = 100;
                break;
            case 3:
                points = 300;
                break;
            case 4:
                points = 1200;
                break;
        }

        CurrentScore += points * CurrentLevel;
        CurrentLinesCleared += lines;

        // Check of je een level omhoog gaat
        if (CurrentLinesCleared >= CurrentLevel * linesPerLevel && CurrentLevel < maxLevel)
        {
            LevelUp();
        }

        // Einde spel als je het maximum level hebt bereikt EN alle lijnen van die en level hebt gehaald
        if (CurrentLevel == maxLevel && CurrentLinesCleared >= maxLevel * linesPerLevel)
        {
            SaveGameFinalScore();
            UpdateHighScores();

            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.EndOfGame();
            }
        }
    }

    void LevelUp()
    {
        CurrentLevel++;

        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();

        // Toon bericht op het scherm bij level up
        if (gameSceneUI != null)
        {
            gameSceneUI.ShowGameMessage("Level Up!", Color.green);
        }

        // Speel soundeffect bij level up
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(audioManager.LevelUpSFX);
        }

        // Versnel het spel bij elke level up
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.IncreaseSpeed();
        }
    }


    public void SaveGameFinalScore()
    {
        PlayerPrefs.SetInt("GameFinalScore", CurrentScore);
        PlayerPrefs.Save();
    }

    public void UpdateHighScores()
    {
        SaveHighScore();
        SaveTopLevel();
        SaveMaximumLines();
    }

    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("TopScore", 0);
        if (CurrentScore > highScore)
        {
            PlayerPrefs.SetInt("TopScore", CurrentScore);
            PlayerPrefs.Save();
        }
    }

    public void SaveTopLevel()
    {
        int topLevel = PlayerPrefs.GetInt("TopLevel", 0);
        if (CurrentLevel > topLevel)
        {
            PlayerPrefs.SetInt("TopLevel", CurrentLevel);
            PlayerPrefs.Save();
        }
    }

    public void SaveMaximumLines()
    {
        int maxLines = PlayerPrefs.GetInt("MaxLines", 0);
        if (CurrentLinesCleared > maxLines)
        {
            PlayerPrefs.SetInt("MaxLines", CurrentLinesCleared);
            PlayerPrefs.Save();
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        CurrentLevel = 1;
        CurrentLinesCleared = 0;
    }

}
