using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MiniCard : MonoBehaviour
{
    [Header("Mini Card")]
    // The canvas group for this mini card.
    public CanvasGroup canvasGroup;

    // This card's highlight.
    public Image highlight;
    
    // The text object for this card's name.
    public TMP_Text nameText;

    // This card's background image.
    public Image backgroundImage;

    // This card's main art.
    public Image art;

    // The text object for this card's mana cost.
    public TMP_Text manaText;

    // The text object for this card's deployment time.
    public TMP_Text timeText;

    // The text object for this card's role.
    public TMP_Text roleText;

    // The image for this card's role.
    public Image roleIcon;

    // Load a card.
    public void LoadCard(string cardName)
    {
        // Get card.
        Card card = DM.I.grimoire[cardName];

        // Set name.
        nameText.text = cardName;

        // Set mana cost.
        manaText.text = card.manaCost.ToString();

        // Set deployment time.
        timeText.text = card.deployTime.ToString();

        // Set role.
        roleText.text = card.role;

        // Load images.
        Utility.LoadImage(art, "Cards/" + card.myName);
        Utility.LoadImage(backgroundImage, "Cards/" + card.myName);
        Utility.LoadImage(roleIcon, "Roles/" + card.role);
    }

    // + Highlight
    // Highlight this mini card.
    public void Highlight()
    {
        highlight.gameObject.SetActive(true);
    }

    // Un-highlight this mini card.
    public void Unhighlight()
    {
        highlight.gameObject.SetActive(false);
    }

    // + Button
    // Select this button to examine it.
    public void B_CardPressed()
    {
        Debug.Log(nameText.text + " pressed!");

        // TBD: Examine cards?
    }
}
