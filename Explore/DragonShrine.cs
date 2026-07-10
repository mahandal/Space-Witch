using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Pray at the dragon shrine to edit your squad/deck.
public class DragonShrine : MonoBehaviour
{
    [Header("Mode")]
    // Whether we are looking at our exploring squad or our resting army.
    // Always set to either "Exploring" or "Resting".
    // Could be an enum or an int or even a bool! But this works too.
    public string mode = "Exploring";

    [Header("Mini-Cards")]
    // The mini card for your squad leader AKA who you play as in explore mode.
    public MiniCard squadLeaderCard;

    // The index of the currently selected card.
    public int selectedCardIndex = -1;

    // The mini cards in the middle.
    public List<MiniCard> minicards;

    // The index of the current page.
    public int pageIndex = 0;

    [Header("Buttons")]
    public CanvasGroup exploringButton;
    public CanvasGroup restingButton;

    // The button taking you to the previous page of cards.
    public Button previousPage;

    // The button taking you to the next page of cards.
    public Button nextPage;

    // The button to promote the currently selected unit to squad leader AKA who you play as.
    public Button promoteToSquadLeader;

    // The button to add the currently resting unit to your exploring squad.
    public Button addToSquad;

    // The button to return your currently exploring unit to rest.
    public Button returnToRest;

    // The button to sell your currently selected unit, in exchange for credits.
    public Button sell;

    // Singleton
    public static DragonShrine I;

    // + Initialization
    public void Initialize()
    {
        if (I == null || I == this)
            I = this;
        else
            Destroy(this);
    }

    // Pray at the dragon shrine!
    public void Pray()
    {
        // Start exploring.
        B_Exploring();

        // Load squad leader.
        LoadSquadLeader();

        // Load our first page.
        // LoadFirstPage();

        // Activate the parent game object.
        gameObject.SetActive(true);
    }

    // Load the exploring squad leader.
    public void LoadSquadLeader()
    {
        // Load squad leader.
        string squadLeaderName = GM.I.exploring[0];
        squadLeaderCard.LoadCard(squadLeaderName);
    }

    // Load the first page of cards.
    // Uses the current mode.
    public void LoadFirstPage()
    {
        // + Load mini cards.
        // Set page index to 0.
        pageIndex = 0;

        // Load your deck, shown in the middle.
        LoadCards();  
    }

    // Load the mini-cards in the middle.
    public void LoadCards()
    {
        // Get which list to use.
        List<string> cardList = GM.I.exploring;
        if (mode == "Resting")
            cardList = GM.I.resting;

        // Loop through each mini card.
        for (int i = 0; i < minicards.Count; i++)
        {
            // Use page index to access lists bigger than 12.
            int cardIndex = i + (pageIndex * minicards.Count);

            // Check if there is a card to load.
            if (cardIndex < cardList.Count)
            {
                // Get card name.
                string cardName = cardList[cardIndex];

                // Load card.
                minicards[i].LoadCard(cardName);

                // Reveal card.
                minicards[i].gameObject.SetActive(true);
            } else {
                // Hide card.
                minicards[i].gameObject.SetActive(false);
            }
        }

        // Deselect previously selected card.
        Deselect(); 

        // Next/Previous page buttons
        RevealPageButtons();
    }

    // Reveal or hide the previous page and next page buttons, depending on if we have enough cards to use them.
    public void RevealPageButtons()
    {
        // + Previous
        if (pageIndex == 0)
            previousPage.gameObject.SetActive(false);
        else
            previousPage.gameObject.SetActive(true);

        // + Next
        nextPage.gameObject.SetActive(true);

        // To use the next page button, you need at least this many cards in your exploring squad/resting army.
        int cardsNeeded = (pageIndex + 1) * minicards.Count;

        if (mode == "Exploring" && cardsNeeded > GM.I.exploring.Count)
            nextPage.gameObject.SetActive(false);
        if (mode == "Resting" && cardsNeeded > GM.I.resting.Count)
            nextPage.gameObject.SetActive(false);
    }

    // + Cards
    // Select the mini card at the given index.
    public void SelectCard(int index)
    {
        // Unhighlight previously selected card.
        if (selectedCardIndex >= 0)
            minicards[selectedCardIndex].Unhighlight();

        // Set selected card index.
        selectedCardIndex = index;

        // Highlight.
        minicards[index].Highlight();

        // + Buttons
        // Reveal 'Add to Squad' button for resting units, hide for exploring units.
        addToSquad.gameObject.SetActive(mode == "Resting");

        // For exploring units that are not the squad leader,
        // reveal buttons 'Return to Rest' and 'Promote to Squad Leader'.
        // Hide those buttons for resting units, and for your squad leader.
        if (mode == "Exploring" && (selectedCardIndex != 0 || pageIndex != 0))
        {
            returnToRest.gameObject.SetActive(true);
            promoteToSquadLeader.gameObject.SetActive(true);
        }
        else
        {
            returnToRest.gameObject.SetActive(false);
            promoteToSquadLeader.gameObject.SetActive(false);
        }

        // Hide sell button only when selecting your squad leader.
        if (mode == "Exploring" && pageIndex == 0 && selectedCardIndex == 0)
            sell.gameObject.SetActive(false);
        else
            sell.gameObject.SetActive(true);
    }

