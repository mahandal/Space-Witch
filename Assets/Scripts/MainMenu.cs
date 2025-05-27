using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu")]
    // The settings menu.
    public GameObject settingsMenu;

    [Header("Settings")]
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider gatherVolume;
    public Slider ambienceVolume;

    [Header("Automated Machinery")]
    // Singleton
    public static MainMenu I;

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
    }

    // Press play!
    public void PlayButtonPressed()
    {
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
