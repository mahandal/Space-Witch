using UnityEngine;
using UnityEngine.InputSystem;

public class InputBattle : MonoBehaviour
{
    [Header("Hover")]
    // The currently hovered unit.
    public Unit hoveredUnit;

    // The currently hovered tile.
    public Tile hoveredTile;
    
    [Header("Selected Card")]
    // The currently selected card.
    public CardInHand selectedCard;

    // Singleton.
    public static InputBattle I;

    // Awaken!
    void Awake()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);
    }

    // Update for inputs.
    void Update()
    {
        // Wait for a battle.
        if (MenuManager.I.gameState != 1) return;

        // Hover a unit to show its tooltip.
        HoverTooltip();

        // 1 selects card 0.
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            DM.I.goodLeader.hand[0].Select();

        // 2 selects card 1.
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            DM.I.goodLeader.hand[1].Select();

        // 3 selects card 2.
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            DM.I.goodLeader.hand[2].Select();

        // 4 selects card 3.
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            DM.I.goodLeader.hand[3].Select();

        // 5 selects card 4.
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            DM.I.goodLeader.hand[4].Select();

        // Right click deselects card.
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedCard != null)
            selectedCard.Deselect();

        // Left click attempts to play card.
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedCard != null && hoveredTile != null)
            DM.I.goodLeader.AttemptPlayCard(selectedCard.index, hoveredTile);

        // + Leader abilities
        // Left clicked on a unit?
        if (Mouse.current.leftButton.wasPressedThisFrame && hoveredUnit != null)
        {
            // Can't target enemy deployment zone.
            if (DM.I.goodLeader.IsInEnemyDeploymentZone(hoveredUnit))
            {
                return;
            }
            // Dying units.
            else if (hoveredUnit.state == -1)
            {
                return;
            }
            // Deploying units
            else if (hoveredUnit.state == 0)
            {
                // Guinevere
                // (Handled in CardInHand)
                // if (DM.I.goodLeader.myName == "Guinevere")
                //     DM.I.goodLeader.GuinevereSing(hoveredUnit);
            }
            // Items?
            else if (hoveredUnit.cardType == "Item")
            {
                // Shruk
                if (DM.I.goodLeader.myName == "Shruk")
                    DM.I.goodLeader.ShrukEat(hoveredUnit);
            }
            // Clicking on friendlies:
            else if (hoveredUnit.good)
            {
                // Sybil
                if (DM.I.goodLeader.myName == "Sybil Solisi")
                    DM.I.goodLeader.SybilHeal(hoveredUnit);
                // Gatama
                else if (DM.I.goodLeader.myName == "Gatama the Seer")
                    DM.I.goodLeader.GatamaHeal(hoveredUnit);
                // Lancelot
                else if (DM.I.goodLeader.myName == "Lancelot")
                    DM.I.goodLeader.Sacrifice(hoveredUnit);
            } else {
                // Clicking on enemies:
                // Morgan le Fey
                if (DM.I.goodLeader.myName == "Morgan le Fey")
                    DM.I.goodLeader.MorganCharm(hoveredUnit);

                // Wubalin Brightforge
                else if (DM.I.goodLeader.myName == "Wubalin Brightforge")
                    DM.I.goodLeader.WubalinShoot(hoveredUnit);


                // Markaus Allstrong
                else if (DM.I.goodLeader.myName == "Markaus Allstrong")
                    DM.I.goodLeader.MarkausPunch(hoveredUnit);

                // Penelope
                else if (DM.I.goodLeader.myName == "Penelope")
                    DM.I.goodLeader.PenEat(hoveredUnit);
            }
        }
    }


    // Hover a unit to show a tooltip for it.
    // Sets hoveredUnit and hoveredTile.
    public void HoverTooltip()
    {
        // Convert our mouse position to 3d coordinates.
        Vector3 mouseScreen = Mouse.current.position.ReadValue();

        // Set the mouse's z to opposite the camera's, for some reason?
        mouseScreen.z = -Camera.main.transform.position.z;

        // Convert our mouse's 3d coordinates to 2d coordinates.
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        // + Hovered tile
        // Convert to grid coordinates.
        int tileX = Mathf.FloorToInt(mouseWorld.x);
        int tileY = Mathf.FloorToInt(mouseWorld.y);

        // Check if within grid bounds.
        if (tileX >= 0 && tileX < DM.I.gridWidth && tileY >= 0 && tileY < DM.I.gridHeight)
        {
            // Get currently hovered tile.
            Tile tile = DM.I.grid[tileX, tileY];

            // Clear highlight of last highlighted tile.
            if (hoveredTile != null && tile != hoveredTile)
                hoveredTile.Unhighlight();

            // Set hovered tile.
            hoveredTile = tile;

            // Highlight.
            hoveredTile.Highlight();
        }
        else
        {
            // Clear highlight of last highlighted tile.
            if (hoveredTile != null)
                hoveredTile.Unhighlight();

            // Set hovered tile to null.
            hoveredTile = null;
        }


        // + Tooltip
        // Look for a collider near our mouse.
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, Constance.I.unitLayer);

        // Check if we're hovering anything.
        if (hit != null)
        {
            // Check if we found a unit.
            hoveredUnit = hit.GetComponent<Unit>();

            // Show tooltip for units!
            // Hide tooltip if hovering something else.
            if (hoveredUnit != null)
                UI.I.ShowTooltip(hoveredUnit);
            else
                UI.I.HideTooltip();
        }
        else
        {
            // Set hovered unit to null.
            hoveredUnit = null;
            
            // Hide the tooltip.
            UI.I.HideTooltip();
        }
    }
}