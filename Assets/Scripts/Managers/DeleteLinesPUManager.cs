using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteLinesPUManager : MonoBehaviour
{
    public float RandomLineDeleteIntervalMin = 20f; // Minimum van de interval range in seconden
    public float RandomLineDeleteIntervalMax = 40f; // Maximum van de interval range in seconden
    public int MinLinesToDelete = 1;
    public int MaxLinesToDelete = 3;
    public int DefaultLevelToStartPU = 5;
    private int levelToStartPU;
    private bool isRoutineRunning = false;
    private Coroutine deleteLineCoroutine = null;

    private void Start()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        // Bepaal vanaf welk level de power-up mag starten: dit wordt bepaald door de speler en is minimum 1
        // Als de speler dit niet ingeeft, wordt het standaard op level 5 gezet
        // Hierbij wordt ook rekening gehouden met het maximum level dat werd ingesteld (en default 10)
        levelToStartPU = Mathf.Clamp(PlayerPrefs.GetInt("PowerUpLevel", DefaultLevelToStartPU), 1, PlayerPrefs.GetInt("MaxLevel", scoreManager != null ? scoreManager.DefaultMaxLevel : 10));
        CheckAndStartRoutine();
    }

    private void Update()
    {
        CheckAndStartRoutine();
    }

    private void CheckAndStartRoutine()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        // De power up voor het deleten van lijnen mag enkel worden geactiveerd in Medium of Hard modus, en vanaf het ingestelde level
        bool shouldRunRoutine = (PlayerPrefs.GetString("GameDifficulty", "Easy") == GameDifficulty.Medium.ToString() ||
                               PlayerPrefs.GetString("GameDifficulty", "Easy") == GameDifficulty.Hard.ToString()) &&
                               scoreManager != null && scoreManager.CurrentLevel >= levelToStartPU &&
                               gameManager != null && gameManager.CurrentStatus == GameStatus.Playing;

        // De power up mag draaien maar is niet aan het draaien
        if (shouldRunRoutine && !isRoutineRunning)
        {
            isRoutineRunning = true;
            deleteLineCoroutine = StartCoroutine(RandomLineRemovalRoutine());
        }
        else if (!shouldRunRoutine && isRoutineRunning) // de power up mag niet draaien maar is wel aan het draaien
        {
            isRoutineRunning = false;
            if (deleteLineCoroutine != null)
            {
                StopCoroutine(deleteLineCoroutine);
                deleteLineCoroutine = null;
            }
        }
    }

    public IEnumerator RandomLineRemovalRoutine()
    {
        while (true)
        {
            // Er wordt een willekeurige tijd bepaald waarna de routine in gang wordt gezet
            float waitTime = Random.Range(RandomLineDeleteIntervalMin, RandomLineDeleteIntervalMax);
            yield return new WaitForSeconds(waitTime);

            // Als de tijd verstreken is, wordt nagegaan of het spel nog actief is
            // Als het spel niet meer actief is, overslaan
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null || gameManager.CurrentStatus != GameStatus.Playing)
            {
                continue;
            }

            //Als het spel nog actief is, kunnen er lijnen verwijderd word  en
            GridManager gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager != null)
            {
                // Enkel rijen met blokken selecteren
                List<int> rowsWithBlocks = FindRowsWithBlocks();
                // Pas als er rijen met blokken zijn, mogen er lijnen verwijderd worden
                if (rowsWithBlocks.Count > 0)
                {
                    SelectRowsToDelete(rowsWithBlocks);
                }
            }
        }

    }

    private List<int> FindRowsWithBlocks()
    {
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        List<int> rowsWithBlocks = new List<int>();

        if (gridManager != null)
        {
            for (int y = 0; y < gridManager.Height; y++) // voor elke rij in de grid
            {
                bool hasBlocks = false;
                for (int x = 0; x < gridManager.Width; x++) // kijken of er in die rij blokken staan
                {
                    if (gridManager.Grid[x, y] != null) // Als er 1 blok in de rij staat, komt de rij in aanmerking
                    {
                        hasBlocks = true;
                        break;
                    }
                }

                if (hasBlocks)
                {
                    rowsWithBlocks.Add(y); // rij toevoegen aan de lijst met mogelijk rijen die verwijderd kunnen worden
                }
            }
        }

        return rowsWithBlocks;
    }

    private void SelectRowsToDelete(List<int> rowsWithBlocks)
    {
        // Bericht tonen dat power-up is geactiveerd
        GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
        if (gameSceneUI != null)
        {
            gameSceneUI.ShowGameMessage("Lines Removed!", Color.deepPink); 
        }

        // Geluid afspelen dat power-up is geactiveerd
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(audioManager.PowerUpSFX);
        }
        
        // Bepaal het aantal lijnen dat verwijderd zal worden: max. 3, min. 1 tenzij er geen rijen met blokken zijn
        int linesToDelete = Mathf.Min(Random.Range(MinLinesToDelete, MaxLinesToDelete + 1), rowsWithBlocks.Count);

        // Rijen verwijderen, te beginnen van onder naar boven om index verschuiving te vermijden
        rowsWithBlocks.Sort();
        for (int i = linesToDelete - 1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, rowsWithBlocks.Count);
            int rowToDelete = rowsWithBlocks[randomIndex];
            rowsWithBlocks.RemoveAt(randomIndex);

            GridManager gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager != null)
            {
                gridManager.DeleteLine(rowToDelete);
                gridManager.MoveAllLinesDown(rowToDelete + 1);
            }
        
            for (int j = 0; j < rowsWithBlocks.Count; j++)
            {
                if (rowsWithBlocks[j] > rowToDelete)
                {
                    rowsWithBlocks[j]--;
                }
            }
        }
    }
}
