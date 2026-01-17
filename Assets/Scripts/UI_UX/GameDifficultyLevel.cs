using UnityEngine;

public class GameDifficultyLevel : MonoBehaviour
{

    public void SetDifficultyToEasy()
    {
        PlayerPrefs.SetString("GameDifficulty", GameDifficulty.Easy.ToString());
        GoToMainMenu();
    }

    public void SetDifficultyToMedium()
    {
        PlayerPrefs.SetString("GameDifficulty", GameDifficulty.Medium.ToString());
        GoToMainMenu();
    }

    public void SetDifficultyToHard()
    {
        PlayerPrefs.SetString("GameDifficulty", GameDifficulty.Hard.ToString());
        GoToMainMenu();
    }

    private void GoToMainMenu()
    {
        SceneChanger sceneChanger = FindFirstObjectByType<SceneChanger>();
        if (sceneChanger != null)
        {
            sceneChanger.GoToMainMenuScene();
        }
    }
}
