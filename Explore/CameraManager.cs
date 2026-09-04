using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // The default position of the camera, for all screens except explore mode.
    private Vector3 defaultPosition;

    // Singleton
    public static CameraManager I;

    // + Initialization
    void Awake()
    {
        if (I == null || I == this)
            I = this;
        else
            Destroy(gameObject);

        // Remember default position.
        defaultPosition = Camera.main.transform.position;
    }

    // Reset the main camera to its default position.
    public void ResetToDefaultPosition()
    {
        Camera.main.transform.position = defaultPosition;
    }

    // + Explore
    void FixedUpdate()
    {
        // While in explore mode, follow the player.
        if (GM.I.gameObject.activeSelf)
            FollowPlayer();
        else
            ResetToDefaultPosition(); // TBD: Optimize!
    }

    // Set the camera's position to the player's position, except keeping z.
    public void FollowPlayer()
    {
        // Get new position.
        Vector3 newPosition = new Vector3(GM.I.player.transform.position.x,
                                          GM.I.player.transform.position.y,
                                          defaultPosition.z);
        // Set camera's position.
        Camera.main.transform.position = newPosition;
    }
}
