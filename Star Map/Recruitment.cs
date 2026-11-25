using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Recruitment : MonoBehaviour
{
    [Header("Recruitment")]
    // How many packs ye've chosen so far.
    public int packCount = 0;

    // A list of which home cards are available to be recruited from.
    public List<string> homeCards = new List<string>();

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

    [Header("View Deck")]
    public ViewDeck viewDeck;

    // + Initialization

    // Open the recruitment screen.
    public void OpenRecruitmentScreen(bool resetPackCount = true)
    {
        // DJ, hit it!
        // DJ.HitIt("Recruitment");

        // Reset pack count.
        if (resetPackCount)
            packCount = 0;

        // Load stardust count.
        stardustText.text = "0";

        // + Generate new packs.

        // Calculate the upper bound for pack cost.
        int upperBound = 100 * (1 + packCount);

        // Get leader bio.
        LeaderBio leaderBio = MainMenu.I.leaderBios[DM.I.goodLeader.myName];

        // Generate home pack.
        homeCards = new List<string>(leaderBio.t1_cards);
        if (StarManager.I.planetIndex >= 1) Utility.Append(homeCards, leaderBio.t2_cards);
        if (StarManager.I.planetIndex >= 2) Utility.Append(homeCards, leaderBio.t3_cards);
        if (StarManager.I.planetIndex >= 3) Utility.Append(homeCards, leaderBio.t4_cards);
        if (StarManager.I.planetIndex >= 4) Utility.Append(homeCards, leaderBio.t5_cards);
        packs[0].GeneratePack(homeCards);
        packCosts[0] = Random.Range(0, upperBound / 2);
        packCostTexts[0].text = "0";
        
        // Generate local pack.
        packs[1].GeneratePack(StarManager.I.currentStar.cards);
        packCosts[1] = Random.Range(0, upperBound);
        packCostTexts[1].text = "0";

        // Generate mercenary pack.
        packs[2].GeneratePack(Constance.I.mercenaries);
        packCosts[2] = Random.Range(0, upperBound * 2);
        packCostTexts[2].text = "0";

        // Randomize re-roll cost.
        rerollCost = Random.Range(0, upperBound);
        rerollCostText.text = "0";

        // Enable the recruitment screen.
        gameObject.SetActive(true);
    }

    // + Activity
    void FixedUpdate()
    {
        // + Increment texts toward target values.

        // Stardust
        Utility.IncrementText(stardustText, MenuManager.I.saveData.stardust);

        // Packs
        Utility.IncrementText(packCostTexts[0], packCosts[0]);
        Utility.IncrementText(packCostTexts[1], packCosts[1]);
        Utility.IncrementText(packCostTexts[2], packCosts[2]);

        // Re-roll
        Utility.IncrementText(rerollCostText, rerollCost);
    }

    // + Buttons

    // Go to the viewing deck to view yer deck.
    public void B_ViewDeck()
    {
        // Enable viewing deck.
        viewDeck.GoToViewDeck();
    }

    // Recruit a pack of cards.
    public void B_Recruit(int option)
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Option -1: Non!
        if (option == -1)
        {
            // Begin the battle and return!
            DM.I.BeginBattle();
            return;
        }

        // Check if ye've enough stardust.
        if (packCosts[option] > MenuManager.I.saveData.stardust)
        {
            // TBD: Meep merp!
            
            // Return!
            return;
        } else {
            // Pay the price!
            MenuManager.I.saveData.stardust -= packCosts[option];

            Utility.SaveGame();

            // Increment pack count.
            packCount++;
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
                if (DM.I.gameMode == "Campaign")
                    MenuManager.I.saveData.decklist.Add(cardName);
                else
                    DM.I.goodLeader.deck.Add(cardName);
            }
        }
       
        // Begin the battle!
        if (DM.I.gameMode == "Campaign")
            DM.I.BeginBattle();
        else
            OpenRecruitmentScreen(false);
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
        OpenRecruitmentScreen(false);
    }
}
