using UnityEngine;

public partial class GGG
{
    // Get the given keyword's description.
    public string GetKeywordDescription(string keyword)
    {
        string s = "";

        // Aquatic
        if (keyword == "Aquatic")
s = "This unit moves faster in water.";

        // Bash
        if (keyword == "Bash")
s = "This unit has a 25% chance to stun enemies for a random duration with its attacks.";

        // Mega Bash
        if (keyword == "Mega Bash")
s = "This unit stuns enemies for a random duration with its attacks.";

        // Bloodthirst
        if (keyword == "Bloodthirst")
s = "This unit gains 1% damage with each attack. Not much, but it can add up!";

        // Blood Oath
        if (keyword == "Blood Oath")
s = "This unit has sworn a sacred blood oath, for which they pay the price with each attack.";

        // Chaotic
        if (keyword == "Chaotic")
s = "This unit's deployment time randomly fluctuates.";

        // Charming
        if (keyword == "Charming")
s = "This unit fights not to kill, but to convert others to join their cause.";

        // Cleave
        if (keyword == "Cleave")
s = "This unit's attacks deal 50% damage to all other enemies near their target.";

        // Decaying
        if (keyword == "Decaying")
s = "This unit passively loses 8% of its max health each second. All things fade in time...";

        // Demolition
        if (keyword == "Demolition")
s = "This unit deals 500% damage to structures. Break down those walls!";

        // Dragonblood
        if (keyword == "Dragonblood")
s = "The blood of dragons is hot as molten lava, and indeed lava is their preferred terrain. Where others burn, dragons heal like a day at the spa!";

        // Dwarf
        if (keyword == "Dwarf")
s = "Dwarves are sturdy creatures, particularly adept at resisting stuns.";

        // Laser
        if (keyword == "Laser")
s = "Lasers are cool! Pew pew!";

        // Lich
        if (keyword == "Lich")
s = "A lich is immortal, simply put.";

        // Poisoned
        if (keyword == "Poisoned")
s = "This unit is losing 10% of its max health per second.";

        // Skeleton
        if (keyword == "Skeleton")
s = "On death, spawn a Skull.";

        // Big Skeleton
        if (keyword == "Big Skeleton")
s = "On death, spawn a Skeleton Warrior.";

        // Summon
        if (keyword == "Summon")
s = "Summon spells allow ye to conjure temporary units outside yer own territory.";

        // Troll
        if (keyword == "Troll")
s = "Trolls heal for 10% of their max health each second.";

        // Vampire
        if (keyword == "Vampire")
s = "Vampires steal health from their foes with each attack.";

        // Venomous
        if (keyword == "Venomous")
s = "This unit inflicts poison with each attack.";

        // Return!
        return s;
    }
}
