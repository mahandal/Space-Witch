using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Recruitment : MonoBehaviour
{
    [Header("Recruitment")]
    // Text object for yer stardust count.
    public TMP_Text stardustText;

    // Recruitment packs.
    public List<Pack> packs;

    // Pack costs.
    public List<int> packCosts;

    // Text objects for pack costs.
    public List<TMP_Text> packCostTexts;

    // The cost to re-roll.
    public int rerollCost;

    // Text object for the cost to re-roll.
    public TMP_Text rerollCostText;

    // + Initialization

    // Open the recruitment screen.
    public void OpenRecruitmentScreen()
    {
        // Load stardust count.
        stardustText.text = "0";

        // + Generate new packs.

        // Get leader bio.
        LeaderBio leaderBio = MainMenu.I.leaderBios[MenuManager.I.saveData.leaderName];

        // Generate home pack.
        packs[0].GeneratePack(leaderBio.reinforcements, StarManager.I.planetIndex);
        if (MenuManager.I.saveData.stardust < 10)
            packCosts[0] = Random.Range(0, MenuManager.I.saveData.stardust);
        else
            packCosts[0] = Random.Range(0, 10);
        packCostTexts[0].text = "0";
        
        // Generate local pack.
        packs[1].GeneratePack(StarManager.I.currentStar.cards, StarManager.I.planetIndex);
        packCosts[1] = Random.Range(0, 100);
        packCostTexts[1].text = "0";

        // Generate mercenary pack.
        packs[2].GeneratePack(Constance.I.mercenaries);
        packCosts[2] = Random.Range(0, 1000);
        packCostTexts[2].text = "0";

        // Randomize re-roll cost.
        rerollCost = Random.Range(0, 100);
        rerollCostText.text = "0";

        // Enable the recruitment screen.
        gameObject.SetActive(true);
    }

    // + Activity
    void FixedUpdate()
    {
        // + Increment texts toward target values.

        // Stardust
        Utility.IncrementText(stardustText, 1, MenuManager.I.saveData.stardust);

        // Packs
        Utility.IncrementText(packCostTexts[0], 1, packCosts[0]);
        Utility.IncrementText(packCostTexts[1], 1, packCosts[1]);
        Utility.IncrementText(packCostTexts[2], 1, packCosts[2]);

        // Re-roll
        Utility.IncrementText(rerollCostText, 1, rerollCost);
    }

    // + Buttons

     // Recruit a pack of cards and begin a battle.
    public void B_Recruit(int option)
    {
        // Option -1: Non!
        if (option == -1)
        {
            // Begin the battle and return!
            DM.I.BeginHunt();
            return;
        }

        // Check if ye've enough stardust.
        if (packCosts[option] > MenuManager.I.saveData.stardust)
        {
            // TBD: Meep merp!
            Debug.Log("Ye can't afford this one! Ye got " + MenuManager.I.saveData.stardust + " and ye need " + packCosts[option]);

            // Return!
            return;
        } else {
            // Pay the price!
            MenuManager.I.saveData.stardust -= packCosts[option];
        }

        // Get the chosen pack.
        Pack pack = packs[option];

        // Add each card from the pack to our decklist.
        foreach(MiniCard card in pack.minicards)
        {
            // Check if minicard is active.
            if (card.gameObject.activeSelf)
            {
                // Get name.
                string cardName = card.nameText.text;

                // Add to decklist.
                MenuManager.I.saveData.decklist.Add(cardName);
            }
        }
       
        // Begin the battle!
        DM.I.BeginHunt();
    }


    // Re-roll recruitment packs.
    public void B_ReRoll()
    {
        // Check if ye've enough stardust.
        if (rerollCost > MenuManager.I.saveData.stardust)
        {
            // TBD: Meep merp!

            // Return.
            return;
        } else {
            // Pay the price!
            MenuManager.I.saveData.stardust -= rerollCost;
        }

        // Reload recruitment screen.
        OpenRecruitmentScreen();
    }
}
