using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [Header("Volume")]
    // Audio mixer.
    public AudioMixer mixer;

    // Audio sliders.
    public SettingSlider masterSlider;
    public SettingSlider ambienceSlider;
    public SettingSlider musicSlider;
    public SettingSlider narratorSlider;
    public SettingSlider sfxSlider;

    // Singleton.
    public static Settings I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;

        // Start off.
        gameObject.SetActive(false);

        // Sliders.
        masterSlider.Initialize();
        ambienceSlider.Initialize();
        musicSlider.Initialize();
        narratorSlider.Initialize();
        sfxSlider.Initialize();
    }

    // + Buttons

    // Return to the main menu.
    public void B_ReturnToMainMenu()
    {
        // SFX
        DJ.I.PlaySound("Snap 2");

        // Unfreeze time.
        Time.timeScale = 1f;

        // Re-load the game scene so everything is reset.
        Utility.LoadGameScene();
    }

    // Open the settings menu.
    public void B_OpenSettings()
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Freeze time.
        Time.timeScale = 0f;

        // Open the settings menu.
        gameObject.SetActive(true);
    }

    // Close the settings menu.
    public void B_CloseSettings()
    {
        // SFX
        DJ.I.PlaySound("Snap");
        
        // Unfreeze time.
        Time.timeScale = 1f;

        // Close the settings menu.
        gameObject.SetActive(false);
    }
}
