using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Explorer
{
	// + Speed
    // A dictionary mapping the names to the values of any modifiers this unit has for its movement speed.
    [System.NonSerialized]
    public Dictionary<string, float> speedModifiers = new Dictionary<string, float>();

    // Return this unit's total speed modifier.
    // Multiple each modifier and return.
    public float SpeedModifiers()
    {
        // Initialize to 100%.
        float modifier = 1f;

        // Iterate through each modifier.
        foreach (float value in speedModifiers.Values)
        {
            modifier *= value;
        }

        // Return.
        return modifier;
    }
}
