using UnityEngine;
using System.Collections.Generic;

// Engineer
public class Engineer : Talent
{
    public Engineer() : base("Engineer", "Engineer", "Common", 
                                "Engineers learn the mechanical intricacies of how the world is put together.", 
                                4, 4, 0, 0, 2, 2, 0, 0)
    {
    }
}

public class Jump : Talent
{
    public float jumpStrength = 5f;
    public Jump() : base("Jump", "Engineer", "Common", 
                                "SPELL: Warp jump forward with sudden force. Higher levels reduce cooldown.", 
                                0, 0, 0, 0, 0, 0, 0, 0)
    {
        // Spell stats
        isSpell = true;
        manaCost = 50f;
        //cooldown = 1f;
    }

    public override void OnLearn()
    {
        cooldown = 6 / GM.I.player.talents[myName];
    }

    public override void OnCast()
    {
        // Add force
        Vector3 direction = (GM.I.player.cursorPosition - GM.I.player.transform.position).normalized;
        GM.I.player.rb2d.AddForce(direction * jumpStrength, ForceMode2D.Impulse);

        // SFX
        GM.I.dj.PlayEffect("jump", GM.I.player.transform.position);
    }
}

// Hull Strength
public class HullStrength : Talent
{
    
    public HullStrength() : base("Hull Strength", "Engineer", "Common", 
                                "Gain bonus Body.", 
                                0, 9, 0, 0, 0, 6, 0, 0)
    {
    }
}

// Fusion Core
public class FusionCore : Talent
{
    public FusionCore() : base("Fusion Core", "Engineer", "Common", 
                                "Gain movement speed.", 
                                1, 3, 0, 0, 0, 2, 0, 0)
    {
    }

    public override void OnLearn()
    {
        // Get talent level
        int talentLevel = GM.I.player.talents[myName];
        
        // Add speed modifier (50% per level)
        float speedBoost = 1.0f + (0.5f * talentLevel);
        GM.I.player.AddSpeedModifier(myName, speedBoost);
    }
}

// Reinvent the Wheel
public class ReinventTheWheel : Talent
{
    public float rotationBoost = 0.5f;  // 50% faster rotation per level
    public float accelerationBoost = 0.2f;  // 20% acceleration per level
    public float stayPowerBoost = 0.25f;

    public ReinventTheWheel() : base("Reinvent the Wheel", "Engineer", "Common", 
                                "Improve rotation speed.", 
                                2, 1, 0, 0, 1, 0, 0, 0)
    {
    }

    public override void OnLearn()
    {
        // Get talent level
        int talentLevel = GM.I.player.talents[myName];
        
        // Add acceleration modifier
        float speedBoost = 1.0f + (accelerationBoost * talentLevel);
        GM.I.player.AddAccelerationModifier(myName, speedBoost);

        // Add rotation modifier
        GM.I.player.AddRotationModifier(myName, 1f + (rotationBoost * talentLevel));

        // Add stay modifier
        float stayMultiplier = 1.0f + (stayPowerBoost * talentLevel);
        GM.I.familiar.AddStayPowerModifier(myName, stayMultiplier);
    }
}

// Laser Drone
public class LaserDrone : Talent
{
    private List<Drone> activeDrones = new List<Drone>();
    private int maxDrones = 1;
    private float baseCooldown = 30f;
    
    public LaserDrone() : base("Laser Drone", "Engineer", "Common", 
                            "SPELL: Build a mechanical drone that follows you and shoots asteroids with a laser beam. Higher levels increase drone count and decrease cooldown.", 
                            2, 0, 0, 0, 0, 1, 0, 0)
    {
        // Spell stats
        isSpell = true;
        manaCost = 80f;
        cooldown = baseCooldown;
    }
    
    public override void OnLearn()
    {
        // Get current talent level
        int talentLevel = GM.I.player.talents[myName];

        // Increase max drones
        maxDrones = talentLevel;

        // Reduce cooldown
        cooldown = baseCooldown / talentLevel;

        // Level up existing drones
        foreach (Drone drone in activeDrones)
        {
            drone.SetStats(talentLevel);
        }
    }

    // Make all drones invulnerable briefly and fully heal them.
    public void DroneTime()
    {
        int talentLevel = GM.I.player.talents[myName];

        foreach(Drone drone in activeDrones)
        {
            // Ignore corpses
            if (drone == null)
                continue;

            // Set stats
            drone.SetStats(talentLevel);

            // Full restore
            drone.FullRestore();
            
            // Set invulnerable
            float duration = talentLevel;
            drone.babyTime = drone.timeAlive + duration;
        }
    }

