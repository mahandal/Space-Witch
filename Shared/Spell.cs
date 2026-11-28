using UnityEngine;
using System.Collections.Generic;

public class Spell : MonoBehaviour
{
    // Cast a spell!
    // Called from a spell's cast animation.
    // Note: Spells are stored as Units, and use their stats when possible.
    // E.g. range is used for the radius of many spells.
    public static void Cast(Unit spell)
    {
        Debug.Log("Casting a spell: " + spell.myName);

        // SFX
        DJ.I.PlaySpellCastSound(spell);

        // + Find targets.
        List<Unit> targets = new List<Unit>();

        // Hexes and curses target enemies
        if (spell.role == "Hex" || spell.role == "Curse")
        {
            // Get a list of all enemies.
            List<Unit> enemies = DM.I.GetAllEnemies(spell);

            // Iterate through each enemy.
            foreach (Unit enemy in enemies)
            {
                // Check if they are close enough.
                float distance = Vector3.Distance(spell.transform.position, enemy.transform.position);

                // If they are within the spell's range, add them to our list of targets.
                if (distance <= spell.range)
                    targets.Add(enemy);
            }
        }

        // Blessings
        if (spell.role == "Blessing")
        {
            // Get a list of all our friends in range.
            List<Unit> friends = DM.I.GetAllFriends(spell);

            // Iterate through each friend.
            foreach (Unit friend in friends)
            {
                // Check if they are close enough.
                float distance = Vector3.Distance(spell.transform.position, friend.transform.position);

                if (distance <= spell.range)
                {
                    // Add to list of targets.
                    targets.Add(friend);

                    // Target picks up blessing like an item.
                    friend.PickUp(spell);
                }
            }
        }

        // +++ Cast spell

        // + Blessings

        // Blessings giving stats are handled above like items.

        // Blink
        if (spell.GetBaseName() == "Blink")
        {
            // Move units forward a tile.
            foreach (Unit target in targets)
            {
                if (target.cardType == "Unit")
                    TeleportForward(target, 1f);
            }
        }

        // Promotion
        if (spell.GetBaseName() == "Promotion")
        {
            // Promote each target to the next level.
            foreach (Unit unit in targets)
            {
                unit.LevelUp();
            }
        }

        // Resurrection
        if (spell.GetBaseName() == "Resurrection")
        {
            // Get all friendly dead units in range.
            targets = DM.I.GetDeadFriends(spell, spell.range);

            // Resurrect each target.
            foreach (Unit target in targets)
            {
                // Re-spawn.
                Unit newUnit = spell.GetLeader().SpawnUnit(target.myName, target.currentTile);

                // Show its full deployment.
                newUnit.showFullDeployment = true;

                // Remove from graveyard.
                DM.I.RemoveFromGraveyard(target);
            }
        }

        // + Summons

        // Summon Ghost
        if (spell.GetBaseName() == "Summon Ghost")
        {
            // Change from spell to unit.
            spell.myName = "Ghost";
            spell.cardType = "Unit";
            spell.role = "Hunter";

            // Add level to name.
            if (spell.level > 1)
                spell.myName = "Level " + spell.level + " Ghost";
        }

        // Summon Hunger Demon
        if (spell.GetBaseName() == "Summon Hunger Demon")
        {
            // Change from spell to unit.
            spell.myName = "Hunger Demon";
            spell.cardType = "Unit";
            spell.role = "Hunter";

            // Add level to name.
            if (spell.level > 1)
                spell.myName = "Level " + spell.level + " Hunger Demon";
        }

        // Summon Water Elemental
        if (spell.GetBaseName() == "Summon Water Elemental")
        {
            // Change from spell to unit.
            spell.myName = "Water Elemental";
            spell.cardType = "Unit";
            spell.role = "Scout";

            // Add level to name.
            if (spell.level > 1)
                spell.myName = "Level " + spell.level + " Water Elemental";
        }

        // + Curses

        // Charm
        if (spell.GetBaseName() == "Charm")
        {
            // Charm all targets!
            foreach (Unit newFriend in targets)
            {
                newFriend.ChangeSides();
            }
        }

        // Polymorph
        if (spell.GetBaseName() == "Polymorph")
        {
            // Transform all units into deer!
            foreach (Unit target in targets)
            {
                if (target.cardType == "Unit")
                    target.LoadUnit("Deer");
            }
        }

        // Toxic spores
        if (spell.GetBaseName() == "Toxic Spores")
        {
            // Poison all targets.
            foreach (Unit target in targets)
            {
                target.keywords.Add("Poisoned");
            }
        }

        // + Hexes

        // Fireball
        if (spell.GetBaseName() == "Fireball")
        {
            // Damage all targets.
            foreach (Unit target in targets)
            {
                target.LoseHealth(spell.damage, spell);
            }
        }

        // Goblin Bomb
        if (spell.GetBaseName() == "Goblin Bomb")
        {
            // Damage all targets.
            foreach (Unit target in targets)
            {
                target.LoseHealth(spell.damage, spell);
            }
        }
    }

    // Teleport a unit forward a given distance.
    public static void TeleportForward(Unit unit, float distance)
    {
        // Add distance to position, multiplied by direction.
        float newX = unit.transform.position.x + distance * unit.direction;

        // Minimum of 1.
        if (newX < 1)
            newX = 1f;

        // Maximum of gridWidth - 2.
        if (newX > DM.I.gridWidth - 2)
            newX = DM.I.gridWidth - 2;

        unit.transform.position = new Vector3(newX, unit.transform.position.y, unit.transform.position.z);
    }
}
