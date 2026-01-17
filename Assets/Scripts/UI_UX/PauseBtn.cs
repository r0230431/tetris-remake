using UnityEngine;

public class PauseBtn : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image playIcon;
    [SerializeField]
    private UnityEngine.UI.Image pauseIcon;

    private void Start()
    {
        pauseIcon.enabled = true;
        playIcon.enabled = false;
        UpdateUI();
    }

    public void OnPauseButtonClicked()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.TogglePause();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            bool isPaused = gameManager.CurrentStatus == GameStatus.Paused;

            if (pauseIcon != null)
            {
                pauseIcon.enabled = !isPaused;
            }

            if (playIcon != null)
            {
                playIcon.enabled = isPaused;
            }
        }
    }
}
