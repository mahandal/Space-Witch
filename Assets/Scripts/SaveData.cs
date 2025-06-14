using UnityEngine;
using System.Collections.Generic;

public class SaveData
{
    // Count our cash.
    public int credits = 0;

    // Remember our talents.
    public List<string> unlockedTalents = new List<string>();

    // - Planets

    // Nectar
    public List<int> planetNectar = new List<int>();

    // Investments
    public List<int> planetScience = new List<int>();
    public List<int> planetCulture = new List<int>();
    public List<int> planetEnvironment = new List<int>();
    public List<int> planetEconomy = new List<int>(); 

    public SaveData()
    {
        // Start with no money,
        credits = 0;

        // just a lil talent.
        unlockedTalents = new List<string>();
        unlockedTalents.Add("Alchemist");
        unlockedTalents.Add("Enchantress");
        unlockedTalents.Add("Engineer");
        unlockedTalents.Add("Druid");
        unlockedTalents.Add("Oracle");
    }
}
