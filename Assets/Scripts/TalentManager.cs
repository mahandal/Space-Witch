using UnityEngine;
using System.Collections.Generic;

public class TalentManager : MonoBehaviour
{
    // Initialize all talents at the beginning of each game
    public void InitializeTalents()
    {
        // Clear all talent lists
        ClearTalents();

        // Register all talents
        RegisterTalents();
    }

    // Register all talents at the beginning of the game
    // Note: All new talents MUST be included here!
    public void RegisterTalents()
    {
        // Alchemist
        RegisterTalent(gameObject.AddComponent<Alchemist>());
        RegisterTalent(gameObject.AddComponent<HealingPotion>());
        RegisterTalent(gameObject.AddComponent<ManaPotion>());
        RegisterTalent(gameObject.AddComponent<ExplosionPotion>());

        // Engineer
        RegisterTalent(gameObject.AddComponent<Engineer>());
        RegisterTalent(gameObject.AddComponent<Jump>());
        RegisterTalent(gameObject.AddComponent<HullStrength>());
        RegisterTalent(gameObject.AddComponent<FusionCore>());
        RegisterTalent(gameObject.AddComponent<ReinventTheWheel>());
        RegisterTalent(gameObject.AddComponent<LaserDrone>());
        RegisterTalent(gameObject.AddComponent<LaserSatellite>());

        // Enchantress
        RegisterTalent(gameObject.AddComponent<Enchantress>());
        RegisterTalent(gameObject.AddComponent<StarDance>());

        // Druid
        RegisterTalent(gameObject.AddComponent<Druid>());
        RegisterTalent(gameObject.AddComponent<RecallTraining>());
        RegisterTalent(gameObject.AddComponent<SummonCrow>());

        // Oracle
        RegisterTalent(gameObject.AddComponent<Oracle>());
        RegisterTalent(gameObject.AddComponent<TheMoon>());
        RegisterTalent(gameObject.AddComponent<TheStar>());
        RegisterTalent(gameObject.AddComponent<TheTower>());
    }
    
    // Register a new talent
    private static void RegisterTalent(Talent talent)
    {
        // Add to full talent list
        GM.I.talents.Add(talent.myName, talent);

        // - Add to class list

        // Alchemist
        if (talent.myClass == "Alchemist")
        {
            // Common
            if (talent.rarity == "Common")
            {
                commonAlchemistTalents.Add(talent.myName);
            }
            // Uncommon
            else if (talent.rarity == "Uncommon")
            {
                uncommonAlchemistTalents.Add(talent.myName);
            }
            // Rare
            else if (talent.rarity == "Rare")
            {
                rareAlchemistTalents.Add(talent.myName);
            }
        }
        // Enchantress
        else if (talent.myClass == "Enchantress")
        {
            // Common
            if (talent.rarity == "Common")
            {
                commonEnchantressTalents.Add(talent.myName);
            }
            // Uncommon
            else if (talent.rarity == "Uncommon")
            {
                uncommonEnchantressTalents.Add(talent.myName);
            }
            // Rare
            else if (talent.rarity == "Rare")
            {
                rareEnchantressTalents.Add(talent.myName);
            }
        }
        // Engineer
        else if (talent.myClass == "Engineer")
        {
            // Common
            if (talent.rarity == "Common")
            {
                commonEngineerTalents.Add(talent.myName);
            }
            // Uncommon
            else if (talent.rarity == "Uncommon")
            {
                uncommonEngineerTalents.Add(talent.myName);
            }
            // Rare
            else if (talent.rarity == "Rare")
            {
                rareEngineerTalents.Add(talent.myName);
            }
        }
        // Druid
        else if (talent.myClass == "Druid")
        {
            // Common
            if (talent.rarity == "Common")
            {
                commonDruidTalents.Add(talent.myName);
            }
            // Uncommon
            else if (talent.rarity == "Uncommon")
            {
                uncommonDruidTalents.Add(talent.myName);
            }
            // Rare
            else if (talent.rarity == "Rare")
            {
                rareDruidTalents.Add(talent.myName);
            }
        }
        // Oracle
        else if (talent.myClass == "Oracle")
        {
            // Common
            if (talent.rarity == "Common")
            {
                commonOracleTalents.Add(talent.myName);
            }
            // Uncommon
            else if (talent.rarity == "Uncommon")
            {
                uncommonOracleTalents.Add(talent.myName);
            }
            // Rare
            else if (talent.rarity == "Rare")
            {
                rareOracleTalents.Add(talent.myName);
            }
        }
        else
        {
            Debug.Log("Error! Confused talent ain't got no class : " + talent.myName);
        }
    }

