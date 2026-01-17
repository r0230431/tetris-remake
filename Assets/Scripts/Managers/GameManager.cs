using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Tetrominos; // Lijst van alle mogelijke tetromino prefabs
    public GameObject[] Tiles; // Lijst van alle mogelijke tegel prefabs (toevoegen voor de garbage rows)
    public float MovementFrequency = 1.0f; // Basis snelheid van bewegen van de tetromino in seconden

    public float GarbageRowIntervalMin = 30f; // Minimum van de interval range in seconden
    public float GarbageRowIntervalMax = 60f; // Maximum van de interval range in seconden
    private float passedTime = 0; // Tijd die is verstreken sinds de laatste beweging van de tetromino
    private GameObject currentTetromino;
    public GameStatus CurrentStatus = GameStatus.NotPlaying;

    private void Start()
    {
        // Game status aanpassen naar Playing bij het starten van het spel
        this.CurrentStatus = GameStatus.Playing;

        // Gekozen moeilijkheidsgraad ophalen om te bepalen of de GarbageRowRoutine moet worden gestart
        string currentDifficulty = PlayerPrefs.GetString("GameDifficulty", "Easy");

        if (currentDifficulty == GameDifficulty.Medium.ToString() ||
            currentDifficulty == GameDifficulty.Hard.ToString())
        {
            StartCoroutine(GarbageRowRoutine());
        }

        // Eerste tetromino spawnen
        SpawnTetromino();
    }

    private void Update()
    {
        // Game logica stoppen als het spel voorbij is of niet aan het spelen is
        if (CurrentStatus != GameStatus.Playing)
        {
            return;
        }

        // Tijd bijhouden
        passedTime += Time.deltaTime;

        // Gebruik snellere snelheid als de pijl naar beneden ingedrukt is
        float currentSpeed = Input.GetKey(KeyCode.DownArrow) ? 0.1f : MovementFrequency;
        
        // Het vallen van de tetromino regelen op basis van de tijd
        if (passedTime >= currentSpeed)
        {
            passedTime -= currentSpeed;
            MoveTetromino(Vector3.down);
        }

        UserInput();
    }

    private void UserInput()
    {
        string selectedDifficulty = PlayerPrefs.GetString("GameDifficulty", "Easy");
        OverrideWithEasyPUManager overrideManager = FindFirstObjectByType<OverrideWithEasyPUManager>();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(selectedDifficulty == GameDifficulty.Hard.ToString() // Als de moeilijkheid Hard is dan moet links eigenlijk rechts worden
                && (overrideManager == null || !overrideManager.IsOverrideActive)) // Tenzij de power-up actief is
            {
                MoveTetromino(Vector3.right);
            }
            else
            {
                MoveTetromino(Vector3.left);
            }

        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(selectedDifficulty == GameDifficulty.Hard.ToString() // Als de moeilijkheid Hard is dan moet rechts eigenlijk links worden
                && (overrideManager == null || !overrideManager.IsOverrideActive)) // Tenzij de power-up actief is
            {
                MoveTetromino(Vector3.left);
            }
            else
            {
                MoveTetromino(Vector3.right);
            }

        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveTetromino(Vector3.down);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Huidige positie en rotatie van de tetromino opslaan
            Vector3 currentPosition = currentTetromino.transform.position;
            Quaternion currentRotation = currentTetromino.transform.rotation;
            // Standaard rotatiewaarden
            int randomRotation = 90;
            int randomDirection = 1;

            if (selectedDifficulty == GameDifficulty.Hard.ToString() // Bij Hard moeilijkheidsgraad wordt een willekeurige rotatie gekozen
                && (overrideManager == null || !overrideManager.IsOverrideActive)) // Tenzij de power-up actief is
            {
                randomRotation = Random.Range(0, 4) * 90;
                randomDirection = Random.Range(0, 2) == 0 ? 1 : -1;
            }

            currentTetromino.transform.Rotate(0, 0, randomRotation * randomDirection); // Rotatie toepassen op tetromino (draaien rond z-as)

            // Controleer of de rotatie mogelijk is, anders terugzetten naar huidige positie en rotatie
            if (!IsValidPosition())
            {
                currentTetromino.transform.position = currentPosition;
                currentTetromino.transform.rotation = currentRotation;
            }
        }
    }

    public void SpawnTetromino()
    {
        // Geen tetromino meer spawnen als het spel voorbij is
        if (CurrentStatus != GameStatus.Playing)
        {
            return;
        }

        // Startpositie voor de tetromino
        int startPosX = 5;
        int startPosY = 18;

        // Willekeurige tetromino kiezen en spawnen
        int randomIndex = Random.Range(0, Tetrominos.Length);
        currentTetromino = Instantiate(Tetrominos[randomIndex], new Vector3(startPosX, startPosY, 0), Quaternion.identity);
        
        // Controleer of er nog een tetromino geplaatst kan worden op de spawnpositie
        if (!IsValidPosition())
        {
            // Game over - het grid is vol en er kunnen geen nieuwe stukken meer worden geplaatst
            GameOver();
        }
    }

    private void MoveTetromino(Vector3 direction)
    {
        // Als het game over is, geen tetromino's meer verplaatsen
        if (CurrentStatus != GameStatus.Playing)
        {
            return;
        }

        // Verplaats de tetromino in de opgegeven richting (op basis van userinput of automatisch vallen)
        currentTetromino.transform.position += direction;
        
        if (!IsValidPosition())
        {
            currentTetromino.transform.position -= direction; // Verplaatsing niet mogelijk dus de coördinaten van de tetromino terugzetten als de positie niet geldig is
            
            // Als de tetromino niet kan verplaatsen en de speler heeft aangegeven dat de tetromino naar beneden moet bewegen,
            // betekent dit dat de tetromino is geland
            if (direction == Vector3.down)
            {
                GridManager gridManager = FindFirstObjectByType<GridManager>();
                if (gridManager != null)
                {
                    gridManager.UpdateGrid(currentTetromino.transform); // Bij elke beweging van de tetromino moet de grid worden bijgewerkt
                    gridManager.CheckForLines(); // Controleren op volle lijnen nadat de tetromino is geland
                }
                AudioManager audioManager = FindFirstObjectByType<AudioManager>();
                // Speel soundeffect af bij landen van tetromino
                if (audioManager != null)
                {
                    audioManager.PlaySoundEffect(audioManager.TetrominoLandedSFX);
                }
                // Nieuwe tetromino spawnen
                SpawnTetromino();
            }
        }
    }

    private bool IsValidPosition()
    {
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            return gridManager.IsValidPosition(currentTetromino.transform);
        }
        return false;
    }

    public void IncreaseSpeed()
    {
        // Verhoog snelheid met 10%
        MovementFrequency *= 0.9f;
    }

    public IEnumerator GarbageRowRoutine()
    {
        while (CurrentStatus == GameStatus.Playing) // Blijf de routine uitvoeren zolang het spel actief is
        {
            float waitTime = Random.Range(GarbageRowIntervalMin, GarbageRowIntervalMax); // Bepalen wanneer de volgende rij toegevoegd wordt
            
            yield return new WaitForSeconds(waitTime); // Als die tijd verstreken is
            
            if (CurrentStatus == GameStatus.Playing) // Eerst controleren of het spel nog actief is
            {
                // Toon bericht
                GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
                if (gameSceneUI != null)
                {
                    gameSceneUI.ShowGameMessage("Garbage line added!", Color.red);
                }

                // Speel geluidseffect af
                AudioManager audioManager = FindFirstObjectByType<AudioManager>();
                if (audioManager != null)
                {
                    audioManager.PlaySoundEffect(audioManager.DifficultyAlarmSFX);
                }

                // Voeg een garbage rij toe onderaan de grid
                GridManager gridManager = FindFirstObjectByType<GridManager>();
                if (gridManager != null)
                {
                    gridManager.AddGarbageRow();
                }
            }
        }
    }

    public void TogglePause()
    {
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
        bool isMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
        
        if (CurrentStatus == GameStatus.Playing) // Pauzeren
        {
            CurrentStatus = GameStatus.Paused;
            gameSceneUI.ShowGameMessage("Game Paused", Color.grey, 0, true); // Pauze bericht tonen en laten staan

            // Muziek ook pauzeren als deze niet uit staat
            if (!isMuted)
            {
                audioManager.TurnOffMusic(true);
            }
            Time.timeScale = 0f; // tijd pauzeren
        }
        else if (CurrentStatus == GameStatus.Paused) // Hervatten
        {
            CurrentStatus = GameStatus.Playing;
            gameSceneUI.ShowGameMessage("Game Resumed", Color.grey, 2f); // Hervatten bericht tonen en na 2 seconden laten verdwijnen
            // Muziek hervatten als deze niet uit stond
            if (!isMuted)
            {
                audioManager.TurnOffMusic(false);
            }
            Time.timeScale = 1f; // tijd hervatten
        }
    }

    public void GameOver()
    {
        // Wijzig spelstatus naar GameOver
        this.CurrentStatus = GameStatus.GameOver;

        // Toon bericht in game
        GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
        if (gameSceneUI != null)    
        {
            gameSceneUI.ShowGameMessage("Game Over!", Color.red, 3f); 
        }

        // Sla de eindscore en high scores op
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SaveGameFinalScore();
            scoreManager.UpdateHighScores();
        }

        // Speel Game Over soundeffect af
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayGameOverSoundEffect(audioManager.GameOverTheme);
        }

        // Ga na een korte pauze naar de Game Over scene
        SceneChanger sceneChanger = FindFirstObjectByType<SceneChanger>();
        if (sceneChanger != null)
        {
            sceneChanger.GoToGameOverSceneWithDelay(2.0f); // 2 second delay
        }
    }

    public void EndOfGame()
    {
        // Wijzig spelstatus naar EndOfGame
        this.CurrentStatus = GameStatus.EndOfGame;

        // Toon bericht in game
        GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
        if (gameSceneUI != null)
        {
            gameSceneUI.ShowGameMessage("You Won!", Color.deepPink, 3f); // Clear any existing message
        }
        
        // Opslag van eindscore en high scores zit in Scoremanager script

        // Speel You win soundeffect af
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayGameOverSoundEffect(audioManager.EndOfGameTheme); // Stop music, then play end game sound
        }

        // Ga na een korte pauze naar de You Win scen
        SceneChanger sceneChanger = FindFirstObjectByType<SceneChanger>();
        if (sceneChanger != null)
        {
            sceneChanger.GoToEndOfGameSceneWithDelay(2.0f); // 2 second delay
        }
    }
}
