using UnityEngine;
using System.Collections.Generic;

public class Druid : Talent
{
    public Druid() : base("Druid", "Druid", "Common", 
                                "Druids commune with nature to follow the natural flow of life.", 
                                1, 1, 6, 0, 1, 2, 3, 0)
    {
    }
}

// Recall Training
public class RecallTraining : Talent
{
    private float speedBoost = 2.0f;
    //private float distanceThreshold = 5.0f; // Distance to player when boost activates
    
    public RecallTraining() : base("Recall Training", "Druid", "Common", 
                                "Your familiar moves faster when moving toward you.", 
                                0, 0, 1, 0, 2, 4, 6, 0)
    {
    }

    public override void OnFixedUpdate(Gatherer gatherer)
    {
        Player player = GM.I.player;
        Familiar familiar = GM.I.familiar;
        
        // Calculate direction vector from familiar to player
        Vector3 toPlayer = player.transform.position - familiar.transform.position;
        float distance = toPlayer.magnitude;
        
        // Check if familiar is moving toward player
        float dotProduct = Vector3.Dot(toPlayer.normalized, familiar.rb2d.linearVelocity.normalized);
        
        if (dotProduct > 0.7f) // If moving generally toward player (cosine > 0.7)
        {
            // Apply speed boost
            float boost = speedBoost * GM.I.player.talents[myName];
            familiar.AddSpeedModifier(myName, boost);
        }
        else
        {
            // Remove speed boost if not moving toward player
            familiar.RemoveSpeedModifier(myName);
        }
    }
}

public class SummonCrow : Talent
{
    private List<Crow> activeCrows = new List<Crow>();
    private int maxCrows = 1;
    private float baseCooldown = 30f;
    
    public SummonCrow() : base("Summon Crow", "Druid", "Common", 
                            "SPELL: Summon a crow to help you gather stars. Higher levels increase max crow count and decrease cooldown.", 
                            0, 0, 1, 0, 0, 0, 2, 1)
    {
        // Spell stats
        isSpell = true;
        manaCost = 60f;
        cooldown = baseCooldown;
    }
    
    public override void OnLearn()
    {
        // Get talent level
        int talentLevel = GM.I.player.talents[myName];

        // Increase max crows
        maxCrows = talentLevel;

        // Reduce cooldown
        cooldown = baseCooldown / talentLevel;

        // Level up existing crows
        foreach (Crow crow in activeCrows)
        {
            crow.SetStats(talentLevel);
        }
    }

    // Make all crows invulnerable briefly and fully heal them.
    public void CrowTime()
    {
        foreach(Crow crow in activeCrows)
        {
            // Ignore corpses
            if (crow == null)
                continue;

            crow.SetStats();

            // Full restore
            crow.FullRestore();
            
            // Set invulnerable
            float duration = GM.I.player.talents[myName];
            crow.babyTime = crow.timeAlive + duration;
        }
    }
    
    public override void OnCast()
    {
        CrowTime();

        // Check if we already have max crows
        if (activeCrows.Count >= maxCrows)
            return;
        
        // Find furthest planet
        Planet furthestPlanet = FindFurthestPlanet();
        Vector3 spawnPosition;
        
        if (furthestPlanet != null)
        {
            // Use furthest planet position
            spawnPosition = furthestPlanet.transform.position;
        }
        else
        {
            // Fallback to a position offscreen from the player
            float distance = 30f; // Far enough to be offscreen
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            spawnPosition = GM.I.player.transform.position + new Vector3(
                randomDirection.x * distance,
                randomDirection.y * distance,
                0
            );
        }
        
        // Instantiate the crow
        Crow newCrow = Object.Instantiate(GM.I.spawnManager.progenitor_Crow, GM.I.universe);
        newCrow.transform.position = spawnPosition;

        // Set stats
        int talentLevel = GM.I.player.talents[myName];
        newCrow.SetStats(talentLevel);
        
        // Set up the crow
        newCrow.witch = GM.I.player;
        newCrow.gameObject.SetActive(true);
        
        // Add to active crows list
        activeCrows.Add(newCrow);
        
        // SFX
        //GM.I.dj.PlayEffect("crow_0", newCrow.transform.position, 13f);
        GM.I.dj.PlayEffect("crow", GM.I.player.transform.position);
        
        // Visual feedback
        HitMarker.CreateLearnMarker(GM.I.player.transform.position, "Crow Summoned!");
    }
    
    private Planet FindFurthestPlanet()
    {
        if (GM.I.planets == null || GM.I.planets.Count == 0)
            return null;
            
        Planet furthestPlanet = null;
        float maxDistance = 0f;
        
        foreach (Planet planet in GM.I.planets)
        {
            float distance = Vector3.Distance(planet.transform.position, GM.I.player.transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPlanet = planet;
            }
        }
        
        return furthestPlanet;
    }
}