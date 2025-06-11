using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            // Move camera
            mainCam.transform.position += Vector3.up * introCameraSpeed * Time.deltaTime;
        }
    }

    // Press play!
    public void PlayButtonPressed()
    {
        // Overlay!
        //overlay.gameObject.SetActive(true);

        // No mas!
        mainUI.SetActive(false);

        // Intro!
        intro.SetActive(true);
        introPlaying = true;

        // Music!
        dj.PlayMusic("Intro");

        // Load game!
        //SceneManager.LoadScene("Game");
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
