using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardOnPlanet : MonoBehaviour
{
    [Header("Meta")]
    // This card's name.
    public string myName;

    [Header("Text Fields")]
    public TMP_Text nameText;
    public TMP_Text manaText;
    public TMP_Text timeText;
    public TMP_Text roleText;

    [Header("Images")]
    // The image for this card's role.
    public Image roleIcon;

    // The image for this card's art.
    public Image image;

    // The background image for this card (matching the main image).
    public Image backgroundImage;

    [Header("Machinery")]
    // A canvas group to fade the opacity of the card.
    public CanvasGroup canvasGroup;

    // Select this card.
    public void Select()
    {
        // Add to deck.
        AddCardToDeck();

        // Close planet screen.
        StarManager.I.planetScreen.SetActive(false);

        // Begin battle.
        GM.I.BeginBattle();
    }

    // Load a card into this position.
    public void LoadCard(string cardName)
    {
        // Get card.
        Card card = GM.I.grimoire[cardName];
        
        // Set name.
        myName = card.myName;
        nameText.text = myName;

        // Set mana cost.
        manaText.text = card.manaCost.ToString();

        // Set deployment time.
        timeText.text = card.deployTime.ToString();

        // Set role.
        roleText.text = card.role;

        // Load images.
        Utility.LoadImage(image, "Cards/" + card.myName);
        Utility.LoadImage(backgroundImage, "Cards/" + card.myName);
        Utility.LoadImage(roleIcon, "Roles/" + card.role);
    }

    // Add this card to your deck.
    // Multiple copies are added for cards with low mana costs.
    // Number of copies = 10 / mana cost
    public void AddCardToDeck()
    {
        // Get card.
        Card card = GM.I.grimoire[myName];

        // Calculate how many to add.
        // (Set to 10 in mana cost is 0, so we don't divide by zero.)
        int numCopies = 10;
        if (card.manaCost > 0)
            numCopies /= card.manaCost;

        // Minimum of 1.
        if (numCopies < 1)
            numCopies = 1;

        // Add to deck!
        for (int i = 0; i < numCopies; i++)
        {
            MainMenu.I.saveData.decklist.Add(myName);
        }
    }
}
