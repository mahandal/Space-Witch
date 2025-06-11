using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Manual Machinery")]
    // Our DJ!
    public AudioManager dj;

    // The main camera
    public Camera mainCam;

    // The settings menu.
    public GameObject settingsMenu;

    // Parent of our main UI, to be disabled upon starting the Intro.
    public GameObject mainUI;

    // Overlay for loading?
    public Image overlay;

    // Intro sequence parent game object
    public GameObject intro;
    public float introCameraSpeed = 21f;
    public float introDuration = 512f;

    private float introTimer = 0f;


    [Header("Settings")]
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider gatherVolume;
    public Slider ambienceVolume;


    [Header("Automated Machinery")]
    // Singleton
    public static MainMenu I;

    // - States

    // First time?
    // True when you first open the game.
    // False when returning to the main menu from Game.
    public static bool firstTime = true;

    // Should the intro play?
    // True by default when first opening the game.
    // False when returning to the main menu from Game or if disabled in settings.
    public bool showIntro = true;

    // Is the intro playing?
    public bool introPlaying = false;

    // Awaken!
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
        settingsMenu.SetActive(false);
        overlay.gameObject.SetActive(false);
        intro.SetActive(false);

        // Enable stuff that should be!
        mainUI.SetActive(true);
        Time.timeScale = 1f;

        // Load settings(?)
        showIntro = PlayerPrefs.GetInt("ShowIntro", 1) == 1;
    }

    void Start()
    {
        // First time?
        if (firstTime)
        {
            // Play the very first thing a new player will hear!
            dj.PlayMusic("Nycticorax");
            firstTime = false;
        } else {
            dj.PlayLoopyMusic("Night Heron");
        }
    }

    void FixedUpdate()
    {
        if (introPlaying)
        {
            // Timer
            introTimer += Time.deltaTime;
            if (introTimer > introDuration)
                LoadGame();

            // Move camera
            mainCam.transform.position += Vector3.up * introCameraSpeed * Time.deltaTime;
        }
    }

    // Press play!
    public void PlayButtonPressed()
    {
        // Hide the main UI.
        mainUI.SetActive(false);

        // Check if we should play the intro
        if (showIntro)
            PlayIntro();
        else
            LoadGame();
    }

    public void PlayIntro()
    {
        // Set bool.
        introPlaying = true;

        // Activate the intro parent object.
        intro.SetActive(true);

        // Play the intro audio.
        dj.PlayMusic("Intro");
    }

    // Begin loading the Game scene.
    public void LoadGame()
    {
        // Activate our loading overlay.
        overlay.gameObject.SetActive(true);

        // Load game!
        SceneManager.LoadScene("Game");
    }

    // Time to go!
    public void ExitButtonPressed()
    {
        Application.Quit();
    }

    // Let's get you set up.
    public void SettingsButtonPressed()
    {
        // Activate settings menu
        settingsMenu.SetActive(true);
    }

    // Back again are we?
    public void MainMenuButtonPressed()
    {
        // Deactivate settings menu
        settingsMenu.SetActive(false);
    }
}
