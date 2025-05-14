using UnityEngine;
using System.Collections.Generic;

// Oracle
public class Oracle : Talent
{
    public Oracle() : base("Oracle", "Oracle", "Common", 
                                "Oracles interpret fate and dance with destiny.", 
                                2, 0, 1, 7, 0, 0, 0, 6)
    {
    }
}

// The Star
public class TheStar : Talent
{
    private float invulnerabilityDuration = 1.0f;
    
    public TheStar() : base("The Star", "Oracle", "Common", 
                        "Gathering stars grants brief invulnerability. The path to enlightenment protects those who walk it.", 
                        0, 0, 0, 7, 0, 0, 0, 0)
    {
    }

    public override void OnLearn()
    {
        // Duration scales with talent level
        invulnerabilityDuration = 1.0f * GM.I.player.talents[myName];
    }
    
    public override void OnGather(Star star, Gatherer gatherer)
    {
        // Set invulnerable through babyTime
        float duration = invulnerabilityDuration;
        //gatherer.babyTime = gatherer.timeAlive + duration;
        
        gatherer.starTime = gatherer.timeAlive + duration;
    }
}

// The Moon
public class TheMoon : Talent
{
    private float duration = 15f;
    
    public TheMoon() : base("The Moon", "Oracle", "Common", 
                        "Gathering moons grants invisibility. We see what is presented to us.", 
                        0, 0, 0, 0, 0, 0, 0, 7)
    {
    }

    public override void OnLearn()
    {
        // Duration scales with talent level
        duration = 15f * GM.I.player.talents[myName];
    }
    
    public override void OnGather(Moon moon, Gatherer gatherer)
    {
        // Set invulnerable through babyTime
        gatherer.babyTime = gatherer.timeAlive + duration;
    }
}

// The Tower
public class TheTower : Talent
{
    private float damageReflectionPercentage = 50f;
    private float baseCooldownTime = 30f;
    private float cooldownTime;
    private float activationTimer = 0f;
    private float baseExplosionRadius = 2f;
    
    public TheTower() : base("The Tower", "Oracle", "Common", 
                        "When taking damage, reflect it back to nearby asteroids. A rising tide lifts all boats, a rising storm wrecks them.", 
                        0, 0, 0, 3, 0, 0, 0, 4)
    {
    }

    public override void OnLearn()
    {
        // Get talent level
        int talentLevel = GM.I.player.talents[myName];
        
        // Percentage scales with talent level
        damageReflectionPercentage = 50f + 5 * talentLevel;
        
        // Cooldown decreases with talent level (from 10s at level 1 to 2s at level 5)
        cooldownTime = baseCooldownTime / talentLevel;
    }
    
    public override void OnFixedUpdate(Gatherer gatherer)
    {
        if (activationTimer > 0)
            activationTimer -= Time.deltaTime;
    }
    
    public void ReflectDamage(float damage, GameObject attacker)
    {
        // If on cooldown, don't proceed
        if (activationTimer > 0)
            return;
        
        // Get asteroid that attacked
        Asteroid asteroid = attacker.GetComponent<Asteroid>();
        if (asteroid == null)
            return;
            
        // Calculate reflection damage
        float reflectionDamage = damage * (damageReflectionPercentage / 100f);
        
        // Get talent level
        int talentLevel = GM.I.player.talents[myName];
        
        // Trigger explosion with radius based on talent level
        float explosionRadius = baseExplosionRadius + (talentLevel - 1) * 0.5f;
        Asteroid.Explosion(explosionRadius, asteroid.transform.position);
        
        // Visual effect
        HitMarker.CreateCombatMarker(GM.I.player.transform.position, "The Tower!");
        
        // SFX
        GM.I.dj.PlayEffect("explosion", asteroid.transform.position);
        
        // Set cooldown
        activationTimer = cooldownTime;
    }
}