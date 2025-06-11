using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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

    [Header("Intro Skip")]
    public TMP_Text skipPrompt; // UI text that says "Hold any button to skip".
    public Image skipCircle; // Radial fill image that fills up as you hold down a button to skip the intro.
    public Image skipBG; // A blank black background that fades out the universe as you skip.
    public float skipHoldTime = 1.5f; // How long to hold.
    public float promptFadeTime = 3f; // How long prompt stays visible.

    private bool skipPromptShown = false;
    private float skipTimer = 0f;
    private float promptTimer = 0f;


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

        // hide skip prompt
        HideSkipPrompt();

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
        // if (introPlaying)
        // {
        //     // Timer
        //     introTimer += Time.deltaTime;
        //     if (introTimer > introDuration)
        //         LoadGame();

        //     // Move camera
        //     mainCam.transform.position += Vector3.up * introCameraSpeed * Time.deltaTime;
        // }
        if (introPlaying)
        {
            // Check for any input
            if (Input.anyKey || Input.anyKeyDown)
            {
                // Show Skip prompt
                ShowSkipPrompt();

                // Show skip prompt on first input
                // if (!skipPromptShown)
                // {
                //     ShowSkipPrompt();
                // }
                
                // Count hold time
                if (Input.anyKey)
                {
                    skipTimer += Time.deltaTime;
                    if (skipTimer >= skipHoldTime)
                    {
                        LoadGame(); // Skip to game
                        return;
                    }
                }
            }
            else
            {
                // Reset skip timer when no input
                skipTimer = 0f;
            }
            
            // Handle prompt fade
            if (skipPromptShown)
            {
                // Timer
                promptTimer += Time.deltaTime;
                
                // Calculate text opacity based on fade progress
                float fadeProgress = promptTimer / promptFadeTime;
                Color textColor = skipPrompt.color;
                textColor.a = 1f - fadeProgress;
                skipPrompt.color = textColor;

                // Update image color
                float skipProgress = skipTimer / skipHoldTime;
                skipCircle.color = new Color(skipProgress, skipProgress, skipProgress, skipProgress);
                skipBG.color = new Color(0f, 0f, 0f, skipProgress);

                // Update fill amount
                skipCircle.fillAmount = skipProgress;
                
                // Hide?
                if (promptTimer >= promptFadeTime)
                {
                    HideSkipPrompt();
                }
            }
            
            // Continue with your existing intro timer logic
            introTimer += Time.deltaTime;
            if (introTimer > introDuration)
                LoadGame();
                
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

    void ShowSkipPrompt()
    {
        // Bool
        skipPromptShown = true;

        // Timer
        promptTimer = 0f;

        // Text color
        Color textColor = skipPrompt.color;
        textColor.a = 1f;
        skipPrompt.color = textColor;

        // Image color
        // skipCircle.color = new Color(0f, 0f, 0f, 0f);
        // skipBG.color = new Color(0f, 0f, 0f, 0f);

        // // Fill
        // skipCircle.fillAmount = 0f;

        // // Activate
        // skipPrompt.gameObject.SetActive(true);
        // skipCircle.gameObject.SetActive(true);
    }

    void HideSkipPrompt()
    {
        skipPromptShown = false;
        // skipPrompt.gameObject.SetActive(false);
        skipPrompt.color = new Color(1f, 1f, 1f, 0f);
    }
}
