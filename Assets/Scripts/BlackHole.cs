using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("Black Hole")]
    // Which nebula this black hole leads to, if any.
    public string destination = "Eldest Ring";


    void FixedUpdate()
    {
        // Timers
        //Timers();

        // Rotate
        transform.Rotate(0,0, 1f);
    }
}
