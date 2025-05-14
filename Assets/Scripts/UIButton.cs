using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
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

    void Start()
    {
        // Load saved preference with specified default
        isToggled = PlayerPrefs.GetInt(prefsKey, defaultValue ? 1 : 0) == 1;
        
        // Set the correct sprite based on loaded value
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

    // Toggle whether screen shake should be enabled or disabled.
    public void ToggleScreenShake()
    {
        // Shared toggling
        ToggleButton();

        Debug.Log("Toggling screen shake to " + isToggled);

        // Update GM setting
        if (GM.I != null)
            GM.I.screenShakeEnabled = isToggled;
        
        // Save the setting
        PlayerPrefs.SetInt("ScreenShakeEnabled", isToggled ? 1 : 0);
        PlayerPrefs.Save();
    }

}