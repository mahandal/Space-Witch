using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public void ContinuePressed()
    {
        // Use pause system(?)
        GM.I.TogglePause();
    }
}
