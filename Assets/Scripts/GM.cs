using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GM : MonoBehaviour
{
    [Header("Manual Machinery")]
    // Our hero!
    public Player player;
    
    // Our trusty sidekick!
    public Familiar familiar;

    // The universe, containing everything we know: our home, nebulas, entities, etc...
    public Transform universe;

    // Our home
    public Nebula home;
    
    // UI
    public UI ui;

    // Scoreboard
    public Scoreboard scoreboard;

    // Managers

    // Audio manager
    public AudioManager dj;

    // Talent manager
    public TalentManager talentManager;

    // Spawn manager
    public SpawnManager spawnManager;
    

    [Header("Automated Machinery")]
    // Singleton
    public static GM I;

    // Game state
    public int gameState = 0;

    // Current nebula
    //public string nebula = "unknown";
    public Nebula nebula;

    // Talents
    public Dictionary<string, Talent> talents = new Dictionary<string, Talent>();

    // Planets
    public List<Planet> planets;

    // Worm holes
    public List<WormHole> wormHoles = new List<WormHole>();

    // Bees
    public List<Bee> bees;


    // - Songs
    
    // The current bpm
    public float bpm;

    // A list of all the songs available to be played.
    public List<Song> songs = new List<Song>();
    
    // The index of the song currently being played.
    public int songIndex = 0;

    // The index of the deepest song unlocked so far.
    public int unlockedSongIndex = 0;


    // States

    // Intensity
    public int intensity = 0;

    // Polarity
    public int polarity = 1;

    public bool isPaused = false;


    // Timers
    public float gameTimer = 180f;
    public float songTimer = 180f;
    public float beatTimer = 0.5f;
    public float intensityTime = 30f;
    public float intensityTimer = 0f;

    // --- Settings

    // Screen shake
    public bool screenShakeEnabled = true;

    void Awake()
    {
        // Singleton
        if (I != null)
        {
            Destroy(this);
        } else {
            I = this;
        }

        // Disable stuff that shouldn't be
        ui.pauseMenu.SetActive(false);
        //ui.levelUpScreen.SetActive(false);
        ui.postGame.SetActive(false);

        // Enable stuff that should be
        //ui.levelUpScreen.SetActive(true);

        // Load settings
        LoadSettings();
    }

    void Start()
    {
        // Initialize talents.
        talentManager.InitializeTalents();

        // Agatha starts with Jump.
        // TBD: Scale to other Witches
        player.LearnTalent("Jump");

        // Preload images so talents load smoothly.
        Utility.PreloadImages();

        GoHome();
        
        /* // Initialize songs
        Song.InitializeSongs();

        // Set up nebula
        spawnManager.SetUpNebula();

        // Spawn Moon
        Gatherer.moonsGathered = 1;
        spawnManager.SpawnMoon();

        // Pregame
        PreGame(); */
    }

    // Load settings stored in player prefs.
    // Note: Audio settings are handled in AudioManager.
    // Note: UIButton handles visuals and actual button presses.
    public void LoadSettings()
    {
        screenShakeEnabled = PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for game to start
        /* if (gameState < 1)
            return; */
        
        // Update timers
        Timers();
        player.Timers();
    }

    // Expected to be called once per update
    void Timers()
    {
        // Wait for game to start
        if (gameState < 1)
            return;
        
        // - Intensity

        // Decrement intensity timer
        intensityTimer -= Time.deltaTime;

        // Intensity grows...
        if (intensityTimer < 0f)
        {
            // Increment intensity
            intensity++;

            // Reset
            intensityTimer = intensityTime;
        }

        // - Game timer
        // (aka the clock)

        // Decrement game timer
        gameTimer -= Time.deltaTime;

        // Game is over, we did it!
        if (gameTimer < 0 && gameState == 1)
        {
            Win();
            return;
        }

        // - Song timer

        // Decrement song timer
        songTimer -= Time.deltaTime;

        // Song is over, on to the next one!
        if (songTimer < 0)
            dj.PlayNextSong();


        // - Beat timer

        // Decrement
        beatTimer -= Time.deltaTime;

        // Next beat?
        if (beatTimer < 0)
        {
            // Beepm
            BeePM();

            // Reset
            beatTimer = 60f / bpm;
        }
    }

    public void BeePM()
    {
        // Pause beepm while leveling up
        if (!universe.gameObject.activeSelf)
            return;

        // Go through each bee
        foreach(Bee bee in bees)
        {
            if (!bee.isDying)
                bee.SpawnStar();

            bee.isBumpable = true;
        }

        // Go through each worm hole
        foreach(WormHole wormHole in wormHoles)
        {
            // Spawn an asteroid(?)
            wormHole.SpawnAsteroid();
        }
    }

    // Survive your flight to win!
    public void Win()
    {
        // Shared game over
        GameOver();

        // Activate background
        ui.winBackground.SetActive(true);
        ui.lossBackground.SetActive(false);

        // sfx
        dj.PlayEffect("victory", player.transform.position, 1f, true);

        // Check for high score
        int score = (int)(Gatherer.starsGathered);
        scoreboard.CheckForHighScore(score);
    }

    // Dying results in loss...
    public void Lose()
    {
        // Shared game over
        GameOver();

        // Activate background
        ui.lossBackground.SetActive(true);
        ui.winBackground.SetActive(true);

        // sfx
        dj.PlayEffect("defeat", player.transform.position, 1f, true);

        // Rank 13
        scoreboard.RankThirteen();
    }

    public void GameOver()
    {
        // Set gamestate
        gameState = 2;

        // Stop time
        Time.timeScale = 0f;

        /* // Check for high score
        int score = (int)(Gatherer.starsGathered);
        scoreboard.CheckForHighScore(score); */

        // Display high scores
        scoreboard.DisplayHighScores();

        // UI
        ui.PostGame();

        // Music
        dj.StopMusic();
    }

    // Go big!
    /* public void GoBigPressed()
    {
        //SceneManager.LoadScene("Game");

        // Les go!
        BeginGame();
    } */

    // Or go home...
    /* public void GoHomePressed()
    {
        SceneManager.LoadScene("Menu");
    } */

    // Go to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Go home (from post game screen)
    public void GoHomePressed()
    {
        SceneManager.LoadScene("Game");
        GoHome();
    }

    // Go home.
    // Called on start and after a run if we so choose.
    public void GoHome()
    {
        // Go home
        spawnManager.DeactivateNebulas();
        home.gameObject.SetActive(true);
        nebula = home;
        //nebula = "Home";
        gameState = -1;
        
        // Deactivate all bees
        foreach(Bee bee in bees)
        {
            bee.gameObject.SetActive(false);
        }

        // Reset player
        player.xp = 0f;
        player.level = 1;

        // Turn time back on
        Time.timeScale = 1f;
    }

    // Sets us up for a run.
    // Triggered by entering a black hole.
    public void GoBig(string nebulaName)
    {
        // Leave home
        home.gameObject.SetActive(false);
        player.transform.position = Vector3.zero;
        familiar.transform.position = new Vector3(0, 1, 0);

        // Activate all bees
        foreach(Bee bee in bees)
        {
            bee.gameObject.SetActive(true);
        }

        // Initialize songs
        Song.InitializeSongs();

        // Set up nebula
        spawnManager.SetUpNebula(nebulaName);

        //
        /////
        //


        // Stop time
        Time.timeScale = 0f;

        // Set up bees
        spawnManager.SetUpBees();

        // Reset star count
        Gatherer.starsGathered = 0;

        // Level 1
        // (game starts from there)
        player.level = 0;
        player.LevelUp();
        player.xp = 0;
        //BeginRun();
    }

    // Start a run.
    public void BeginRun()
    {
        // Hide pre-game
        //ui.preGame.SetActive(false);

        // Close pre-game
        //ui.BeginGame();

        // Activate game start time
        Time.timeScale = 1f;
        gameState = 1;

        // Start playing first song!
        dj.StartSong(Random.Range(0, 7));
        unlockedSongIndex = songIndex;
        gameTimer = songs[songIndex].duration;

        // Hit marker
        HitMarker.CreateSongMarker(player.transform.position, songs[songIndex].myName + " (1/7)");

        // Hrm?
        IntroHaiku();

        // Level 1!
        //player.LevelUp();
    }

    public void IntroHaiku()
    {
        Vector3 v1 = new Vector3(player.transform.position.x, player.transform.position.y + 5.5f, 0f);
        Vector3 v2 = new Vector3(player.transform.position.x, player.transform.position.y + 5f, 0f);
        Vector3 v3 = new Vector3(player.transform.position.x, player.transform.position.y + 4.5f, 0f);
        HitMarker.CreateNarrativeMarker(v1, "Go gather the stars");
        HitMarker.CreateNarrativeMarker(v2, "Make haste, the hour is late");
        HitMarker.CreateNarrativeMarker(v3, "It all counts on this");
    }

    // Toggles the pause menu
    public void TogglePause()
    {
        // Already paused, so we should unpause.
        if (isPaused)
        {
            Unpause();
        } else {
            // Game is unpaused, so pause.
            Pause();
        }
    }

    public void Unpause()
    {
        // Bool
        isPaused = false;

        // Unfreeze time
        Time.timeScale = 1f;
        dj.musicSource.Play();
        //universe.SetActive(true);

        // UI
        ui.Unpause();
    }

    public void Pause()
    {
        // Bool
        isPaused = true;

        // Freeze time
        Time.timeScale = 0f;
        dj.musicSource.Pause();
        //universe.SetActive(false);

        // UI
        ui.Pause();
    }

    public void ShakeCamera(float intensity, float duration, Vector3 position)
    {
        // Early return if shake is disabled
        if (!screenShakeEnabled)
            return;
        
        // Apply a maximum cap to the intensity
        float cappedIntensity = Mathf.Min(intensity, 0.5f);
        
        // Optional: Apply a curve to make scaling more pleasing
        // This will make smaller damage feel responsive while keeping larger damage manageable
        float scaledIntensity = 0.1f + (0.4f * (1 - Mathf.Exp(-cappedIntensity * 3f)));

        StartCoroutine(ShakeCameraCoroutine(scaledIntensity, duration, position));
    }

    private System.Collections.IEnumerator ShakeCameraCoroutine(float strength, float duration, Vector3 position)
    {
        /*
        Vector3 originalPos = player.mainCam.transform.localPosition;
        originalPos.x = position.x;
        originalPos.y = position.y;
        */
        Vector3 originalPos = position;
        originalPos.z = player.mainCam.transform.localPosition.z;
        player.isShaking = true;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * strength;
            float yOffset = Random.Range(-1f, 1f) * strength;

            
            player.mainCam.transform.localPosition = new Vector3(
                originalPos.x + xOffset, 
                originalPos.y + yOffset, 
                originalPos.z);
            
                
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        player.isShaking = false;
    }
}
