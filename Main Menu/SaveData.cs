using UnityEngine;
using System;
using System.Collections.Generic;

// Simple class to store our save progress to a json file (and load from it!)
[Serializable]
public class SaveData
{
    // How much stardust we have.
    public int stardust = 0;

    // The name of the leader we are playing as.
    public string leaderName = "";

    // The name of the current star we are on.
    public string currentStarName = "";

    // How many credits we have.
    public int credits = 0;

    // Our decklist.
    public List<string> decklist = new List<string>();

    // Default constructor.
    public SaveData()
    {
    }

    // Level up a card in our deck.
    public void LevelUp(string cardName)
    {
        // Get index.
        int index = decklist.IndexOf(cardName);

        // Failed to find?
        if (index < 0)
        {
            Debug.LogWarning("Failed to find " + cardName + " while leveling up!");
            return;
        }


        // Split name into parts.
        string[] parts = cardName.Split(' ', 3);

        // Check if the card name has a level.
        if (parts[0] == "Level")
        {
            // Extract parts.
            string level = parts[1];
            string baseName = parts[2];

            // Increment level.
            int newLevel = Utility.IncrementString(level);

            // Combine name.
            string newName = "Level " + newLevel + " " + baseName;

            // Assign new name.
            decklist[index] = newName;
        }
        else
        {
            // Level up to level 2.
            decklist[index] = "Level 2 " + cardName;
        }
    }
}
