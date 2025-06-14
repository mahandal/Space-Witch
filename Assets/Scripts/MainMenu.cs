using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Manual Machinery")]
    // Our DJ!
    //public AudioManager dj;

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

    public float introTimer = 0f;


    // [Header("Settings")]
    // public Slider masterVolume;
    // public Slider musicVolume;
    // public Slider sfxVolume;
    // public Slider gatherVolume;
    // public Slider ambienceVolume;

    [Header("Intro Skip")]
    public TMP_Text skipPrompt; // UI text that says "Hold any button to skip".
    public Image skipCircle; // Radial fill image that fills up as you hold down a button to skip the intro.
    public Image skipBG; // A blank black background that fades out the universe as you skip.
    public float skipHoldTime = 1.5f; // How long to hold.
    public float promptFadeTime = 3f; // How long prompt stays visible.

    private bool skipPromptShown = false;
    private float skipTimer = 0f;
    private float promptTimer = 0f;

    [Header("Intro Black Hole")]
    public Transform blackHole;
    public Image bhBG;
    public float blackHoleStartTime = 13f; // Last 13 seconds
    private bool inBlackHoleEffect = false;
    private float orbitRadius;
    private float orbitAngle = 0f;


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

        // // Disable stuff that shouldn't be
        // settingsMenu.SetActive(false);
        // overlay.gameObject.SetActive(false);
        // intro.SetActive(false);

        // // hide skip prompt
        // HideSkipPrompt();

        // // Enable stuff that should be!
        // mainUI.SetActive(true);
        // Time.timeScale = 1f;

        // // Load settings(?)
        // showIntro = PlayerPrefs.GetInt("ShowIntro", 1) == 1;
    }

    void OnEnable()
    {
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
            GM.I.dj.PlayMusic("Nycticorax");
            firstTime = false;
        } else {
            GM.I.dj.PlayLoopyMusic("Night Heron");
        }
    }

    void FixedUpdate()
    {
        if (introPlaying)
        {
            // Check for any input
            if (Input.anyKey || Input.anyKeyDown)
            {
                // Show Skip prompt
                ShowSkipPrompt();
                
                // Count hold time
                if (Input.anyKey)
                {
                    skipTimer += Time.deltaTime;
                    if (skipTimer >= skipHoldTime)
                    {
                        GoHome(); // Skip to game
                        return;
                    }
                }
            }
            else
            {
                // Reset skip timer when no input
                skipTimer = 0f;
            }
            
            // Handle prompt progress
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
                GoHome();
            

            // Move camera up.
            //mainCam.transform.position += Vector3.up * introCameraSpeed * Time.deltaTime;
            HandleIntroBlackHole();
        }
    }

    // Handle the intro black hole effect.
    // Note: Also handles the camera moving up until then. Is that the black hole too????
    public void HandleIntroBlackHole()
    {
        // Check if we should start black hole effect
        float timeRemaining = introDuration - introTimer;
        if (timeRemaining <= blackHoleStartTime && !inBlackHoleEffect)
        {
            inBlackHoleEffect = true;
            // Calculate initial orbit radius (distance from camera to black hole)
            orbitRadius = Vector3.Distance(mainCam.transform.position, blackHole.position);
        }

        if (inBlackHoleEffect)
        {
            // Orbit
            OrbitBlackHole(timeRemaining);

            // Fade in background
            float percentDone = 1 - (timeRemaining / blackHoleStartTime);
            bhBG.color = new Color (0f, 0f, 0f, percentDone);
        }
        else
        {
            // Normal upward movement
            mainCam.transform.position += Vector3.up * introCameraSpeed * Time.deltaTime;
        }
    }

    void OrbitBlackHole(float timeRemaining)
    {
        // Gradually transition from upward movement to orbital
        float effectProgress = 1f - (timeRemaining / blackHoleStartTime); // 0 to 1
        
        // Reduce upward movement and increase orbital speed over time
        float upwardSpeed = introCameraSpeed * (1f - effectProgress);
        float orbitSpeed = effectProgress * 90f; // Degrees per second at full effect
        
        // Continue some upward movement early on
        mainCam.transform.position += Vector3.up * upwardSpeed * Time.deltaTime;
        
        // Shrink orbit radius (zoom in)
        float currentRadius = orbitRadius * (1f - effectProgress * 0.8f); // Zoom to 20% of original
        
        // Add orbital movement
        orbitAngle += orbitSpeed * Time.deltaTime;
        Vector3 orbitOffset = new Vector3(
            Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * currentRadius * effectProgress,
            Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * currentRadius * effectProgress,
            0
        );
        
        Vector3 targetPos = blackHole.position + orbitOffset;
        targetPos.z = mainCam.transform.position.z; // Keep camera Z
        
        // Blend between current movement and orbital position
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPos, effectProgress * Time.deltaTime * 2f);
        
        // Zoom camera
        float startSize = 5f; // Adjust to your normal camera size
        float endSize = 0.1f; // How zoomed in you want
        mainCam.orthographicSize = Mathf.Lerp(startSize, endSize, effectProgress);
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
            GoHome();
    }

    public void PlayIntro()
    {
        // Set bool.
        introPlaying = true;

        // Activate the intro parent object.
        intro.SetActive(true);

        // Play the intro audio.
        GM.I.dj.PlayMusic("Intro");
    }

    // Set us up to go home.
    // Note: I don't like how this overlaps with GM.GoHome but I'm not quite sure which direction to go. A problem for another time.
    // TBD: Refactor
    public void GoHome()
    {
        // Activate our loading overlay.
        //overlay.gameObject.SetActive(true);

        // Load game!
        //SceneManager.LoadScene("Game");

        // Stop the music!
        GM.I.dj.StopMusic();

        // Disable the main menu
        gameObject.SetActive(false);

        // Unfreeze universe
        GM.I.universe.gameObject.SetActive(true);

        // Activate the main camera
        GM.I.mainCam.gameObject.SetActive(true);

        // Go home!
        GM.I.GoHome();
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
