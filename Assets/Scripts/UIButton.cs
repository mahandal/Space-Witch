using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    [Header("Difficulty Settings")]
    // Use prefskey directly instead of category
    //public string category = ""; // "AsteroidSize", "AsteroidSpeed", "SpawnRate
    //public int difficultyLevel = 1; // 1, 2, or 3

    [Header("UI Button")]
    // This button's image.
    public Image image;

    // The sprite used by this button's image when the button is toggled on.
    public Sprite onSprite;

    // The sprite used by this button's image when the button is toggled off.
    public Sprite offSprite;
    
    // Whether this button is currently toggled on or off.
    public bool isToggled;
    
    // What this setting is named in player prefs, for storage.
    public string prefsKey = "ScreenShakeEnabled";

    // Whether this setting should default to being on or off.
    public bool defaultValue = true;

    // What value is assigned to this setting, if it uses an int value.
    // Most common for difficulty settings.
    public int value = 1;

    public bool isAnInt = false;

    void OnEnable()
    {
        // Load saved preference with specified default
        // isToggled = PlayerPrefs.GetInt(prefsKey, defaultValue ? 1 : 0) == 1;
        LoadValue();
        
        // Set the correct sprite based on loaded value
        //LoadSprite();
    }

    public void LoadValue()
    {
        // Handle ints
        if (isAnInt)
        {
            isToggled = PlayerPrefs.GetInt(prefsKey, 1) == value;
        }
        else
        {
            // Rest are bools
            isToggled = PlayerPrefs.GetInt(prefsKey, defaultValue ? 1 : 0) == 1;
        }
        // Load the correct sprite, based off isToggled.
        LoadSprite();
    }

    public void LoadSprite()
    {
        // Load on sprite into image if we're toggled on, load off sprite into image if we're toggled off.
        image.sprite = isToggled ? onSprite : offSprite;
    }


    // Shared toggling
    public void ToggleButton()
    {
        // Toggle bool
        isToggled = !isToggled;

        // Load on sprite into image if we're toggled on, load off sprite into image if we're toggled off.
        LoadSprite();
    }

    // - Difficulty

    public void SetDifficulty()
    {
        switch(prefsKey)
        {
            case "AsteroidSize":
                GM.I.difficulty.asteroidSize = value;
                PlayerPrefs.SetInt("AsteroidSize", value);
                break;
            case "AsteroidSpeed":
                GM.I.difficulty.asteroidSpeed = value;
                PlayerPrefs.SetInt("AsteroidSpeed", value);
                break;
            case "SpawnRate":
                GM.I.difficulty.spawnRate = value;
                PlayerPrefs.SetInt("SpawnRate", value);
                break;
        }
        PlayerPrefs.Save();

        // Reload
        //LoadValue();
        GM.I.ui.GoToSun();
    }

    // - Settings

    // Toggle whether screen shake should be enabled or disabled.
    public void ToggleScreenShake()
    {
        // Shared toggling
        ToggleButton();

        // Update GM setting
        if (GM.I != null)
            GM.I.screenShakeEnabled = isToggled;
        
        // Save the setting
        PlayerPrefs.SetInt("ScreenShakeEnabled", isToggled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleIntro()
    {
        // Shared toggling
        ToggleButton();

        // Update Main Menu setting
        if (MainMenu.I != null)
            MainMenu.I.showIntro = isToggled;

        // Save the setting
        PlayerPrefs.SetInt("ShowIntro", isToggled ? 1 : 0);
        PlayerPrefs.Save();
    }
}