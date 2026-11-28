using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// The view deck is where ye view yer deck!
public class ViewDeck : MonoBehaviour
{
    [Header("Mini-Cards")]
    // The mini cards in the middle.
    public List<MiniCard> minicards;

    // The index of the currently selected card.
    public int selectedCardIndex = -1;

    // The name of the currently selected card.
    public string selectedCardName = "";

    // The index of the current page.
    public int pageIndex = 0;

    [Header("Cards")]
    // The player's current deck.
    public List<string> deck = new List<string>();

    [Header("Next/Prev")]
    // The button taking you to the previous page of cards.
    public Button previousPage;

    // The button taking you to the next page of cards.
    public Button nextPage;

    [Header("Stardust")]
    // Text object for yer stardust count.
    public TMP_Text stardustText;

    [Header("Selected Buttons")]
    // Parent object of level up and send home buttons, to hide when not selecting a card.
    public GameObject selectedParent;

    // Cost to level up.
    public int cost;
    public TMP_Text costText;

    // Stardust gained from sending home.
    public int stardustGained;
    public TMP_Text stardustGainedText;


    // + Initialization
    public void GoToViewDeck()
    {
        // Get deck.
        if (DM.I.gameMode == "Campaign")
            deck = MenuManager.I.saveData.decklist;
        else
            deck = DM.I.goodLeader.deck;

        // Deselect.
        Deselect();

        // Load first page.
        LoadFirstPage();

        // Reset stardust count.
        stardustText.text = "0";

        // Enable game object.
        gameObject.SetActive(true);
    }

    // Load the first page of cards.
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
        // Loop through each mini card.
        for (int i = 0; i < minicards.Count; i++)
        {
            // Use page index to access lists bigger than 12.
            int cardIndex = i + (pageIndex * minicards.Count);

            // Check if there is a card to load.
            if (cardIndex < deck.Count)
            {
                // Get card name.
                string cardName = deck[cardIndex];

                // Load card.
                minicards[i].LoadCard(cardName);

                // Reveal card.
                minicards[i].gameObject.SetActive(true);
            } else {
                // Hide card.
                minicards[i].gameObject.SetActive(false);
            }
        }

        // Next/Previous page buttons
        RevealPageButtons();

        // Level up/Send home buttons
        RevealSelectedButtons();
    }

    // Reveal or hide the level up and send home buttons, depending on if we are selecting a card or not.
    public void RevealSelectedButtons()
    {
        if (selectedCardIndex >= 0)
            selectedParent.SetActive(true);
        else
            selectedParent.SetActive(false);
    }

    // Reveal or hide the previous page and next page buttons, depending on if we have enough cards to use them.
    public void RevealPageButtons()
    {
        // + Previous
        // Can't go back from the first page.
        if (pageIndex == 0)
            previousPage.gameObject.SetActive(false);
        else
            previousPage.gameObject.SetActive(true);

        // + Next
        // To use the next page button, you need at least this many cards in your deck.
        int cardsNeeded = (pageIndex + 1) * minicards.Count + 1;

        if (deck.Count >= cardsNeeded)
            nextPage.gameObject.SetActive(true);
        else
            nextPage.gameObject.SetActive(false);
    }


    // + Activity
    void FixedUpdate()
    {
        // Stardust
        Utility.IncrementText(stardustText, MenuManager.I.saveData.stardust);
    }

    void Update()
    {
        // Right click deselects.
        if (Mouse.current.rightButton.wasPressedThisFrame)
            Deselect();

        // Escape also deselects.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Deselect();
    }


    // + Cards
    // Deselect
    public void Deselect()
    {
        // Reset selected card.
        if (selectedCardIndex >= 0)
            minicards[selectedCardIndex].Unhighlight();
        selectedCardIndex = -1;
        selectedCardName = "";
        selectedParent.SetActive(false);
    }

    // Select the mini card at the given index.
    public void SelectCard(int index)
    {
        // Deselect previous card.
        Deselect();

        // Set selected card index.
        selectedCardIndex = index;

        // Use inspection's dummy unit.
        Unit inspectedUnit = Inspection.I.inspectedUnit;

        // Get card name.
        selectedCardName = minicards[index].nameText.text;

        // Load dummy unit.
        inspectedUnit.LoadUnit(selectedCardName, true);

        // Set stardust.
        stardustText.text = MenuManager.I.saveData.stardust.ToString();

        // Set cost to level.
        cost = (int)Mathf.Pow(10, inspectedUnit.level);
        costText.text = cost.ToString();

        // Set stardust gained from sending home.
        stardustGained = Mathf.RoundToInt((inspectedUnit.manaCost + inspectedUnit.deployTime) * inspectedUnit.level);
        stardustGainedText.text = stardustGained.ToString();

        // Reveal buttons
        selectedParent.SetActive(true);

        // Highlight
        minicards[selectedCardIndex].Highlight();
    }

    // Inspect a card.
    public void InspectCard()
    {
        // Open the inspect card screen.
        Inspection.I.InspectCard(selectedCardName);
    }


    // + Buttons
    // Spend stardust to level up the current card.
    public void B_LevelUp()
    {
        // Check if ye've enough stardust.
        if (MenuManager.I.saveData.stardust < cost)
            return;

        // Spend stardust.
        MenuManager.I.saveData.stardust -= cost;

        // Level up.
        // string leveledCard = MenuManager.I.saveData.LevelUp(selectedCardName);
        Utility.LevelCardInDeck(deck, selectedCardName);

        // Re-load cards.
        LoadCards();

        // Re-select card.
        SelectCard(selectedCardIndex);
    }

    // Send home the current card, giving ye stardust in exchange.
    public void B_SendHome()
    {
        // Remove from decklist.
        deck.Remove(selectedCardName);

        // Gain stardust.
        MenuManager.I.saveData.stardust += stardustGained;

        // Deselect.
        Deselect();

        // Re-load cards.
        LoadCards();
    }

    // Go back to the recruitment screen.
    public void B_Back()
    {
        // Reset recruitment's stardust text.
        StarManager.I.recruitment.stardustText.text = "0";

        // Close view deck.
        gameObject.SetActive(false);
    }

    // Go to the next page of cards.
    public void B_NextPage()
    {
        // Can't go forward past the last page.
        int cardsNeeded = (pageIndex + 1) * minicards.Count;

        if (cardsNeeded > deck.Count) return;

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
}