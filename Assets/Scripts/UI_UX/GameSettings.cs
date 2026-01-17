using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] 
    private TMPro.TMP_InputField maxLevelInput;

    [SerializeField]
    private TMPro.TMP_InputField linesPerLevelInput;

    [SerializeField]
    private TMPro.TMP_InputField powerUpLevelInput;

    [SerializeField]
    private UnityEngine.UI.Slider volumeSlider;

    private void Start()
    {
        // Laden van spelervoorkeuren uit PlayerPrefs
        int maxLevel = PlayerPrefs.GetInt("MaxLevel", 10);
        int linesPerLevel = PlayerPrefs.GetInt("LinesPerLevel", 10);
        int powerUpLevel = PlayerPrefs.GetInt("PowerUpLevel", 5);
        float volume = PlayerPrefs.GetFloat("Volume", 0.8f);
        
        // Spelervoorkeuren in UI zetten
        maxLevelInput.text = maxLevel.ToString();
        linesPerLevelInput.text = linesPerLevel.ToString();
        powerUpLevelInput.text = powerUpLevel.ToString();
        volumeSlider.value = volume;

        // Event listeners toevoegen aan de inputvelden en slider
        maxLevelInput.onEndEdit.AddListener(OnMaxLevelChanged);
        linesPerLevelInput.onEndEdit.AddListener(OnLinesPerLevelChanged);
        powerUpLevelInput.onEndEdit.AddListener(OnPowerUpLevelChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnMaxLevelChanged(string input)
    {
        // Probeer de invoer om te zetten naar een integer
        if (int.TryParse(input, out int chosenMaxLevel))
        {
            // Beperk de waarde (bijv. 1–100)
            chosenMaxLevel = Mathf.Clamp(chosenMaxLevel, 1, 100);
            // Spelervoorkeur opslaan
            PlayerPrefs.SetInt("MaxLevel", chosenMaxLevel);
            PlayerPrefs.Save();
        }
    }

    private void OnLinesPerLevelChanged(string input)
    {
        // Probeer de invoer om te zetten naar een integer
        if (int.TryParse(input, out int chosenLinesPerLevel))
        {
            // Beperk de waarde (bijv. 1–100)
            chosenLinesPerLevel = Mathf.Clamp(chosenLinesPerLevel, 1, 100);
            // Spelervoorkeur opslaan
            PlayerPrefs.SetInt("LinesPerLevel", chosenLinesPerLevel);
            PlayerPrefs.Save();
        }
    }

    private void OnPowerUpLevelChanged(string input)
    {
        // Probeer de invoer om te zetten naar een integer
        if (int.TryParse(input, out int chosenPowerUpLevel))
        {
            // Beperk de waarde (bijv. 1–maxLevel)
            chosenPowerUpLevel = Mathf.Clamp(chosenPowerUpLevel, 1, PlayerPrefs.GetInt("MaxLevel", 10));
            // Spelervoorkeur opslaan
            PlayerPrefs.SetInt("PowerUpLevel", chosenPowerUpLevel);
            PlayerPrefs.Save();
        }
    }

    private void OnVolumeChanged(float volume)
    {
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetVolume(volume);
        }
        // Spelervoorkeur opslaan
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }
}
