using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // + Buttons

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
