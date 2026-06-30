using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Hovered Tile")]
    public Tile hoveredTile;
    
    [Header("Selected Card")]
    public CardInHand selectedCard;

    // Singleton.
    public static InputManager I;

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
        if (GM.I.gameState != 1) return;

        // 1 selects card 0.
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            GM.I.goodLeader.hand[0].Select();

        // 2 selects card 1.
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            GM.I.goodLeader.hand[1].Select();

        // 3 selects card 2.
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            GM.I.goodLeader.hand[2].Select();

        // 4 selects card 3.
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            GM.I.goodLeader.hand[3].Select();

        // 5 selects card 4.
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            GM.I.goodLeader.hand[4].Select();

        // Right click deselects card.
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedCard != null)
            selectedCard.Deselect();

        // Left click attempts to play card.
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedCard != null && hoveredTile != null)
            GM.I.goodLeader.AttemptPlayCard(selectedCard.index, hoveredTile);

        HoverTooltip();
    }

    // Hover a unit to show a tooltip for it.
    // Also sets the currently hoveredTile.
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
        if (tileX >= 0 && tileX < GM.I.gridWidth && tileY >= 0 && tileY < GM.I.gridHeight)
        {
            // Get currently hovered tile.
            Tile tile = GM.I.grid[tileX, tileY];

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
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

        // Check if we're hovering anything.
        if (hit != null)
        {
            // Check if we found a unit.
            Unit hoveredUnit = hit.GetComponent<Unit>();

            // Show tooltip for units!
            // Hide tooltip if hovering something else.
            if (hoveredUnit != null)
                UI.I.ShowTooltip(hoveredUnit);
            else
                UI.I.HideTooltip();
        }
        else
        {
            // Hide the tooltip.
            UI.I.HideTooltip();
        }
    }
}