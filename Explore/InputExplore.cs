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

        // Spacebar = Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            GM.I.player.TryJump();

        // Right Click = Dodge
        if (Mouse.current.rightButton.wasPressedThisFrame)
            GM.I.player.TryDodge();
    }
}
