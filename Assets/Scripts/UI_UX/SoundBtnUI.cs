using UnityEngine;

public class SoundBtnUI : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image volumeOnIcon;
    [SerializeField]
    private UnityEngine.UI.Image volumeOffIcon;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        UpdateUI();
    }

    public void OnSoundButtonClicked()
    {
        if (audioManager != null)
        {
            audioManager.ToggleMute();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (audioManager == null)
        {
            audioManager = FindFirstObjectByType<AudioManager>();
        }

        if (audioManager != null)
        {
            bool isMuted = audioManager.IsMuted();
            
            if (volumeOnIcon != null)
            {
                volumeOnIcon.enabled = !isMuted;
            }
            
            if (volumeOffIcon != null)
            {
                volumeOffIcon.enabled = isMuted;

            }
        }
    }
    
}