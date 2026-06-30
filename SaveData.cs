using UnityEngine;
using System;
using System.Collections.Generic;

// Simple class to store our save progress to a json file (and load from it!)
[Serializable]
public class SaveData
{
    // The name of the leader we are playing as.
    public string leaderName;

    // The name of the current star we are on.
    public string currentStarName = "";

    // Our decklist.
    public List<string> decklist = new List<string>();

    // Default constructor.
    public SaveData()
    {
        // Starter decks!

        // + Knights of Camelot
        // decklist.Add("Sword Fighter");
        // decklist.Add("Sword Fighter");
        // decklist.Add("Sword Fighter");
        // decklist.Add("Sword Fighter");
        // decklist.Add("Sword Fighter");
        // decklist.Add("Bow Shooter");
        // decklist.Add("Bow Shooter");
        // decklist.Add("Bow Shooter");
        // decklist.Add("Armored Cavalier");
        // decklist.Add("Armored Cavalier");
        // decklist.Add("King of the People");
        // decklist.Add("Wise Witch");
        // decklist.Add("Wise Witch");

        // + Beasts of Sarras
        // decklist.Add("Charm");
        // decklist.Add("Polar Bear");
        // decklist.Add("Lion");
        // decklist.Add("Moose");
        // decklist.Add("Rhino");
        // decklist.Add("Hyena");
        // decklist.Add("Hyena");
        // decklist.Add("Hyena");
        // decklist.Add("Wallflower");
        // decklist.Add("Wallflower");
        // decklist.Add("Wallflower");
        // decklist.Add("Wallflower");
        // decklist.Add("Wallflower");

        // + Dwarven Rebellion
        // decklist.Add("Laser Turret");
        // decklist.Add("Laser Turret");
        // decklist.Add("Steel Sword");
        // decklist.Add("Steel Sword");
        // decklist.Add("Steel Sword");
        // decklist.Add("Hero of the Hill Tribes");
        // decklist.Add("Syndicate Enforcer");
        // decklist.Add("Syndicate Enforcer");
        // decklist.Add("Murder Hobo");
        // decklist.Add("Murder Hobo");
        // decklist.Add("Murder Hobo");
        // decklist.Add("Murder Hobo");
        // decklist.Add("Murder Hobo");

        // + Roaming Legion of Orkney
        // decklist.Add("Roaming Warrior");
        // decklist.Add("Roaming Warrior");
        // decklist.Add("Roaming Archer");
        // decklist.Add("Roaming Archer");
        // decklist.Add("Roaming Raider");
        // decklist.Add("Roaming Raider");
        // decklist.Add("Roaming Captain");
        // decklist.Add("Roaming Barracks");

        // + Ogres of Logres
        // decklist.Add("Ogrite");
        // decklist.Add("Ogrite");
        // decklist.Add("Ogrite");
        // decklist.Add("Ogrite");
        // decklist.Add("Ogrite");
        // decklist.Add("Chocolate Chip Cookie");
        // decklist.Add("Chocolate Chip Cookie");
        // decklist.Add("Chocolate Chip Cookie");
        // decklist.Add("Chocolate Chip Cookie");
        // decklist.Add("Chocolate Chip Cookie");
        // decklist.Add("Ogre");
        // decklist.Add("Ogre");
        // decklist.Add("Chicken Drumstick");
        // decklist.Add("Chicken Drumstick");
        // decklist.Add("Giant");

        // + Dragons
        // decklist.Add("Dragonling");
        // decklist.Add("Dragonling");
        // decklist.Add("Dragonling");
        // decklist.Add("Dragonling");
        // decklist.Add("Dragonling");
        // decklist.Add("Wyrm");
        // decklist.Add("Wyrm");
        // decklist.Add("Fireball");
        // decklist.Add("Fireball");
        // decklist.Add("Polar Bear");
        // decklist.Add("Dragon");

        // + Lake Folk
        // decklist.Add("Mermaid");
        // decklist.Add("Mermaid");
        // decklist.Add("Mermaid");
        // decklist.Add("Mermaid");
        // decklist.Add("Mermaid");
        // decklist.Add("Giant Tortoise");
        // decklist.Add("Giant Tortoise");
        // decklist.Add("Summon Water Elemental");
        // decklist.Add("Summon Water Elemental");
        // decklist.Add("Summon Water Elemental");
        // decklist.Add("Medusa");
        // decklist.Add("Excalibur");

        // + The Dead of Dolorous Gard
        // decklist.Add("Skeleton Warrior");
        // decklist.Add("Skeleton Warrior");
        // decklist.Add("Skeleton Warrior");
        // decklist.Add("Skeleton Warrior");
        // decklist.Add("Skeleton Warrior");
        // decklist.Add("Skeleton Archer");
        // decklist.Add("Skeleton Archer");
        // decklist.Add("Skeleton Archer");
        // decklist.Add("Skeleton Archer");
        // decklist.Add("Skeleton Archer");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Skull");
        // decklist.Add("Summon Ghost");
        // decklist.Add("Summon Ghost");
        // decklist.Add("Lich");
    }
}
