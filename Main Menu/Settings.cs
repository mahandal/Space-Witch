using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // Singleton.
    public static Settings I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;

        // Start off.
        gameObject.SetActive(false);
    }

    // + Buttons

    // Return to the main menu.
    public void B_ReturnToMainMenu()
    {
        // Unfreeze time.
        Time.timeScale = 1f;
        
        // Re-load the game scene so everything is reset.
        Utility.LoadGameScene();
    }

    // Open the settings menu.
    public void B_OpenSettings()
    {
        // Freeze time.
        Time.timeScale = 0f;

        // Open the settings menu.
        gameObject.SetActive(true);
    }

    // Close the settings menu.
    public void B_CloseSettings()
    {
        // Unfreeze time.
        Time.timeScale = 1f;

        // Close the settings menu.
        gameObject.SetActive(false);
    }
}
