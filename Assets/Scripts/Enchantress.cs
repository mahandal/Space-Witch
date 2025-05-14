using UnityEngine;

// Enchantress
public class Enchantress : Talent
{
    public Enchantress() : base("Enchantress", "Enchantress", "Common", 
                                "Enchant the world with style and grace.", 
                                0, 6, 4, 2, 0, 2, 0, 0)
    {
    }
}

// Star Dance
public class StarDance : Talent
{
    // Speed boost amount per star
    private float speedBoost = 0.6f;

    // Max boost total
    private float maxBoost = 3.0f;

    // Duration (resets each star, non-stacking)
    private float boostDuration = 2.0f;

    // Timer
    private float boostTimer = 0f;
    
    public StarDance() : base("Star Dance", "Enchantress", "Common", 
                            "Gathering stars gives speed.", 
                            0, 1, 1, 0, 0, 1, 1, 0)
    {
    }

    public override void OnGather(Star star, Gatherer gatherer)
    {
        // Get boost amount
        float boost = speedBoost * GM.I.player.talents[myName];

        // Scale with current boost if extant
        if (gatherer.accelerationModifiers.ContainsKey(myName))
            boost *= gatherer.accelerationModifiers[myName];

        // Cap the maximum boost
        maxBoost = 3 + GM.I.player.talents[myName];
        if (boost > maxBoost)
            boost = maxBoost;

        // Apply speed boost
        //gatherer.AddAccelerationModifier(myName, 1.0f + boost);
        gatherer.AddSpeedModifier(myName, 1.0f + boost);
        
        // Reset timer
        boostTimer = boostDuration;
    }
    
    public override void OnFixedUpdate(Gatherer gatherer)
    {
        // Count down timer if active
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            
            // Remove boost when timer expires
            if (boostTimer <= 0)
            {
                //gatherer.RemoveAccelerationModifier(myName);
                gatherer.RemoveSpeedModifier(myName);
            }
        }
    }
}