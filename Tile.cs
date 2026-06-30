using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [Header("Coordinates")]
    // Coordinates.
    public int x;
    public int y;

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

    // The color the outline is set to for valid tiles when playing a card.
    public Color validColor = new Color(0f, 1f, 0f, 0.1f);

    // The color the outline is set to for invalid tiles when playing a card.
    public Color invalidColor = new Color(1f, 0f, 0f, 0.1f);

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
        if (InputManager.I.selectedCard != null)
        {
            // Get card.
            Card card = GM.I.grimoire[InputManager.I.selectedCard.myName];

            // Check if we are playing a spell.
            if (card.cardType == "Spell")
            {
                // Highlight all tiles outside of our enemy's deployment zone.
                if (GM.I.goodLeader.IsInEnemyDeploymentZone(this))
                    outline.color = invalidColor;
                else
                    outline.color = validColor;
            }
            // All other card types require playing in your deployment zone.
            else if (GM.I.goodLeader.IsInDeploymentZone(this))
            {
                // Check if we are playing a structure or item, to look for structures in the way.
                if (card.cardType == "Structure" || card.cardType == "Item")
                {
                    // Valid!
                    if (structure == null)
                        outline.color = validColor;
                    // Invalid: Structure in the way.
                    else
                        outline.color = invalidColor;
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

    // void OnMouseEnter()
    // {
    //     // Set as hovered tile.
    //     InputManager.I.hoveredTile = this;

    //     // Highlight.
    //     image.color = hoverColor;
    // }

    // void OnMouseExit()
    // {
    //     // Clear hovered tile.
    //     if (InputManager.I.hoveredTile == this)
    //         InputManager.I.hoveredTile = null;

    //     // Clear highlight.
    //     image.color = Color.clear;
    // }
}
