using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class GM : MonoBehaviour
{
    [Header("Manual Machinery")]
    // The main menu.
    public MainMenu mainMenu;

    // The main camera.
    // Note: Not for the main menu! 
    // (the intro has its own camera, that plays by its own rules)
    public Camera mainCam;

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
    // Key:
    // -4 : init
    // -3 : main menu
    // -2 : intro
    // -1 : home
    //  0 : pre-game
    //  1 : game
    //  2 : post-game
    public int gameState = -4;

    // Saved data for persistence
    // (credits, unlocked talents, etc...)
    public SaveData saveData = new SaveData();

    // Current nebula
    //public string nebula = "unknown";
    public Nebula nebula;

    // Talents
    public Dictionary<string, Talent> talents = new Dictionary<string, Talent>();

    // Planets
    public List<Planet> planets;
    public List<Planet> activePlanets;

    // Worm holes
    public List<WormHole> wormHoles = new List<WormHole>();

    // Bees
    public List<Bee> bees;

    // Beacons
    public List<Beacon> beacons = new List<Beacon>();


    // - Songs

    // The current bpm
    public float bpm;

    // A list of all the songs available to be played.
    public List<Song> songs = new List<Song>();

    // The index of the song currently being played.
    public int songIndex = 0;

    // The index of the deepest song unlocked so far.
    public int unlockedSongIndex = 0;

    // - Time
    public float timeElapsed = 0f;
    public float totalTime = 0f;


    // States

    // Hype
    public int hype = 0;
    public int passiveHype = 0;

    // Polarity
    public int polarity = 1;

    // Is the game currently paused?
    public bool isPaused = false;

    // Should we load Home on startup?
    // (if not, we load the Main Menu)
    public static bool startAtHome = false;

    // The current witch we are learning from, while training.
    public Witch currentWitch;


    // Timers
    public float gameTimer = 180f;
    public float songTimer = 180f;
    public float beatTimer = 0.5f;
    public float hypeTime = 60f;
    public float hypeTimer = 60f;

    // --- Settings

    // Screen shake
    public bool screenShakeEnabled = true;

    void Awake()
    {
        // Singleton
        if (I != null)
        {
            Destroy(this);
        }
        else
        {
            I = this;
        }

        // Disable stuff that shouldn't be
        ui.pauseMenu.SetActive(false);
        //ui.levelUpScreen.SetActive(false);
        ui.postGame.SetActive(false);

        // Enable stuff that should be
        //ui.levelUpScreen.SetActive(true);

        // Load our saved data
        LoadGame();

        // Load settings
        //LoadSettings();
    }

    void Start()
    {
        // Initialize talents.
        talentManager.InitializeTalents();

        // Agatha starts with Jump.
        // TBD: Scale to include other Witches and more starting talents.
        player.LearnTalent("Jump");

        // Preload images so talents load smoothly.
        Utility.PreloadImages();

        //GoHome();

        // Start in the main menu.
        // if (gameState == -4)
        //     GoToMainMenu();

        // Start at home?
        if (GM.startAtHome)
            mainMenu.GoHome();
        else
            GoToMainMenu();
    }

    public void StopTime()
    {
        // Time scale
        Time.timeScale = 0f;

        // Music
        dj.musicSource.Pause();
    }

    public void StartTime()
    {
        // Time scale
        Time.timeScale = 1f;

        // Music
        //dj.musicSource.Play();
        dj.musicSource.UnPause();
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "gamedata.json");

    // Save our progress.
    public void SaveGame()
    {
        // Update save data with current values
        saveData.credits = Gatherer.credits;

        // Save planet nectar
        saveData.planetNectar.Clear();
        foreach (Planet planet in home.planets)
        {
            saveData.planetNectar.Add(planet.nectar);
        }

        // Convert to JSON and write to file
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    // Load our progress.
    public void LoadGame()
    {
        // Settings are handled in player prefs
        LoadSettings();

        // Load saved data from json
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                saveData = JsonUtility.FromJson<SaveData>(json);

                // Clear ooold saves
                // (literally just for Evan and Nate I think?)
                if (saveData.unlockedTalents.Count < 5)
                    saveData = new SaveData();
            }
            else
            {
                // No save found, make a new one.
                saveData = new SaveData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
            saveData = new SaveData();
        }

        // - Apply loaded data to game

        // Credits
        Gatherer.credits = saveData.credits;

        // Nectar
        for (int i = 0; i < saveData.planetNectar.Count; i++)
        {
            home.planets[i].nectar = saveData.planetNectar[i];
        }
    }

    // Load settings stored in player prefs.
    // Note: Audio settings are handled in AudioManager.
    // Note: UIButton handles visuals and actual button presses.
    public void LoadSettings()
    {
        // Screen shake
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
        // - Beat timer

        // Decrement
        beatTimer -= Time.deltaTime;

        // Next beat?
        if (beatTimer < 0)
        {
            // Beat
            Beat();

            // Reset
            beatTimer = 60f / bpm;
        }


        // Wait for game to start
        if (gameState < 1)
            return;


        // - Time elapsed
        timeElapsed += Time.deltaTime;

        // - Hype

        // Decrement hype timer
        hypeTimer -= Time.deltaTime;

        // Hype grows...
        if (hypeTimer < 0f)
        {
            // Increment hype
            passiveHype++;

            // Reset
            hypeTimer = hypeTime;
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
    }

    // New beat!
    void Beat()
    {
        // BeePM!
        BeePM();

        // DJ beat
        dj.Beat();

        // UI Beat
        ui.Beat();

        // Beacon beat!
        Beacon.Beat();

        // SpawnManager?
        spawnManager.UpdateAsteroidPath();

        // Astarax
        Astarax();
    }

    void BeePM()
    {
        if (gameState < 1) return;

        // Go through each bee
        foreach (Bee bee in bees)
        {
            if (!bee.isDying && bee.gameObject.activeSelf)
                bee.SpawnStar();

            bee.isBumpable = true;
        }
    }

    // Astarax the Asteroid Demon.
    // Responsible for all the nefarious deeds done throughout the universe.
    // Known by many names, including The Devil.
    void Astarax()
    {
        // wait for it to get intense
        if (gameState < 1 || dj.songHype <= 0)
            return;

        // Go through each worm hole
        foreach (WormHole wormHole in wormHoles)
        {
            // Spawn an asteroid
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

        // Collect nectar
        //CollectNectar();

        // Award credits
        int score = CalculateScore();
        Gatherer.credits += score / 100;
        SaveGame();

        // sfx
        dj.PlayEffect("victory", player.transform.position, 1f, true);

        // Check for high score
        scoreboard.CheckForHighScore(score);
    }

    // Calculate the player's live score each beat.
    // (used by UI)
    // V similar to the total score, but keeps some things secret.
    public int CalculatePreScore()
    {
        // Initialize score.
        float score = Gatherer.starsGathered;

        // Multiply by # of lives remaining
        // score *= GM.I.player.livesRemaining;

        // // Multiply by # of planets remaining
        // score *= GM.I.activePlanets.Count;

        // // Gain bonus score per moon.
        // score *= 1f + (0.5f * Gatherer.moonsGathered);

        return (int)score;
    }

    // Calculate the player's total score at the end of a run.
    public int CalculateScore()
    {
        // Initialize score.
        float score = Gatherer.starsGathered;

        // Gain bonus for # of lives remaining
        score *= 1f + (0.1f * GM.I.player.livesRemaining);

        // Gain bonus for # of planets remaining
        score *= 1f + (0.1f * GM.I.activePlanets.Count);

        // Gain bonus score per moon.
        score *= 1f + (0.1f * Gatherer.moonsGathered);

        return (int)score;
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
        int score = CalculateScore();
        scoreboard.RankThirteen(score);
    }

    void GameOver()
    {
        // Set gamestate
        gameState = 2;

        // Stop time
        StopTime();

        // Display high scores
        scoreboard.DisplayHighScores();

        // UI
        ui.PostGame();

        // Music
        dj.StopMusic();

        // Collect nectar?
        CollectNectar();
    }

    // Go to the main menu.
    public void MainMenuPressed()
    {
        // Set bool.
        GM.startAtHome = false;

        // Re-load game.
        SceneManager.LoadScene("Game");
    }

    // Go home (from post game screen).
    public void GoHomePressed()
    {
        // Set bool.
        GM.startAtHome = true;

        // Re-load game.
        SceneManager.LoadScene("Game");
        //GoHome();
    }

    // Go home.
    // Called on start and after a run if we so choose.
    public void GoHome()
    {
        // Go home
        spawnManager.DeactivateNebulas();
        home.gameObject.SetActive(true);
        nebula = home;
        gameState = -1;

        // UI go home
        ui.GoHome();

        // DJ go home
        dj.GoHome();

        // Deactivate all bees
        foreach (Bee bee in bees)
        {
            bee.gameObject.SetActive(false);
        }

        // Reset player
        player.xp = 0f;
        player.level = 1;

        // Give each planet some nectar
        GiveEachPlanetNectar();

        // Turn time back on
        StartTime();
    }

    // Set up the main menu?
    // Note: I got a bit confused while writing this. Still not sure exactly how much to re-set up versus reload or w/e.
    public void GoToMainMenu()
    {
        // Stop the music(?)
        //GM.I.dj.StopMusic();

        // Reload scene?
        //SceneManager.LoadScene("Game");

        // Set gamestate
        gameState = -3;

        // Hide the universe
        universe.gameObject.SetActive(false);

        // Hide our UI
        ui.GoToMainMenu();

        // Toggle camera
        mainCam.gameObject.SetActive(false);
        mainMenu.mainCam.gameObject.SetActive(true);

        // Show the main menu
        mainMenu.gameObject.SetActive(true);
    }

    // Give each home planet some nectar.
    public void GiveEachPlanetNectar(int amount = 1)
    {
        foreach (Planet planet in home.planets)
        {
            planet.nectar++;
        }
    }

    // Sets us up for a run.
    // Triggered by entering a black hole.
    public void GoBig(string nebulaName)
    {
        // Leave home
        home.gameObject.SetActive(false);
        player.transform.position = Vector3.zero;
        familiar.transform.position = new Vector3(0, 1, 0);

        // Set up outside UI
        ui.GoOut();

        // Set up outside audio
        dj.GoOut();

        // // Activate all bees
        // foreach (Bee bee in bees)
        // {
        //     bee.gameObject.SetActive(true);
        // }

        // Initialize songs
        Song.InitializeSongs();

        // Set up nebula
        spawnManager.SetUpNebula(nebulaName);

        //
        /////
        //


        // Stop time?
        // Or handled in player level up?
        //StopTime();

        // // Set up bees
        // spawnManager.SetUpBees();

        // Reset star count
        Gatherer.starsGathered = 0;

        // Level 1
        // (game starts from there)
        player.level = 0;
        player.BeginLevelUp();
        player.xp = 0;
        //BeginRun();
    }

    // Enter the planning phase of a run.
    public void PreGame()
    {
        // Set game state.
        gameState = 0;

        // Pregame song
        bpm = 120;
        beatTimer = 60f / bpm;
        dj.PlayLoopyMusic("Planning");

        // Time to start.
        StartTime();
    }

    // Start a run.
    public void BeginRun()
    {
        Debug.Log("Beginning a new run!");
        // Hide pre-game
        //ui.preGame.SetActive(false);

        // Close pre-game
        //ui.BeginGame();

        // Set up game UI
        ui.BeginGame();

        // Set up bees
        spawnManager.SetUpBees();

        // Activate the beacons
        foreach (Beacon beacon in beacons)
        {
            // Freeze in place, just in case (and reduce their physics load?)
            beacon.rb2d.bodyType = RigidbodyType2D.Static;

            // Make trigger so they don't physically block objects anymore.
            beacon.GetComponent<Collider2D>().isTrigger = true;

            // Materialize visual
            SpriteRenderer renderer = beacon.GetComponent<SpriteRenderer>();
            renderer.color = new Color(1f, 1f, 1f, 0.5f); // Half opacity
        }

        // Set game state
        gameState = 1;

        // Start playing first song!
        dj.StartSong(Random.Range(0, 7));
        //dj.StartSong(0);
        unlockedSongIndex = songIndex;
        gameTimer = songs[songIndex].duration;
        totalTime = songs[songIndex].duration;

        // Hit marker
        HitMarker.CreateSongMarker(player.transform.position, songs[songIndex].myName + " (1/7)");

        // Spawn Moon
        Gatherer.moonsGathered = 1;
        spawnManager.SpawnMoon();

        // Stop meditating
        // Note: Time is started here!
        player.EndMeditation();

        // Hrm?
        //IntroHaiku();

        // Level 1!
        //player.LevelUp();
    }

    public void IntroHaiku()
    {
        Vector3 v1 = new Vector3(player.transform.position.x, player.transform.position.y + 5.5f, 0f);
        Vector3 v2 = new Vector3(player.transform.position.x, player.transform.position.y + 5f, 0f);
        Vector3 v3 = new Vector3(player.transform.position.x, player.transform.position.y + 4.5f, 0f);
        HitMarker.CreateNarrativeMarker(v1, "Protect the planets!");
        HitMarker.CreateNarrativeMarker(v2, "Make haste, the hour is late.");
        HitMarker.CreateNarrativeMarker(v3, "Good folk need your help.");
    }

    // Toggles the pause menu.
    // Also handles closing screens that are open.
    public void TogglePause()
    {
        // If we're training, stop.
        if (ui.persistentGrowth.activeSelf)
        {
            ui.CloseTrainingScreen();
            return;
        }

        // If we're leveling up, stop.
        if (ui.growth.activeSelf)
        {
            ui.CloseLevelUpScreen();
            return;
        }

        // If we're investing, stop.
        if (ui.planetSurface.gameObject.activeSelf)
        {
            ui.LeavePlanet();
            return;
        }

        // Already paused, so we should unpause.
        if (isPaused)
        {
            Unpause();
        }
        else
        {
            // Game is unpaused, so pause.
            Pause();
        }
    }

    public void Unpause()
    {
        // Bool
        isPaused = false;

        // Unfreeze time
        StartTime();

        // UI
        ui.Unpause();
    }

    public void Pause()
    {
        // Bool
        isPaused = true;

        // Freeze time
        StopTime();

        // UI
        ui.Pause();
    }

    public void ShakeCamera(float strength, float duration, Vector3 position)
    {
        // Early return if shake is disabled
        if (!screenShakeEnabled)
            return;

        // Apply a maximum cap to the strength
        float cappedStrength = Mathf.Min(strength, 0.5f);

        // Optional: Apply a curve to make scaling more pleasing
        // This will make smaller damage feel responsive while keeping larger damage manageable
        float scaledStrength = 0.1f + (0.4f * (1 - Mathf.Exp(-cappedStrength * 3f)));

        StartCoroutine(ShakeCameraCoroutine(scaledStrength, duration, position));
    }

    private System.Collections.IEnumerator ShakeCameraCoroutine(float strength, float duration, Vector3 position)
    {
        Vector3 originalPos = position;
        originalPos.z = mainCam.transform.localPosition.z;
        player.isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * strength;
            float yOffset = Random.Range(-1f, 1f) * strength;


            mainCam.transform.localPosition = new Vector3(
                originalPos.x + xOffset,
                originalPos.y + yOffset,
                originalPos.z);


            elapsed += Time.deltaTime;
            yield return null;
        }

        player.isShaking = false;
    }

    // Collect all the nectar our bees have gathered, at the end of a victory.
    public void CollectNectar()
    {
        // Go through each bee
        for (int i = 0; i < bees.Count; i++)
        {
            // Get bee
            Bee bee = bees[i];

            // Check if it's alive
            if (!bee.gameObject.activeSelf)
                continue;

            if (i < 9) // Bees 1-9 to their planet
            {
                home.planets[i].nectar += bee.nectar;
            }
            else if (i == 9) // Bee 10 - Friendship
            {
                int nectarPerPlanet = bee.nectar / home.planets.Count;
                foreach (Planet planet in home.planets)
                    planet.nectar += nectarPerPlanet;
            }
            else if (i == 10) // Bee 11 - Curiosity  
            {
                int randomIndex = Random.Range(0, home.planets.Count);
                home.planets[randomIndex].nectar += bee.nectar;
            }
            else if (i == 11) // Bee 12 - Patience
            {
                // TBD: Give to each planet in sequence
                /* int planetIndex = bee.goalIndex % home.planets.Count;
                home.planets[planetIndex].nectar += bee.nectar; */
                int randomIndex = Random.Range(0, home.planets.Count);
                home.planets[randomIndex].nectar += bee.nectar;
            }
            
            // Reset bee's pollen
            bee.nectar = 0;
        }
    }
}
