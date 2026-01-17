using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Transform[,] Grid;
    public int Width = 10;
    public int Height = 20;

    private void Awake()
    {
        // Grid klaarzetten
        Grid = new Transform[Width, Height];
    }

    // Over elke cell in de grid gaat om te kijken of deze is opgevuld
    public void UpdateGrid(Transform tetromino)
    {
        for (int y = 0; y < Height; y++) // loopen over de rijen
        {
            
            // Eerst alle cellen leegmaken die bij de tetromino horen
            for (int x = 0; x < Width; x++) // loopen over de kolommen
            {
                if (Grid[x, y] != null) // als de cel is opgevuld
                {
                    if (Grid[x, y].parent == tetromino) // en de cel behoort tot de tetromino die beweegt
                    {
                        Grid[x, y] = null; // dan maken we de cel leeg
                    }
                }
            }

            // Daarna de nieuwe posities van de tetromino invullen in de grid
            foreach (Transform tile in tetromino) // voor elke tile in de tetromino
            {
                Vector2 pos = Round(tile.position); // we ronden de positie van de tile af
                if (pos.y < Height) // als de y-positie van de tile binnen de grid is
                {
                    Grid[(int)pos.x, (int)pos.y] = tile; // dan vullen we de cel in de grid met de tile
                }
            }
        }
    }

    public static Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public bool IsValidPosition(Transform tetromino)
    {
        foreach (Transform tile in tetromino) // voor elke tile in de tetromino
        {
            Vector2 pos = Round(tile.position); // we ronden de positie van de tile af

            // Is de positie binnen de grid?
            if (!IsInsideGrid(pos)) // als de positie van de tile niet binnen de grid is
            {
                return false; // dan is de positie niet geldig
            }

            // Is er conflict met andere tetromino's?
            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino) // als de cel in de grid is opgevuld en de tile is niet van de tetromino die we aan het checken zijn
            {
                return false; // dan is de positie niet geldig
            }
        }

        return true; // als alle tiles binnen de grid zijn en geen van hen een andere tetromino overlapt, dan is de positie geldig
    }

    // Controleert of een positie binnen de borders van de grid valt
    private bool IsInsideGrid(Vector2 pos)
    {
        return (int)pos.x >= 0 && (int)pos.x < Width && (int)pos.y >= 0 && (int)pos.y < Height;
    }


    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y >= Height)
        {
            return null;
        }
        return Grid[(int)pos.x, (int)pos.y];
    }

    public void CheckForLines()
    {
        int linesCleared = 0;

        for (int y = 0; y < Height; y++) // loopen over de rijen
        {
            if (IsLineFull(y)) // als de rij vol is
            {
                linesCleared++;
                DeleteLine(y); // dan verwijderen we de rij
                MoveAllLinesDown(y + 1); // en verplaatsen we alle rijen boven de verwijderde rij naar beneden
                y--; // we moeten y met 1 verlagen omdat we een rij hebben verwijderd
            }
        }

        // Punten toekennen voor het aantal verwijderde lijnen
        if (linesCleared > 0)
        {
            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.CalculateScore(linesCleared);

                // Update de UI van de gameScene met de nieuwe score
                GameSceneUI gameSceneUI = FindFirstObjectByType<GameSceneUI>();
                if (gameSceneUI != null)
                {
                    gameSceneUI.UpdateUI();
                }
            }
        }
    }

    private bool IsLineFull(int y)
    {
        for (int x = 0; x < Width; x++) // loopen over de kolommen
        {
            if (Grid[x, y] == null) // als een cel in de rij leeg is
            {
                return false; // dan is de rij niet vol
            }
        }

        // Speel sound effect af voor het verwijderen van een rij
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySoundEffect(audioManager.LineClearSFX);
        }

        return true; // als alle cellen in de rij zijn opgevuld, dan is de rij vol
    }

    public void DeleteLine(int y)
    {
        for (int x = 0; x < Width; x++) // loopen over de kolommen
        {
            if (Grid[x, y] != null) // als de cel in de rij is opgevuld
            {
                Destroy(Grid[x, y].gameObject); // moet de tile in die cel weggehaald worden
            }
            Grid[x, y] = null; // en maken we de cel leeg
        }
    }

    public void MoveAllLinesDown(int deletedRow)
    {
        // Verplaats alle rijen boven de verwijderde rij naar beneden met één positie
        for (int y = deletedRow; y < Height; y++) // lopen over de rijen boven de verwijderde rij
        {
            for (int x = 0; x < Width; x++) // Ga voor elke rij door elke kolom
            {

                if (Grid[x, y] != null) // Als er een tegel in de rij staat
                {
                    // Verplaats de tegel naar omlaag met één rij in de grid-array
                    Grid[x, y - 1] = Grid[x, y]; // Slaat de nieuwe positie op
                    Grid[x, y] = null; // Maak de oude positie leeg
                    Grid[x, y - 1].position += Vector3.down; // Verplaats de tile 1 rij omlaag in de grid
                }
            }
        }
    }

    public void AddGarbageRow()
    {       
        // Eerst alle bestaande rijen omhoog verplaatsen om onderaan ruimte te maken
        MoveAllLinesUp();

        // Dan een garbage row toevoegen aan de onderkant (y = 0)
        int emptyCellIndex = Random.Range(0, Width); // Zorg ervoor dat er minstens één lege cel in de rij is

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        
        for (int x = 0; x < Width; x++)
        {
            if (x != emptyCellIndex)
            {
                if (gameManager != null && gameManager.Tiles != null && gameManager.Tiles.Length > 0)
                {
                    GameObject randomTile = gameManager.Tiles[Random.Range(0, gameManager.Tiles.Length)];
                    GameObject instantiatedTile = Instantiate(randomTile, new Vector3(x, 0, 0), Quaternion.identity);
                    Grid[x, 0] = instantiatedTile.transform;
                    
                }
            }
        }
    }

    public void MoveAllLinesUp()
    {
        for (int y = Height - 1; y > 0; y--) // Van boven naar beneden over de rijen loopen
        {
            for (int x = 0; x < Width; x++) // Ga voor elke rij door elke kolom
            {
                if (Grid[x, y - 1] != null) // Als er een tegel in de rij onder de huidige rij is
                {
                    // Verplaats de tegel omhoog met één rij in de grid-array
                    Grid[x, y] = Grid[x, y - 1]; // Slaat de nieuwe positie op
                    Grid[x, y - 1] = null; // Maak de oude positie leeg
                    Grid[x, y].position += Vector3.up; // Verplaats de tile 1 rij omhoog in de grid
                }
            }
        }
    }
}
