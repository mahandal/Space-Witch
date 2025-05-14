using UnityEngine;

public class Moon : MonoBehaviour
{
    public bool isBeingGathered = false;
    public Planet planet;
    void FixedUpdate()
    {
        // Rotate
        transform.Rotate(0,0,-13f);
    }
}
