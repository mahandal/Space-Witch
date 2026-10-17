using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// The view deck is where ye view yer deck!
public class ViewDeck : MonoBehaviour
{
    [Header("Mini-Cards")]
    // The index of the currently selected card.
    public int selectedCardIndex = -1;

    // The mini cards in the middle.
    public List<MiniCard> minicards;

    // The index of the current page.
    public int pageIndex = 0;

    [Header("Buttons")]
    // The button taking you to the previous page of cards.
    public Button previousPage;

    // The button taking you to the next page of cards.
    public Button nextPage;

    [Header("Stardust")]
    // Text object for yer stardust count.
    public TMP_Text stardustText;

    [Header("Inspection")]
    public Inspection inspection;


    // + Initialization
    public void GoToViewDeck()
    {
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
        // Get our deck list.
        List<string> cardList = MenuManager.I.saveData.decklist;

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

        // Next/Previous page buttons
        RevealPageButtons();
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

        if (MenuManager.I.saveData.decklist.Count >= cardsNeeded)
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


    // + Cards
    // Select the mini card at the given index.
    public void SelectCard(int index)
    {
        // Set selected card index.
        selectedCardIndex = index;

        // Inspect card.
        InspectCard(index);
    }

    // Inspect a card.
    public void InspectCard(int index)
    {
        // Get card name.
        string cardName = minicards[index].nameText.text;

        // Open the inspect card screen.
        inspection.InspectCard(cardName);
    }


    // + Buttons
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

        if (cardsNeeded > MenuManager.I.saveData.decklist.Count) return;

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