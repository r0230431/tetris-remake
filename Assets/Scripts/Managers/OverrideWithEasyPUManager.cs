using System.Collections;
using UnityEngine;

public class OverrideWithEasyPUManager : MonoBehaviour
{
    public bool IsOverrideActive = false; // Houdt bij of de override actief is - Als dit zo is mag de routine niet opnieuw starten
    public int DefaultLevelToStartPU = 5; // Standaard level om de power-up te activeren
    private int levelToStartPU; // Pas vanaf level 5 wordt de power-up geactiveerd
    public float OverrideWithEasyDuration = 10f; // Duur van de override in seconden (10 seconden)
    public float OverrideWithEasyChancePerCheck = 0.2f; // Na elke 20 seconden wordt bepaald of de override wordt geactiveerd (20% kans)
    public float OverrideWithEasyCheckInterval = 20f;   // elke 20 seconden kans op activatie
    
    private bool isRoutineRunning = false;
    private Coroutine triggerRoutineCoroutine = null; // Coroutine nodig om iets tijdsgebonden te af te handelen

    private void Start()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        // Bepaal vanaf welk level de power-up mag starten: dit wordt bepaald door de speler en is minimum 1
        // Als de speler dit niet ingeeft, wordt het standaard op level 5 gezet
        // Hierbij wordt ook rekening gehouden met het maximum level dat werd ingesteld (en default 10)
        levelToStartPU = Mathf.Clamp(PlayerPrefs.GetInt("PowerUpLevel", DefaultLevelToStartPU), 1, PlayerPrefs.GetInt("MaxLevel", scoreManager != null ? scoreManager.DefaultMaxLevel : 10));
        CheckForOverrideActivation();
    }

    private void Update()
    {
        CheckForOverrideActivation();
    }

    void CheckForOverrideActivation()
    {
        // Bepalen of de power-up mag draaien (enkel bij Hard mode en als het spel actief is)
        bool isHardMode = PlayerPrefs.GetString("GameDifficulty", "Easy") == GameDifficulty.Hard.ToString();
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        bool gameIsActive = gameManager != null && gameManager.CurrentStatus == GameStatus.Playing;
        
        if (isHardMode && gameIsActive && !isRoutineRunning)
        {
            isRoutineRunning = true;
            triggerRoutineCoroutine = StartCoroutine(TriggerOverrideWithEasyRoutine());
        }
        else if ((!isHardMode || !gameIsActive) && isRoutineRunning)
        {
            isRoutineRunning = false;

            // Stop de coroutine als die aan het lopen is
            if (triggerRoutineCoroutine != null)
            {
                StopCoroutine(triggerRoutineCoroutine);
                triggerRoutineCoroutine = null;
            }
        }
    }
    
    //Als de Power-up mag draaien (Hard mode), moet er nog nagekenen wanneer deze in de game mag geactiveerd worden (vanaf de opgegeven level)
    private IEnumerator TriggerOverrideWithEasyRoutine()
    {
        while (isRoutineRunning)
        {
            yield return new WaitForSeconds(OverrideWithEasyCheckInterval);

            if (!isRoutineRunning) break; // Check of de routine nog steeds op true staat en mag doorgaan

            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            GameManager gameManager = FindFirstObjectByType<GameManager>();

            // Activatievoorwaarden controleren (spel is actief en speler heeft het vereiste level bereikt)
            bool canActivateOverride = scoreManager != null && scoreManager.CurrentLevel >= levelToStartPU &&
                                      gameManager != null && gameManager.CurrentStatus == GameStatus.Playing;

            // Als aan de voorwaarden is voldaan en er nog geen override actief is, kans berekenen om te activeren
            if (canActivateOverride && !IsOverrideActive)
            {
                float randomValue = Random.Range(0f, 1f); // Willekeurige waarde tussen 0 en 1

                if (randomValue <= OverrideWithEasyChancePerCheck) // Hier 20% kans op activatie dus wanneer randomvalue <= 0.2f
                {
                    //Wanneer power-up actief is, moet de override met de Easy mode gedurende een bepaalde tijd geactiveerd worden
                    StartCoroutine(OverrideWithEasyRoutine());
                }
            }
        }
        
        // Waarden resetten wanneer de routine eindigt
        isRoutineRunning = false;
        triggerRoutineCoroutine = null;
    }

    private IEnumerator OverrideWithEasyRoutine()
    {
        GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();

        IsOverrideActive = true;

        // Toon bericht dat power-up is geactiveerd (naar Easy mode)

        if (gameSceneUI != null) 
        {
            gameSceneUI.ShowGameMessage("Easy mode active!", Color.deepPink);
        }

        // Speel sound effect af voor het activeren van de power-up
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(audioManager.PowerUpSFX);
        }

        yield return new WaitForSeconds(OverrideWithEasyDuration);

        IsOverrideActive = false;

        // Toon bericht dat power-up is gedeactiveerd (terug naar Hard mode)
        if (gameSceneUI != null)
        {
            gameSceneUI.ShowGameMessage("Back to Hard mode!", Color.red);
        }

        // Speel sound effect af voor het deactiveren van de power-up (terug moeilijk)
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(audioManager.DifficultyAlarmSFX);
        }

    }
}
