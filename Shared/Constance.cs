using UnityEngine;
using System.Collections.Generic;

public class Constance : MonoBehaviour
{
    [Header("Mercenaries")]
    public List<string> mercenaries;

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
