using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// Yer grimoire manages yer card collection.
// Unlock, favorite, and hide cards here.
public class Grimoire : MonoBehaviour
{
    [Header("Grimoire")]
    // Which hero we're looking at.
    public string currentHero;

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
    // A list of all the cards we are currently looking at.
    // (Including other pages!)
    public List<string> cards;

    [Header("Buttons")]
    // The button taking you to the previous page of cards.
    public Button previousPage;

    // The button taking you to the next page of cards.
    public Button nextPage;

    // Parent object of the buttons that show up when ye select a card.
    public GameObject selectedParent;

    [Header("Stardust")]
    // Text object for yer stardust count.
    public TMP_Text stardustText;

    // Singleton
    public static Grimoire I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;
    }

    // Open the Grimoire.
    public void OpenGrimoire()
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Deselect.
        Deselect();

        // Pull hero from skirmish settings.
        currentHero = MainMenu.I.skirmishHero;

        // Get leader bio.
        LeaderBio bio = MainMenu.I.leaderBios[currentHero];

        // Get card list.
        cards = new List<string>(bio.t1_cards);
        foreach (string s in bio.t2_cards) cards.Add(s);
        foreach (string s in bio.t3_cards) cards.Add(s);
        foreach (string s in bio.t4_cards) cards.Add(s);
        foreach (string s in bio.t5_cards) cards.Add(s);

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

        // Load cards.
        LoadCards();  
    }

    // Load the mini-cards in the middle.
    public void LoadCards()
    {
        // Loop through each mini card.
        for (int i = 0; i < minicards.Count; i++)
        {
            // Use page index to access pages beyond the first.
            int cardIndex = i + (pageIndex * minicards.Count);

            // Check if there is a card to load.
            if (cardIndex < cards.Count)
            {
                // Get card name.
                string cardName = cards[cardIndex];

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

    // + Activity
    void Update()
    {
        // Right click deselects.
        if (Mouse.current.rightButton.wasPressedThisFrame)
            Deselect();

        // Escape also deselects.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Deselect();
    }

    void FixedUpdate()
    {
        // Stardust
        Utility.IncrementText(stardustText, MenuManager.I.saveData.stardust);
    }

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
        // To use the next page button, you need at least this many cards.
        int cardsNeeded = (pageIndex + 1) * minicards.Count + 1;

        if (cards.Count >= cardsNeeded)
            nextPage.gameObject.SetActive(true);
        else
            nextPage.gameObject.SetActive(false);
    }    

    // + Buttons
    // Select the mini card at the given index.
    public void SelectCard(int index)
    {
        // Deselect previous card.
        Deselect();

        // Set selected card index.
        selectedCardIndex = index;

        // Set card name.
        selectedCardName = minicards[index].nameText.text;

        // Reveal buttons
        selectedParent.SetActive(true);

        // Highlight
        minicards[selectedCardIndex].Highlight();
    }

    // Inspect the currently selected card.
    public void B_Inspect()
    {
        Inspection.I.InspectCard(selectedCardName);
    }

    // Go back to the main menu.
    public void B_Back()
    {
        // SFX
        DJ.I.PlaySound("Snap");

        // Close grimoire.
        gameObject.SetActive(false);
    }

    // Go to the next page of cards.
    public void B_NextPage()
    {
        // Can't go forward past the last page.
        int cardsNeeded = (pageIndex + 1) * minicards.Count;

        if (cardsNeeded > cards.Count) return;

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
