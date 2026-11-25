using UnityEngine;

// Granga's Guide to the Galaxy
public partial class GGG : MonoBehaviour
{
    // Singleton
    public static GGG I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;

        // Hide.
        gameObject.SetActive(false);
    }

    // Open Granga's Guide to the Galaxy.
    public void Open()
    {
        // Activate game object.
        gameObject.SetActive(true);
    }

    // Close Granga's Guide to the Galaxy.
    public void B_Close()
    {
        // Deactivate game object.
        gameObject.SetActive(false);
    }
}