    // Return the name of a random talent, with the given class and rarity.
    public static string GetRandomTalent(string subclass, string rarity = "Common")
    {
        // Get full list for class and rarity
        List<string> fullTalentList = new List<string>();

        // Alchemist
        if (subclass == "Alchemist")
        {
            // Common
            if (rarity == "Common")
                fullTalentList = commonAlchemistTalents;

            // Uncommon
            if (rarity == "Uncommon")
                fullTalentList = uncommonAlchemistTalents;

            // Rare
            if (rarity == "Rare")
                fullTalentList = rareAlchemistTalents;
        }
        else if (subclass == "Enchantress")
        {
            // Common
            if (rarity == "Common")
                fullTalentList = commonEnchantressTalents;

            // Uncommon
            if (rarity == "Uncommon")
                fullTalentList = uncommonEnchantressTalents;

            // Rare
            if (rarity == "Rare")
                fullTalentList = rareEnchantressTalents;
        }
        else if (subclass == "Engineer")
        {
            // Common
            if (rarity == "Common")
                fullTalentList = commonEngineerTalents;

            // Uncommon
            if (rarity == "Uncommon")
                fullTalentList = uncommonEngineerTalents;

            // Rare
            if (rarity == "Rare")
                fullTalentList = rareEngineerTalents;
        }
        else if (subclass == "Druid")
        {
            // Common
            if (rarity == "Common")
                fullTalentList = commonDruidTalents;

            // Uncommon
            if (rarity == "Uncommon")
                fullTalentList = uncommonDruidTalents;

            // Rare
            if (rarity == "Rare")
                fullTalentList = rareDruidTalents;
        }
        else if (subclass == "Oracle")
        {
            // Common
            if (rarity == "Common")
                fullTalentList = commonOracleTalents;

            // Uncommon
            if (rarity == "Uncommon")
                fullTalentList = uncommonOracleTalents;

            // Rare
            if (rarity == "Rare")
                fullTalentList = rareOracleTalents;
        }

        // Track available talents
        List<string> availableTalents = new List<string>();

        // Go through full list
        foreach (string talent in fullTalentList)
        {
            // Check if it's unlocked
            if (GM.I.saveData.unlockedTalents.Contains(talent))
            {
                availableTalents.Add(talent);
            }
        }

        return Shuffle(availableTalents);
        /* // Alchemist
        if (subclass == "Alchemist")
        {
            // Common
            if (rarity == "Common")
                return Shuffle(commonAlchemistTalents);

            // Uncommon
            if (rarity == "Uncommon")
                return Shuffle(uncommonAlchemistTalents);

            // Rare
            if (rarity == "Rare")
                return Shuffle(rareAlchemistTalents);
        }
        else if (subclass == "Enchantress")
        {
            // Common
            if (rarity == "Common")
                return Shuffle(commonEnchantressTalents);

            // Uncommon
            if (rarity == "Uncommon")
                return Shuffle(uncommonEnchantressTalents);

            // Rare
            if (rarity == "Rare")
                return Shuffle(rareEnchantressTalents);
        }
        else if (subclass == "Engineer")
        {
            // Common
            if (rarity == "Common")
                return Shuffle(commonEngineerTalents);

            // Uncommon
            if (rarity == "Uncommon")
                return Shuffle(uncommonEngineerTalents);

            // Rare
            if (rarity == "Rare")
                return Shuffle(rareEngineerTalents);
        }
        else if (subclass == "Druid")
        {
            // Common
            if (rarity == "Common")
                return Shuffle(commonDruidTalents);

            // Uncommon
            if (rarity == "Uncommon")
                return Shuffle(uncommonDruidTalents);

            // Rare
            if (rarity == "Rare")
                return Shuffle(rareDruidTalents);
        }
        else if (subclass == "Oracle")
        {
            // Common
            if (rarity == "Common")
                return Shuffle(commonOracleTalents);

            // Uncommon
            if (rarity == "Uncommon")
                return Shuffle(uncommonOracleTalents);

            // Rare
            if (rarity == "Rare")
                return Shuffle(rareOracleTalents);
        }

        Debug.Log("Invalid subclass (or maybe rarity?) submitted when pulling talents : " + subclass);

        return null;
        */
    }

    //private static Random rng = new Random();
    
    // Shuffle a given list and return the first element from its new order.
    public static string Shuffle(List<string> shufflee)
    {
        int n = shufflee.Count;
        while (n > 1)
        {
            n--;
            //int k = rng.Next(n + 1);
            int k = Random.Range(0, n + 1);
            string value = shufflee[k];
            shufflee[k] = shufflee[n];
            shufflee[n] = value;
        }

        return shufflee[0];
    }

    public void ClearTalents()
    {
        // Alchemist
        commonAlchemistTalents.Clear();
        uncommonAlchemistTalents.Clear();
        rareAlchemistTalents.Clear();

        // Enchantress
        commonEnchantressTalents.Clear();
        uncommonEnchantressTalents.Clear();
        rareEnchantressTalents.Clear();

        // Engineer
        commonEngineerTalents.Clear();
        uncommonEngineerTalents.Clear();
        rareEnchantressTalents.Clear();

        // Druid
        commonDruidTalents.Clear();
        uncommonDruidTalents.Clear();
        rareDruidTalents.Clear();

        // Oracle
        commonOracleTalents.Clear();
        uncommonOracleTalents.Clear();
        rareOracleTalents.Clear();
    }

    // --- Talent lists (separated by class & rarity)


    // - Alchemist

    // Common
    public static List<string> commonAlchemistTalents = new List<string>();

    // Uncommon
    public static List<string> uncommonAlchemistTalents = new List<string>();

    // Rare
    public static List<string> rareAlchemistTalents = new List<string>();


    // - Enchantress

    // Common
    public static List<string> commonEnchantressTalents = new List<string>();

    // Uncommon
    public static List<string> uncommonEnchantressTalents = new List<string>();

    // Rare
    public static List<string> rareEnchantressTalents = new List<string>();


    // - Engineer

    // Common
    public static List<string> commonEngineerTalents = new List<string>();

    // Uncommon
    public static List<string> uncommonEngineerTalents = new List<string>();

    // Rare
    public static List<string> rareEngineerTalents = new List<string>();

    
    // - Druid
    
    // Common
    public static List<string> commonDruidTalents = new List<string>();

    // Uncommon
    public static List<string> uncommonDruidTalents = new List<string>();

    // Rare
    public static List<string> rareDruidTalents = new List<string>();


    // - Oracle
    
    // Common
    public static List<string> commonOracleTalents = new List<string>();

    // Uncommon
    public static List<string> uncommonOracleTalents = new List<string>();

    // Rare
    public static List<string> rareOracleTalents = new List<string>();
}