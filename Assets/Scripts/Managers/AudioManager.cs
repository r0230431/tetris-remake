using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip MainTheme; // Achtergrondmuziek voor menu's
    public AudioClip[] GameMusicTracks; // Achtergrondmuziek voor het spel (mmeerdere tracks)

    [Header("Sound Effects")]
    public AudioClip TetrominoLandedSFX, LineClearSFX, LevelUpSFX, DifficultyAlarmSFX, PowerUpSFX, GameOverTheme, EndOfGameTheme; // Geluidseffecten voor verschillende gebeurtenissen
    private AudioSource musicSource; // AudioSource voor achtergrondmuziek
    private AudioSource sfxSource; // AudioSource voor geluidseffecten
    private string lastSceneName = ""; // Houdt de naam van de laatste scène bij om wijzigingen te detecteren en de achtergrondmuziek te wijzigen van menu naar game en omgekeerd 
    private int currentGameMusicIndex; // Houdt bij welke track momenteel wordt afgespeeld in de GameScene

    private void Awake()
    {
        // Nakijken dat er maar één AudioManager bestaat
        AudioManager[] audioManagers = FindObjectsByType<AudioManager>(FindObjectsSortMode.None); //
        if (audioManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // De achtergrondmuziek moet blijven spelen bij het wisselen van scenes

        AudioSource[] existingSources = GetComponents<AudioSource>(); // AudioSource voor achtergrondmuziek en geluidseffecten ophalen

        currentGameMusicIndex = 0; // Als de game(scene) opstart, wordt de eerste audiotrack van de achtergrondmuziek van de games klaargezet

        // AudioSource voor achtergrondmuziek aanmaken
        musicSource = gameObject.AddComponent<AudioSource>();
        // Settings voor achtergrondmuziek instellen
        musicSource.loop = false; // Voor de gamescene mag de achtergrondmuziek niet loopen, voor menu's wordt dit later ingesteld
        musicSource.playOnAwake = false; // hangt af van de voorkeur van de speler
        musicSource.volume = PlayerPrefs.GetFloat("Volume", 0.8f); // Volume instellen op de waarde die de speler heeft ingesteld
        // indien niet ingesteld, standaard op 0.8f (max voor achtergrondmuziek, dan is er nog ruimte van 0.2f om de geluidseffecten harder te laten doorkomen)
        musicSource.mute = IsMuted();

        // AudioSource voor geluidseffecten aanmaken
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Settings voor geluidseffecten instellen
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = PlayerPrefs.GetFloat("Volume", 0.8f) == 0 ? 0 : PlayerPrefs.GetFloat("Volume", 0.8f) + 0.2f;
        sfxSource.mute = IsMuted(); // SFX follows the same mute state as music

    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume); // Zorg dat het volume tussen 0 en 1 blijft

        // Volume voor geluidseffecten en achtergrondmuziek instellen
        if (sfxSource != null)
        {
            sfxSource.volume = volume == 0 ? 0 : Mathf.Clamp01(volume + 0.2f); // Als het volume 0 is, dan ook SFX op 0 zetten, anders 0.2f hoger dan het door de speler gekozen volume (ook hier moet volume tussen 0 en 1 blijven)
        }

        if (musicSource != null)
        {
            musicSource.volume = volume; // Voor de achtergrondmuziek wordt de keuze van de speler overgenomen
        }

        // Volume settings opslaan
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public bool IsMuted()
    {
        return PlayerPrefs.GetInt("AudioMuted", 0) == 1; // Als de gebruiker heeft gekozen om audio te dempen, is AudioMuted 1 en IsMuted() geeft true terug
    }

    // Uitwerking van de SoundBtn
    public void ToggleMute()
    {
        if (musicSource != null)
        {
            if (!IsMuted()) // Speler zet muziek uit
            {
                // Speler geluidsvoorkeur aanpassen
                PlayerPrefs.SetInt("AudioMuted", 1);
                PlayerPrefs.Save();

                TurnOffMusic(true);  // Muziek afzetten
            }
            else // Speler zet muziek aan
            {
                PlayerPrefs.SetInt("AudioMuted", 0);
                PlayerPrefs.Save();

                TurnOffMusic(false); // Muziek aanzetten

                PlayMusicForCurrentScene(); // Zorgen dat de juiste muziek wordt afgespeeld voor de huidige scene
            }
        }
    }

    public void TurnOffMusic(bool mute)
    {
        // Achtergrondmuziek aan/uitzetten
        if (musicSource != null)
        {
            musicSource.mute = mute; // Muziek dempen of aanzetten
        }

        // Geluidseffecten aan/uitzetten
        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }

        // Zorg ervoor dat het gekozen volume correct wordt ingesteld bij het aanzetten van de muziek
        if (!mute)
        {
            float currentVolume = PlayerPrefs.GetFloat("Volume", 0.8f); // Volumevoorkeur speler ophalen
            SetVolume(currentVolume); // Volume instellen naar de voorkeur van de speler
        }
    }

    private void Start()
    {
        PlayMusicForCurrentScene(); // Bij het starten van de AudioManager de juiste muziek afspelen voor de huidige scene
    }

    private void Update()
    {
        // De huidige scene controleren zodat de juiste achtergrondmuziek kan worden afgespeeld
        string currentScene = SceneManager.GetActiveScene().name;

        // Als de scene is gewijzigd moet de muziek mogelijks aangepast worden
        if (currentScene != lastSceneName)
        {
            lastSceneName = currentScene;
            PlayMusicForCurrentScene();
        }

        // In de GameScene moet de achtergrondmuziek wisselen en automatisch terug bij index 0 starten als alle tracks zijn afgespeeld en de game nog niet gedaan is
        if (currentScene == "GameScene" && GameMusicTracks != null && GameMusicTracks.Length >= 1)
        {
            // Check of de game nog niet is afgelopen
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            bool gameIsActive = gameManager != null && gameManager.CurrentStatus == GameStatus.Playing;

            // Indien de muziek is gestopt en de game is nog actief, de volgende track afspelen
            if (musicSource != null && !musicSource.isPlaying && !IsMuted() && gameIsActive)
            {
                PlayNextGameTrack();
            }
        }
    }

    // Speel een specifieke audioclip af als achtergrondmuziek
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;

            // Hier wordt ingesteld dat in alle scenes waarbij het maintheme wordt meegegeven (alle scenes behalve die van de game), de muziek moet loopen
            musicSource.loop = clip == MainTheme;

            musicSource.Play();

            musicSource.volume = PlayerPrefs.GetFloat("Volume", 0.8f); // Zorg dat het volume correct is ingesteld bij het afspelen van muziek
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    private void PlayMusicForCurrentScene()
    {
        if (IsMuted()) return; // Als de muziek uitstaat moet er geen muziek worden afgespeeld

        string currentScene = SceneManager.GetActiveScene().name; // De huidige scene ophalen om te bepalen welke muziek moet worden afgespeeld

        // In alle scenes behalve de GameScene, GameOver en YouWin scene wordt het MainTheme afgespeeld
        if (currentScene == "StartScene" || currentScene == "MainMenuScene" || currentScene == "HighScores" || currentScene == "GameDifficultyScene" || currentScene == "SettingsScene")
        {
            if (MainTheme != null && (!musicSource.isPlaying || musicSource.clip != MainTheme))
            {
                PlayMusic(MainTheme);
            }
        }
        else if (currentScene == "GameOverScene" || currentScene == "YouWinScene") // Bij GameOver en YouWin geen muziek afspelen
        {
            StopMusic();
        }
        else if (currentScene == "GameScene") // Bij GameScene de game muziektracks afspelen
        {
            if (GameMusicTracks != null && GameMusicTracks.Length > 0)
            {
                // We gaan ervan uit dat er nog geen game track wordt afgespeeld
                bool isPlayingGameTrack = false;

                // Nakijken of de huidige clip een van de game tracks is
                if (musicSource.clip != null)
                {
                    for (int i = 0; i < GameMusicTracks.Length; i++)
                    {
                        if (musicSource.clip == GameMusicTracks[i])
                        {
                            isPlayingGameTrack = true;
                            currentGameMusicIndex = i;
                            break;
                        }
                    }
                }

                // Als er nog geen game track wordt afgespeeld, de eerste track afspelen
                if (!isPlayingGameTrack || !musicSource.isPlaying)
                {
                    currentGameMusicIndex = 0;
                    PlayMusic(GameMusicTracks[currentGameMusicIndex]);
                }
            }

            // Extra veiligheid inbouwen dat muziek onmiddellijk stopt als de game is afgelopen zodat soundeffects van GameOver of YouWin alleen te horen zijn (zonder achtergrondmuziek)
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null && (gameManager.CurrentStatus == GameStatus.GameOver || gameManager.CurrentStatus == GameStatus.EndOfGame))
            {
                StopMusic();
            }
        }
    }

    private void PlayNextGameTrack()
    {
        if (GameMusicTracks == null || GameMusicTracks.Length == 0) return;

        currentGameMusicIndex = (currentGameMusicIndex + 1) % GameMusicTracks.Length;
        PlayMusic(GameMusicTracks[currentGameMusicIndex]);
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayGameOverSoundEffect(AudioClip clip)
    {
        // Achtergrondmuziek stoppen
        StopMusic();

        // Soundeffect afspelen van Game Over of You Win
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    /*Mogelijkheid voorzien om een soundeffect met vertraging af te spelen - voorlopig niet gebruikt
    public IEnumerator PlaySoundEffectWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }*/
}
