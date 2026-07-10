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

        // + Find targets.
        List<Unit> targets = new List<Unit>();

        // Hexes target enemies
        if (spell.role == "Hex")
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
        // TBD: Beneficial spells. Blessings?

        // + Cast spell

        // Charm
        if (spell.myName == "Charm")
        {
            // Charm all targets!
            foreach (Unit newFriend in targets)
            {
                newFriend.ChangeSides();
            }
        }

        // Toxic spores
        if (spell.myName == "Toxic Spores")
        {
            // Poison all targets.
            foreach (Unit target in targets)
            {
                target.keywords.Add("Poisoned");
            }
        }

        // Fireball
        if (spell.myName == "Fireball")
        {
            // Damage all targets.
            foreach (Unit target in targets)
            {
                target.LoseHealth(spell.damage, spell);
            }
        }

        // Summon Water Elemental
        if (spell.myName == "Summon Water Elemental")
        {
            // Change from spell to unit.
            spell.myName = "Water Elemental";
            spell.cardType = "Unit";
            spell.role = "Scout";
        }

        // Summon Ghost
        if (spell.myName == "Summon Ghost")
        {
            // Change from spell to unit.
            spell.myName = "Ghost";
            spell.cardType = "Unit";
            spell.role = "Hunter";
        }
    }
}
