using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [Header("Coordinates")]
    // Coordinates.
    public int x;
    public int y;

    [Header("Claim")]
    // + Claim
    //  1 = good
    //  0 = neutral
    // -1 = evil
    public int claim = 0;

    [Header("Structure")]
    // If there is a structure on this tile, store a reference to it here!
    public Unit structure;

    [Header("Machinery")]
    // Image
    public Image image;

    // Outline.
    public Image outline;

    // - Colors
    // The color shown when hovered.
    public Color hoverColor = new Color(1f, 1f, 1f, 1f);

    // Outline color for valid tiles.
    public Color validColor = new Color(0f, 1f, 0f, 0.1f);

    // Outline color for invalid tiles.
    public Color invalidColor = new Color(1f, 0f, 0f, 0.1f);

    // Outline color for structures when playing another structure.
    public Color structureColor = new Color (1f, 1f, 0f, 1f);

    // Awaken!
    void Awake()
    {
        // Get image.
        image = GetComponent<Image>();

        // Hide.
        image.color = Color.clear; 
    }

    // Fixed update.
    void FixedUpdate()
    {
        // Check if the player is playing a card.
        if (InputBattle.I.selectedCard != null)
        {
            // Get card.
            Card card = DM.I.Grimoire(InputBattle.I.selectedCard.myName);

            // Check if we are playing a spell.
            if (card.cardType == "Spell")
            {
                // Highlight all good and neutral tiles.
                if (claim >= 0)
                    outline.color = validColor;
            }
            // All other card types require playing in one of your tiles.
            else if (claim > 0)
            {
                // Check if we are playing a structure, to look for a structure in the way.
                if (card.cardType == "Structure")
                {
                    // Valid!
                    if (structure == null)
                        outline.color = validColor;
                    // Can't build on yer base!
                    else if (structure.role == "Base")
                        outline.color = invalidColor;
                    // Structure!
                    else
                        outline.color = structureColor;
                } else {
                    // Valid!
                    outline.color = validColor;
                }
            }
            else
            {
                // Invalid: Out of deployment zone.
                outline.color = invalidColor;
            }
        } else {
            // No card being played, so clear the outline.
            outline.color = Color.clear;
        }
    }

    public void Highlight()
    {
        image.color = hoverColor;
    }

    public void Unhighlight()
    {
        image.color = Color.clear;
    }
}
