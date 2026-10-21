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

    // + Levels
    public int RandomLevel(int powerLevel = 0)
    {
        // Default to level 2.
        int level = 2;

        // Roll for higher level.
        int d100 = Random.Range(1, 101);

        // 80% level 2
        // 15% level 3
        // 4% level 4
        // 1% level 5
        if (d100 > 80 && d100 < 95)
            level = 3;
        else if (d100 >= 95 && d100 < 100)
            level = 4;
        else if (d100 == 100)
            level = 5;

        // Power level?
        level += powerLevel;

        // Return.
        return level;
    }

    // + Mercenaries

    // Return the name of a random mercenary.
    public string RandomMerc()
    {
        // Roll.
        int index = Random.Range(0, mercenaries.Count);

        // Get merc.
        string merc = mercenaries[index];

        // Return.
        return merc;
    }
}
