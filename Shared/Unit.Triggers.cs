using UnityEngine;

public partial class Unit
{
    // Called each tick.
    // (In FixedUpdate)
    public void OnTick()
    {
        // Summon
        if (keywords.Contains("Summon"))
            LoseHealth(maxHealth * 0.05f * Time.fixedDeltaTime, this, true, false);

        // Poisoned
        if (keywords.Contains("Poisoned"))
            LoseHealth(maxHealth * 0.1f * Time.fixedDeltaTime, this, true);

        // Troll regeneration.
        // Note: the 'else' makes poison super effective against trolls!
        else if (keywords.Contains("Troll"))
            GainHealth(maxHealth * 0.1f * Time.fixedDeltaTime);
    }

    // Called when this unit dies.
    // Handles on death triggers.
    public void OnDeath()
    {
        // Prevent persisting into further battles.
        if (MenuManager.I.gameState != 1) return;
        
        // Skeleton
        if (keywords.Contains("Skeleton"))
        {
            // Spawn a skull.
            Unit newUnit = GetLeader().SpawnUnit("Skull", currentTile);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }

        // Big Skeleton
        if (keywords.Contains("Big Skeleton"))
        {
            // Spawn a skull.
            Unit newUnit = GetLeader().SpawnUnit("Skeleton Warrior", currentTile);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }

        // Lich
        if (keywords.Contains("Lich"))
        {
            // The lich respawns!
            Unit newUnit = GetLeader().SpawnUnit("Lich", currentTile);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }
    }

    // On attack triggers.
    // Called when this unit gets to the Attack() frame in its attack animation.
    public void OnAttack()
    {
        // Bloodthirst?
        if (keywords.Contains("Bloodthirst"))
        {
            damage *= 1.01f;
        }

        // Cleave?
        if (keywords.Contains("Cleave"))
        {
            // Iterate through each other enemy unit nearby.
            foreach(Unit enemy in GetEnemiesNear(target))
            {
                // Deal half damage.
                enemy.LoseHealth(damage / 2f, this);
            }
        }

        // Laser?
        if (keywords.Contains("Laser"))
        {
            // Draw a red line to our target.
            attackLine.SetPosition(0, transform.position);
            attackLine.SetPosition(1, target.transform.position);
            attackLine.enabled = true;

            // Fade the laser after a brief delay.
            StartCoroutine(FadeLaser());
        }
    }
}
