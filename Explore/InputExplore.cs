using UnityEngine;
using UnityEngine.InputSystem;

public class InputExplore : MonoBehaviour
{
    [Header("Hover")]
    public Unit hoveredUnit;

    // Singleton.
    public static InputExplore I;

    // + Initialization
    void Awake()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(this);
    }

    // + Exploring
    void Update()
    {
        // Dying?
        if (GM.I.player.state == -1) return;
        
        // Stunned?
        if (GM.I.player.state == 3) return;

        // Deploying?
        if (GM.I.player.deployTimer > 0f) return;

        // + Get mouse position in world coordinates.

        // Convert our mouse position to 3d coordinates.
        Vector3 mouseScreen = Mouse.current.position.ReadValue();

        // Set the mouse's z to opposite the camera's, for some reason?
        mouseScreen.z = -Camera.main.transform.position.z;

        // Convert our mouse's 3d coordinates to 2d coordinates.
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        // Hover to show tooltips.
        HoverTooltip(mouseWorld);
        
        // WASD = Move up, left, down, right.
        GM.I.player.isPressingUp = Keyboard.current.wKey.isPressed;
        GM.I.player.isPressingDown = Keyboard.current.sKey.isPressed;
        GM.I.player.isPressingLeft = Keyboard.current.aKey.isPressed;
        GM.I.player.isPressingRight = Keyboard.current.dKey.isPressed;

        // Shift = Sprint
        if (Keyboard.current.leftShiftKey.isPressed)
            GM.I.player.Sprint();
        else
            GM.I.player.EndSprint();

        // Ctrl = Stealth
        if (Keyboard.current.ctrlKey.isPressed)
            GM.I.player.Stealth();
        else
            GM.I.player.Unstealth();

        // Spacebar = Dodge
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            GM.I.player.TryDodge();

        // E = Interact
        if (Keyboard.current.eKey.wasPressedThisFrame)
            GM.I.Interact();

        // Left Click = Attack
        if (Mouse.current.leftButton.wasPressedThisFrame)
            GM.I.player.BeginAttack();

        // Right Click = Move
        if (Mouse.current.rightButton.isPressed)
            GM.I.player.TryMoveToward(mouseWorld);


        // + Dragon Shrine

        // Right Click = Deselect
        if (DragonShrine.I.gameObject.activeSelf && Mouse.current.rightButton.wasPressedThisFrame)
            DragonShrine.I.Deselect();
    }

    // Hover a unit to show a tooltip for it.
    // Sets hoveredUnit.
    // Note: V similar to InputBattle. Possibly redundant and could be combined?
    public void HoverTooltip(Vector2 mouseWorld)
    {
        // Remember whether we're hovering anything.
        bool hoveringAnything = false;

        // + Tooltip
        // Look for a collider near our mouse.
        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorld);

        // Check if we're hovering anything.
        foreach (Collider2D hit in hits)
        {
            Debug.Log("Hovering: " + hit.name);

            // Check if we found a unit.
            hoveredUnit = hit.GetComponent<Unit>();
            if (hoveredUnit != null)
            {
                // Set bool.
                hoveringAnything = true;

                // Show tooltip.
                ExploreUI.I.ShowTooltip(hoveredUnit);
            }

            // Check if we found a region with a tooltip.
            Region hoveredRegion = hit.GetComponent<Region>();
            if (hoveredRegion != null && hoveredRegion.description != "")
            {
                // Set bool.
                hoveringAnything = true;

                // Show tooltip.
                ExploreUI.I.ShowTooltip(hoveredRegion);
            }
        }
        
        // If we're not hovering anything, reset.
        if (!hoveringAnything)
        {
            // Set hovered unit to null.
            hoveredUnit = null;
            
            // Hide the tooltip.
            ExploreUI.I.HideTooltip();
        }
    }
}
