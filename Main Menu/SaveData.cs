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

    // How many credits we have.
    public int credits = 0;

    // Our decklist.
    public List<string> decklist = new List<string>();

    // Default constructor.
    public SaveData()
    {
    }
}
