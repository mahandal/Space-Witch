using UnityEngine;
using UnityEngine.InputSystem;

public class InputExplore : MonoBehaviour
{
    // Singleton.
    public static InputExplore I;

    // + Initialization
    void Awake()
    {
        // Singleton.
        if (I == null)
            I = this;
        else
            Destroy(this);
    }

    // + Exploring
    void Update()
    {
        // Stunned?
        if (GM.I.player.state == 3) return;

        // + Get mouse position in world coordinates.
        // Convert our mouse position to 3d coordinates.
        // Vector3 mouseScreen = Mouse.current.position.ReadValue();

        // Set the mouse's z to opposite the camera's, for some reason?
        // mouseScreen.z = -Camera.main.transform.position.z;

        // // Convert our mouse's 3d coordinates to 2d coordinates.
        // Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        // Set target.
        // GM.I.player.target = GM.I.player.NearestVisibleExplorer(mouseScreen);
        
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
    }
}
