using UnityEngine;

public partial class Explorer : MonoBehaviour
{
    [Header("Meta")]
    // What's our name?
    public string myName;

    // How many credits does this explorer cost to recruit?
    public int creditCost = -1;

    // This explorer's description.
    [TextArea(10, 30)]
    public string description;
    
    // This explorer's state.
    // States:
    // -1 = Dying
    //  0 = Idle
    //  1 = Moving
    //  2 = Attacking
    //  3 = Stunned
    //  4 = Dodging
    public int state = 0;

    [Header("Stats")]
    // How fast this explorer moves.
    public float speed = 2f;

    // How much faster this explorer moves while sprinting.
    public float sprintMultiplier = 2f;

    // How far this explorer can see.
    public float vision = 3f;

    [Header("States")]
    // Are we jumping?
    // public bool isJumping = false;

    // Are we dodging?
    public bool isDodging = false;

    // Are we rolling?
    // (Subset of above isDodging to distinguish between rolling and spot dodging)
    public bool isRolling = false;

    // Which direction we last moved in.
    Vector2 lastMoveDirection = Vector2.right;

    [Header("Timers")]
    // How much longer our current jump will last.
    // public float jumpTimer = 0f;

    // How much longer our current dodge will last.
    public float dodgeTimer = 0f;

    [Header("Inputs")]
    // Are we trying to move up?
    public bool isPressingUp;

    // Are we trying to move down?
    public bool isPressingDown;

    // Are we trying to move left?
    public bool isPressingLeft;

    // Are we trying to move right?
    public bool isPressingRight;

    [Header("Jump")]
    // How long a jump lasts.
    // public float jumpDuration = 1f;

    // // How much force we jump with.
    // public float jumpForce = 2f;

    [Header("Dodge")]
    // How long we dodge.
    public float dodgeDuration = 0.5f;

    // How fast we roll.
    public float rollSpeed = 5f;


    [Header("Machinery")]
    // This explorer's rigid body, for collisions.
    public Rigidbody2D rb;

    // The Animator component for this explorer's animations.
    public Animator animator;

    // + Exploring!

    // Fixed update.
    void FixedUpdate()
    {
        // +++ Movement
        Vector2 movement = Vector2.zero;

        // + Jumping
        // if (isJumping)
        // {
        //     // Decrement jump timer.
        //     jumpTimer -= Time.fixedDeltaTime;

        //     // Get progress.
        //     float progress = 1f - (jumpTimer / jumpDuration);

        //     // Rising?
        //     if (progress < 0.5f)
        //     {
        //         // Move up a bit!
        //         // rb.MovePosition(rb.position + Vector2.up * jumpForce * Time.fixedDeltaTime);
        //         movement += Vector2.up * jumpForce;
        //     } else {
        //         // Fall down.
        //         // rb.MovePosition(rb.position + Vector2.down * jumpForce * Time.fixedDeltaTime);
        //         movement += Vector2.down * jumpForce;
        //     }

        //     // Done?
        //     if (jumpTimer <= 0f)
        //         isJumping = false;
        // }

        // + Dodging
        if (isDodging)
        {
            // Decrement dodge timer.
            dodgeTimer -= Time.fixedDeltaTime;

            // Get progress so we can spin a full 360 over the course of the roll.
            float progress = 1f - (dodgeTimer / dodgeDuration);

            // Rolling?
            if (isRolling)
            {
                // Move.
                rb.MovePosition(rb.position + lastMoveDirection * rollSpeed * Time.fixedDeltaTime);

                // Rotate
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, -progress * 360f);
            } else {
                // Otherwise we're spot dodging i.e. dodging in place!

                // Rotate
                transform.eulerAngles = new Vector3(0f, progress * 360f, transform.eulerAngles.z);
            }

            // Done?
            if (dodgeTimer <= 0f)
            {
                isDodging = false;
                isRolling = false;
            }

            // Return to prevent normal movement.
            return;
        }

        // + WASD walking
        Vector2 direction = Vector2.zero;

        if (isPressingUp)
            direction.y += 1f;
        if (isPressingDown)
            direction.y -= 1f;
        if (isPressingLeft)
            direction.x -= 1f;
        if (isPressingRight)
            direction.x += 1f;

        // Face our direction of movement.
        if (direction.x < 0)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        if (direction.x > 0)
            transform.eulerAngles = new Vector3(0f, 0f, 0f);

        // No movement.
        if (direction == Vector2.zero)
        {
            // Set state to idle.
            state = 0;
        }
        else
        {
            // Set state to moving.
            state = 1;

            // Normalize direction of movement.
            direction.Normalize();

            // Remember which direction we're moving.
            lastMoveDirection = direction;

            // Calculate speed.
            float currentSpeed = speed * SpeedModifiers();

            // Move!
            // rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime);
            movement += direction * currentSpeed;
        }

        // Move!
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        // Animations.
        animator.SetInteger("State", state);
    }

    // +++ Movement

    // + Sprint
    public void Sprint()
    {
        // Add speed modifier.
        speedModifiers["Sprint"] = sprintMultiplier;

        // Animate faster(?)
        animator.speed = sprintMultiplier;
    }

    public void EndSprint()
    {
        // Remove speed modifier.
        speedModifiers.Remove("Sprint");

        // Reset animation speed.
        animator.speed = 1f;
    }

    // + Jump
    // Try to jump.
    // public void TryJump()
    // {
    //     // Prevent double jumps.
    //     // TBD: Add double jumps somehow...
    //     if (isJumping) return;

    //     // Prevent jumping while dying.
    //     if (state == -1) return;

    //     // Set bool.
    //     isJumping = true;

    //     // Set timer.
    //     jumpTimer = jumpDuration;
    // }

    // + Dodge
    // Try to dodge.
    public void TryDodge()
    {
        // Don't double dodge.
        if (isDodging) return;

        // You can't dodge death!
        if (state == -1) return;

        // Set bool.
        isDodging = true;

        // Roll if we're moving, spot dodge if we're idle or attacking.
        if (state == 1)
            isRolling = true;

        // Set timer.
        dodgeTimer = dodgeDuration;

        // Set state.
        state = 4;

        // Animate.
        animator.SetInteger("State", state);
    }
}
