using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Unit
{
    // Called each tick.
    // (In FixedUpdate)
    public void OnTick()
    {
        // Summon
        if (keywords.Contains("Summon"))
            LoseHealth(maxHealth * 0.05f * Time.fixedDeltaTime, this, true, false);

        // Decaying
        if (keywords.Contains("Decaying"))
            LoseHealth(maxHealth * 0.08f * Time.fixedDeltaTime, this, true);

        // Poisoned
        if (keywords.Contains("Poisoned"))
        {
            // Count stacks of poison.
            int stacks = Utility.CountStringsInList(keywords, "Poisoned");

            // Lose health.
            LoseHealth(maxHealth * 0.1f * stacks * Time.fixedDeltaTime, this, true);
        }

        // Troll regeneration.
        if (keywords.Contains("Troll"))
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
            Unit newUnit = GetLeader().SpawnUnit("Skull", currentTile, level);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }

        // Big Skeleton
        if (keywords.Contains("Big Skeleton"))
        {
            // Spawn a skull.
            Unit newUnit = GetLeader().SpawnUnit("Skeleton Warrior", currentTile, level);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }

        // Lich
        if (keywords.Contains("Lich"))
        {
            // The lich respawns!
            Unit newUnit = GetLeader().SpawnUnit("Lich", currentTile, level);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }
    }

    // On attack triggers.
    // Called when this unit gets to the Attack() frame in its attack animation.
    public void OnAttack()
    {
        // Bloodthirst
        if (keywords.Contains("Bloodthirst"))
        {
            damage *= 1.01f;
        }

        // Blood Oath
        if (keywords.Contains("Blood Oath"))
        {
            // Deal half damage to yerself.
            LoseHealth(damage / 2f);
        }

        // Cleave
        if (keywords.Contains("Cleave"))
        {
            // Iterate through each other enemy unit nearby.
            foreach(Unit enemy in GetEnemiesNear(target))
            {
                // Deal half damage.
                enemy.LoseHealth(damage / 2f, this);
            }
        }

        // Laser
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

    private IEnumerator FadeLaser()
    {
        yield return new WaitForSeconds(0.1f);
        attackLine.enabled = false;
    }

    // On deal damage.
    public float OnDealDamage(Unit target, float healthLost)
    {
        // Bash
        if (keywords.Contains("Bash"))
        {
            // Roll to stun.
            int d100 = Random.Range(1, 101);

            // Dwarves resist stuns.
            if (target.keywords.Contains("Dwarf"))
                d100 -= 25;

            // 50% chance to stun.
            if (d100 > 50)
                target.Stun();
        }

        // Mega Bash
        if (keywords.Contains("Mega Bash"))
        {
            target.Stun();
        }

        // Demolition
        if (keywords.Contains("Demolition") && target.cardType == "Structure")
            healthLost *= 2f;

        // Vampire
        if (keywords.Contains("Vampire") && target != this)
            GainHealth(healthLost);

        // Venomous
        if (keywords.Contains("Venomous") && target.cardType != "Structure")
            target.keywords.Add("Poisoned");
        

        // Track damage dealt.
        if (target != this)
        {
            damageDealt += healthLost;
            GetLeader().damageDealt += healthLost;
        }
        
        return healthLost;
    }

    // On receive damage.
    public float OnReceiveDamage(Unit source, float healthLost)
    {
        // Return.
        return healthLost;
    }

    // On receiving healing.
    public float OnHeal(float healthGained, Unit source = null)
    {
        // Track healing.
        // (Unless we're vital cause leaders handle that.)
        if (role != "Base")
            GetLeader().healingDone += healthGained;

        // Return.
        return healthGained;
    }
}
