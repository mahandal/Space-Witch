using UnityEngine;

public class Sun : MonoBehaviour
{
    void FixedUpdate()
    {
        // Timers
        //Timers();

        // Rotate
        transform.Rotate(0,0, -0.1f);
    }
}