    public override void OnCast()
    {
        /* // Check if we already have max drones
        if (activeDrones.Count >= maxDrones)
            return; */

        // If we're already at max satellites, remove the oldest one
        if (activeDrones.Count >= maxDrones)
        {
            // Find the oldest satellite (first one in list)
            if (activeDrones.Count > 0 && activeDrones[0] != null)
            {
                Object.Destroy(activeDrones[0].gameObject);
                activeDrones.RemoveAt(0);
            }
            else
            {
                // Clean up any null references in the list
                activeDrones.RemoveAll(item => item == null);
            }
        }
        
        // Instantiate the drone
        Drone newDrone = Object.Instantiate(GM.I.spawnManager.progenitor_Drone, GM.I.universe);
        
        // Set position to mouse cursor
        //Vector3 newPosition = GM.I.player.transform.position;
        //newPosition.y = newPosition.y - 1f;
        newDrone.transform.position = GM.I.player.cursorPosition;

        // Set stats
        int talentLevel = GM.I.player.talents[myName];
        newDrone.SetStats(talentLevel);
        
        // Activate drone
        newDrone.gameObject.SetActive(true);
        
        // Add to active drones list
        activeDrones.Add(newDrone);
        
        // SFX
        GM.I.dj.PlayEffect("drone_deployed", GM.I.player.transform.position);
        
        // Visual feedback
        HitMarker.CreateLearnMarker(GM.I.player.transform.position, "Drone Built!");

        // Drone time!
        DroneTime();
    }
}

// Laser Satellite
public class LaserSatellite : Talent
{
    private List<Satellite> activeSatellites = new List<Satellite>();
    private int maxSatellites = 1;
    private float baseCooldown = 30f;
    
    public LaserSatellite() : base("Laser Satellite", "Engineer", "Common", 
                            "SPELL: Deploy a stationary satellite that fires powerful lasers at nearby asteroids. Higher levels increase satellite count and decrease cooldown.", 
                            2, 0, 0, 0, 0, 1, 0, 0)
    {
        // Spell stats
        isSpell = true;
        manaCost = 100f;
        cooldown = baseCooldown;
    }
    
    public override void OnLearn()
    {
        // Get current talent level
        int talentLevel = GM.I.player.talents[myName];

        // Increase max satellites
        maxSatellites = talentLevel;

        // Reduce cooldown
        cooldown = baseCooldown / talentLevel;

        // Level up existing satellites
        foreach (Satellite satellite in activeSatellites)
        {
            if (satellite != null)
                satellite.SetStats(talentLevel);
        }
    }

    // Make all satellites invulnerable briefly and fully heal them
    public void SatelliteTime()
    {
        int talentLevel = GM.I.player.talents[myName];

        foreach(Satellite satellite in activeSatellites)
        {
            // Ignore destroyed satellites
            if (satellite == null)
                continue;

            // Set stats
            satellite.SetStats(talentLevel);

            // Full restore
            satellite.FullRestore();
            
            // Set invulnerable
            float duration = talentLevel;
            satellite.babyTime = satellite.timeAlive + duration;
        }
    }

    public override void OnCast()
    {
        // If we're already at max satellites, remove the oldest one
        if (activeSatellites.Count >= maxSatellites)
        {
            // Find the oldest satellite (first one in list)
            if (activeSatellites.Count > 0 && activeSatellites[0] != null)
            {
                Object.Destroy(activeSatellites[0].gameObject);
                activeSatellites.RemoveAt(0);
            }
            else
            {
                // Clean up any null references in the list
                activeSatellites.RemoveAll(item => item == null);
            }
        }
        
        // Instantiate the satellite
        Satellite newSatellite = Object.Instantiate(GM.I.spawnManager.laser_satellite, GM.I.universe);

        // Set position to mouse cursor
        newSatellite.transform.position = GM.I.player.cursorPosition;

        // Set stats
        int talentLevel = GM.I.player.talents[myName];
        newSatellite.SetStats(talentLevel);
        
        // Activate satellite
        newSatellite.gameObject.SetActive(true);
        
        // Add to active satellites list
        activeSatellites.Add(newSatellite);
        
        // SFX
        GM.I.dj.PlayEffect("satellite_deployed", GM.I.player.transform.position); // Reuse drone sound for now
        
        // Visual feedback
        HitMarker.CreateLearnMarker(GM.I.player.transform.position, "Satellite Deployed!");

        // Satellite time!
        SatelliteTime();
    }
}