    // Deselect any card.
    public void Deselect()
    {
        // Unhighlight previously selected card.
        if (selectedCardIndex >= 0)
            minicards[selectedCardIndex].Unhighlight();

        // Set selected card index.
        selectedCardIndex = -1;

        // Hide buttons.
        addToSquad.gameObject.SetActive(false);
        returnToRest.gameObject.SetActive(false);
        promoteToSquadLeader.gameObject.SetActive(false);
        sell.gameObject.SetActive(false);
    }

    // + Buttons
    // Load your exploring squad.
    public void B_Exploring()
    {
        // Set mode to exploring.
        mode = "Exploring";

        // Load our first page.
        LoadFirstPage();

        // Set opacity of buttons.
        exploringButton.alpha = 1f;
        restingButton.alpha = 0.5f;
    }

    // Load your resting army.
    public void B_Resting()
    {
        // Set mode to resting.
        mode = "Resting";

        // Load our first page.
        LoadFirstPage();

        // Set opacity of buttons.
        restingButton.alpha = 1f;
        exploringButton.alpha = 0.5f;
    }

    // Go to the next page of cards.
    public void B_NextPage()
    {
        // Can't go forward past the last page.
        int cardsNeeded = (pageIndex + 1) * minicards.Count;

        if (mode == "Exploring" && cardsNeeded > GM.I.exploring.Count) return;
        if (mode == "Resting" && cardsNeeded > GM.I.resting.Count) return;


        // Increment pageIndex.
        pageIndex++;

        // Re-load cards.
        LoadCards();
    }

    // Go to the previous page of cards.
    public void B_PreviousPage()
    {
        // Can't go back from the first page.
        if (pageIndex == 0) return;

        // Decrement page index.
        pageIndex--;

        // Re-load cards.
        LoadCards();
    }

    // Return the selected explorer to rest.
    public void B_ReturnToRest()
    {
        // Get card name.
        string cardName = minicards[selectedCardIndex].nameText.text;

        // Add to list of resting units.
        GM.I.resting.Add(cardName);

        // Remove from list of exploring units.
        GM.I.exploring.Remove(cardName);

        // Delete unit.
        Object.Destroy(GM.I.player.squad[selectedCardIndex].gameObject);

        // Remove from squad.
        GM.I.player.squad.RemoveAt(selectedCardIndex);

        // Re-load cards.
        LoadCards();
    }

    // Add the selected unit to your exploring squad.
    public void B_AddToSquad()
    {
        // Get card name.
        string cardName = minicards[selectedCardIndex].nameText.text;

        // Add to list of exploring units.
        GM.I.exploring.Add(cardName);

        // Remove from list of resting units.
        GM.I.resting.Remove(cardName);

        // Spawn unit.
        Vector3 spawnPosition = GM.I.currentPlanet.exploreStart.position;
        Unit newUnit = GM.I.SpawnUnit(cardName, spawnPosition);

        // Add to squad.
        GM.I.player.squad.Add(newUnit);
        newUnit.squadLeader = GM.I.player;

        // Re-load cards.
        LoadCards();
    }

    // Set the selected unit as your squad leader, making them the player character.
    public void B_PromoteToSquadLeader()
    {
        // Get the currently selected unit.
        int cardIndex = (pageIndex * minicards.Count) + selectedCardIndex;
        Unit newLeader = GM.I.player.squad[cardIndex];

        // Remember which unit was the leader, to keep in the squad.
        Unit oldLeader = GM.I.player;

        // Set as new first index in exploring and squad.
        GM.I.exploring[0] = newLeader.myName;
        GM.I.player.squad[0] = newLeader;

        // Put old leader in new leader's old place.
        GM.I.exploring[cardIndex] = oldLeader.myName;
        oldLeader.squad[cardIndex] = oldLeader;

        // Set as player.
        GM.I.player = newLeader;

        // Transfer over squad.
        newLeader.squad = oldLeader.squad;
        foreach (Unit squadMember in oldLeader.squad)
        {
            squadMember.squadLeader = newLeader;
        }

        // Say goodbye.
        oldLeader.squad = null;

        // Re-load squad leader.
        LoadSquadLeader();

        // Return to the first page, to show your leader now in their new place.
        LoadFirstPage();
    }

    // Sell the selected card for credits.
    public void B_Sell()
    {
        // Get card name.
        string cardName = minicards[selectedCardIndex].nameText.text;

        // Get progenitor.
        Unit p = Progenitors.I.GetProgenitor(cardName);

        // Gain credits.
        MenuManager.I.saveData.credits += p.creditCost;

        // Remove from decklist.
        MenuManager.I.saveData.decklist.Remove(cardName);

        // Remove from explore list.
        if (mode == "Exploring")
        {
            // Remove from list of exploring units.
            GM.I.exploring.Remove(cardName);

            // Delete unit.
            Object.Destroy(GM.I.player.squad[selectedCardIndex].gameObject);

            // Remove from squad.
            GM.I.player.squad.RemoveAt(selectedCardIndex);
        }
        else
        {
            // Remove from list of resting units.
            GM.I.resting.Remove(cardName);
        }

        // Re-load cards.
        LoadCards();
    }
}
