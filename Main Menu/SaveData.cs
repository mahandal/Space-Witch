using UnityEngine;
using System;
using System.Collections.Generic;

// Simple class to store our save progress to a json file (and load from it!)
[Serializable]
public class SaveData
{
    // How much stardust we have.
    public int stardust = 100;

    // The name of the leader we are playing as.
    public string leaderName = "Dreaming Soul";

    // The name of the current star we are on.
    public string currentStarName = "";

    // Our current planet index.
    public int currentPlanetIndex = 1;

    // How many credits we have.
    public int credits = 0;

    // Our decklist.
    public List<string> decklist = new List<string>();

    // Default constructor.
    public SaveData()
    {
    }
}
