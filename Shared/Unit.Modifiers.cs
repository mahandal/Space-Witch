using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Unit
{
    // + Damage received
    // A dictionary mapping the names to the values of any modifiers this unit has for its damage received.
    [System.NonSerialized]
    public Dictionary<string, float> damageReceivedModifiers = new Dictionary<string, float>();

    // Return this unit's total damage received modifier.
    // Multiple each damage received modifier and return.
    public float DamageReceivedModifiers()
    {
        // Initialize to 100%.
        float modifier = 1f;

        // Iterate through each modifier.
        foreach (float value in damageReceivedModifiers.Values)
        {
            modifier *= value;
        }

        // Return.
        return modifier;
    }

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
