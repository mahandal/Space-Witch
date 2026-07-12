using UnityEngine;

public class Constance : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask unitLayer;

    // Singleton.
    public static Constance I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(this);
    }
}
