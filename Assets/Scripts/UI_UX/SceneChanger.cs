using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void GoToGameDifficultyScene()
    {
        SceneManager.LoadScene("GameDifficultyScene");
    }

    public void GoToMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void GoToGameSettingsScene()
    {
        SceneManager.LoadScene("GameSettingsScene");
    }


    public void GoToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToHighScoresScene()
    {
        SceneManager.LoadScene("HighScoresScene");
    }

    public void GoToGameOverSceneWithDelay(float delay)
    {
        StartCoroutine(LoadSceneWithDelay("GameOverScene", delay));
    }

    public void GoToEndOfGameSceneWithDelay(float delay)
    {
        StartCoroutine(LoadSceneWithDelay("YouWinScene", delay));
    }

    private System.Collections.IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}