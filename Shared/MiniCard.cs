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
        // Get progenitor.
        Unit p = Progenitors.I.GetProgenitor(cardName);

        // Set name.
        nameText.text = cardName;

        // Set mana cost.
        manaText.text = p.manaCost.ToString();

        // Set deployment time.
        if (p.keywords.Contains("Chaotic"))
            timeText.text = p.deployTime.ToString() + " - " + (p.deployTime * 2f).ToString();
        else
            timeText.text = p.deployTime.ToString();

        // Set role.
        Utility.SetRoleText(p, roleText);

        // Load images.
        Utility.LoadImage(art, "Cards/" + p.myName);
        Utility.LoadImage(backgroundImage, "Cards/" + p.myName);
        Utility.LoadImage(roleIcon, "Roles/" + p.role);
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
