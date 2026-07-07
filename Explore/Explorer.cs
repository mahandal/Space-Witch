using UnityEngine;
using System.Collections.Generic;

public partial class Explorer : MonoBehaviour
{
    [Header("Meta")]
    // Is this explorer good? Or evil?
    public bool good = true;

    // What's our name?
    public string myName;

    // How many credits does this explorer cost to recruit?
    public int creditCost = -1;

    // This explorer's description.
    [TextArea(10, 30)]
    public string description;

    [Header("AI")]
    // This explorer's state.
    // States:
    // -1 = Dying
    //  0 = Idle
    //  1 = Moving
    //  2 = Attacking
    //  3 = Stunned
    //  4 = Dodging
    public int state = 0;

    // Our current target, while in combat.
    public Explorer target;

    [Header("Squad")]
    // Our squad, if we are a squad leader.
    public List<Explorer> squad;

    // Our squad leader, if we are in a squad.
    public Explorer squadLeader;

    [Header("Core Stats")]
    // The maximum amount of health this explorer can have.
    public float maxHealth = 10f;

    // The current amount of health this explorer has.
    public float currentHealth = 10f;

    // How much damage this explorer does per attack.
    public float damage = 5f;

    // How many seconds it takes for this explorer to attack.
    public float attackTime = -1f;

    // How much damage this explorer negates per incoming attack.
    public float armor = 0f;

    // How fast this explorer moves.
    // (In tiles per second?)
    public float speed = 3f;

    // How many tiles away this explorer can attack.
    public float range = 1f;

    // How many tiles away this explorer can see.
    public float vision = 3f;


    [Header("Explorer Specific Stats")]
    // How much faster this explorer moves while sprinting.
    public float sprintMultiplier = 2f;

    // How much slower this explorer moves while stealthing.
    public float stealthMultiplier = 0.5f;

    [Header("States")]
    // Are we jumping?
    // public bool isJumping = false;

    // Are we sprinting?
    public bool isSprinting = false;

    // Are we stealthing?
    public bool isStealthing = false;

    // Are we dodging?
    public bool isDodging = false;

    // Are we rolling?
    // (Subset of above isDodging to distinguish between rolling and spot dodging)
    public bool isRolling = false;

    // Which direction we last moved in.
    Vector2 lastMoveDirection = Vector2.right;

    [Header("Timers")]
    // How much longer our current dodge will last.
    public float dodgeTimer = 0f;

    // How much longer our current red damage flash will last.
    public float hurtTimer = 0f;

    [Header("Inputs")]
    // Are we trying to move up?
    public bool isPressingUp;

    // Are we trying to move down?
    public bool isPressingDown;

    // Are we trying to move left?
    public bool isPressingLeft;

    // Are we trying to move right?
    public bool isPressingRight;

    [Header("Dodge")]
    // How long we dodge.
    public float dodgeDuration = 0.5f;

    // How fast we roll.
    public float rollSpeed = 3f;


    [Header("Machinery")]
    // This explorer's rigid body, for collisions.
    public Rigidbody2D rb;

    // The Animator component for this explorer's animations.
    public Animator animator;

    // The sprite renderer displaying this explorer's primary visuals.
    public SpriteRenderer spriteRenderer;

    // This explorer's vision circle.
    public SpriteMask visionCircle;

    // This explorer's attack range circle.
    public SpriteRenderer attackCircle;

    // +++ Initialization
    void Start()
    {
        // Randomize animation speed(?)
        animator.speed = Random.Range(0.5f, 1f);

        // Load this explorer's stats from its progenitor unit.
        LoadUnit(myName);
    }

    // Load the given unit's stats into this explorer.
    // Note: Does not change the animator, so not usable to shapeshift.
    public void LoadUnit(string unitName)
    {
        // Get the progenitor unit.
        Unit p = Progenitors.I.GetProgenitor(myName);

        // Null check.
        if (p == null) return;

        // Set stats.
        maxHealth = p.maxHealth;
        currentHealth = p.currentHealth;
        armor = p.armor;
        // vision = p.vision;
        SetVision(p.vision);
        speed = p.speed;
        damage = p.damage;
        attackTime = p.attackTime;
        // range = p.range;
        SetRange(p.range);
    }

    // + Stats
    // Set our vision stat and update our vision circle size accordingly.
    public void SetVision(float newVision)
    {
        // Update vision.
        vision = newVision;

        // Set vision circle size.
        // Vision circles are children of the unit they're attached to so they scale with them, which has to be accounted for.
        // Also we scale by 2 for some reason?
        float visionScale = 1 / transform.localScale.x;
        visionScale *= vision * 2;
        visionCircle.transform.localScale = new Vector3(visionScale, visionScale, visionScale);
    }

    // Set our range stat and update our attack range circle size accordingly.
    public void SetRange(float newRange)
    {
        // Update range.
        range = newRange;

        // Set attack range circle size.
        float attackScale = 1 / transform.localScale.x;
        attackScale *= range * 2;
        attackCircle.transform.localScale = new Vector3(attackScale, attackScale, attackScale);
    }

    // +++ Exploring!

    // Fixed update.
    void FixedUpdate()
    {
        // Dying?
        if (state == -1) return;

        // Hurt?
        if (hurtTimer > 0)
        {
            // Decrement.
            hurtTimer -= Time.fixedDeltaTime;

            // Use hurt timer to fade redness.
            // Note: Hurt timer is set to 1 second right now. This may need to adjust if that changes!
            spriteRenderer.color = new Color(1f, 1f - hurtTimer, 1f - hurtTimer, 1f);

            // Done?
            if (hurtTimer <= 0)
            {
                // Reset color.
                spriteRenderer.color = Color.white;

                // Cleanly set hurt timer to 0.
                hurtTimer = 0f;
            }
        } else {
            // + Set target.
            // Look for nearest enemy.
            target = NearestEnemy();
        }

        // +++ AI
        if (this != GM.I.player)
            AI();


        // +++ Attacking(?)
        if (state == 2) return;


        // +++ Movement
        Vector2 movement = Vector2.zero;

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
                // Use speed modifiers.
                float modifiedRollSpeed = rollSpeed * SpeedModifiers();

                // Move.
                rb.MovePosition(rb.position + lastMoveDirection * modifiedRollSpeed * Time.fixedDeltaTime);

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

    // + Squad
    public void AddToSquad(Explorer newSquadMember)
    {
        // Add to squad.
        squad.Add(newSquadMember);

        // Set squad leader.
        newSquadMember.squadLeader = this;

        // Set alignment(?)
        newSquadMember.good = good;
    }

    // + AI
    // Decide what to do.
    // If we see an enemy, move toward them and attack.
    // (Actual movement is handled elsewhere already, so just input direction)
    public void AI()
    {
        // Reset directional input.
        isPressingLeft = false;
        isPressingRight = false;
        isPressingUp = false;
        isPressingDown = false;

        // If attacking, follow through.
        if (state == 2) return;

        // Look for enemies.
        // target = LookForEnemy();

        // Do we have a target?
        if (target == null)
        {
            // No target

            // Do we have a squad leader to follow?
            if (squadLeader != null)
            {
                // Are we far enough from our squad leader to move toward them?
                float distance = Vector3.Distance(transform.position, squadLeader.transform.position);
                if (distance > vision)
                    TryMoveToward(squadLeader);
            }
        }
        else
        {
            // + Attack if we're in range.
            // Check distance.
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance <= range)
            {
                // Begin attacking.
                BeginAttack();
            } else {
                // + Move toward them
                // Set state to moving.
                state = 1;

                TryMoveToward(target);

                // Get direction to move.

                // // Is the target to our left?
                // // if (target.transform.position.x < transform.position.x)
                // if (target.transform.position.x - transform.position.x < -0.1f)
                //     isPressingLeft = true;
                // // else if (target.transform.position.x > transform.position.x)
                // else if (target.transform.position.x - transform.position.x > 0.1f)
                //     isPressingRight = true;

                // // Is the target below us?
                // // if (target.transform.position.y < transform.position.y)
                // if (target.transform.position.y - transform.position.y < -0.1f)
                //     isPressingDown = true;
                // // else if (target.transform.position.y > transform.position.y)
                // else if (target.transform.position.y - transform.position.y > 0.1f)
                //     isPressingUp = true;
            }
        }
    }

    // Set our movement toward a given explorer.
    public void TryMoveToward(Explorer target)
    {
        // Reset directional movement.
        isPressingLeft = false;
        isPressingRight = false;
        isPressingUp = false;
        isPressingDown = false;

        // Is the target to our left?
        // (with a bit of leeway)
        // if (target.transform.position.x < transform.position.x)
        if (target.transform.position.x - transform.position.x < -0.1f)
            isPressingLeft = true;
        // else if (target.transform.position.x > transform.position.x)
        else if (target.transform.position.x - transform.position.x > 0.1f)
            isPressingRight = true;

        // Is the target below us?
        // if (target.transform.position.y < transform.position.y)
        if (target.transform.position.y - transform.position.y < -0.1f)
            isPressingDown = true;
        // else if (target.transform.position.y > transform.position.y)
        else if (target.transform.position.y - transform.position.y > 0.1f)
            isPressingUp = true;
    }

    // Look for an enemy.
    // Return the nearest enemy within our vision range, or null if there is none.
    public Explorer NearestEnemy()
    {
        // Get nearby colliders.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, vision);

        // Remember nearest enemy.
        Explorer nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        // Look through each collider.
        foreach (Collider2D col in colliders)
        {
            // Check if the collider is attached to an explorer.
            Explorer e = col.GetComponent<Explorer>();

            // Ignore non-explorers.
            if (e == null) continue;

            // Ignore allies.
            if (e.good == good) continue;

            // Ignore dying enemies.
            if (e.state == -1) continue;

            // Get distance.
            float distance = Vector3.Distance(e.transform.position, transform.position);

            // Compare distance.
            if (distance < nearestDistance)
            {
                // New nearest.
                nearestDistance = distance;
                nearestEnemy = e;
            }
        }

        // Return.
        return nearestEnemy;
    }

    // Look for nearest explorer.
    // Return the nearest other explorer within our vision range, or null if there is none.
    public Explorer NearestExplorer()
    {
        // Get nearby colliders.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, vision);

        // Remember nearest explorer.
        Explorer nearestExplorer = null;
        float nearestDistance = float.MaxValue;

        // Look through each collider.
        foreach (Collider2D col in colliders)
        {
            // Check if the collider is attached to an explorer.
            Explorer e = col.GetComponent<Explorer>();

            // Ignore non-explorers.
            if (e == null) continue;

            // Ignore self.
            if (e == this) continue;

            // Ignore dying explorers.
            if (e.state == -1) continue;

            // Get distance.
            float distance = Vector3.Distance(e.transform.position, transform.position);

            // Compare distance.
            if (distance < nearestDistance)
            {
                // New nearest.
                nearestDistance = distance;
                nearestExplorer = e;
            }
        }

        // Return.
        return nearestExplorer;
    }

    // + Combat
    // Begin attacking.
    public void BeginAttack()
    {
        // Set state.
        state = 2;
        animator.SetInteger("State", state);
    }

    // Attack!
    public void Attack()
    {
        // Fail if dying.
        if (state == -1) return;
        
        // Return to idle, if only for a moment.
        state = 0;
        animator.SetInteger("State", state);

        // Abort if no target for w/e reason.
        if (target == null)
        {
            return;
        }

        // Check if target is in range.
        float distance = Vector3.Distance(transform.position, target.transform.position);

        // Only deal damage if target is still in range.
        if (distance <= range)
        {
            // Target loses health.
            target.LoseHealth(damage, this);
        }
    }

    // Lose health.
    public void LoseHealth(float healthLost, Explorer source = null, bool ignoreArmor = false, bool damageFlash = true)
    {
        // Can't touch this!
        if (isDodging) return;
        
        // Flash red when hurt.
        if (damageFlash)
        {
            // Flash red.
            spriteRenderer.color = Color.red;

            // Set timer to reset color.
            hurtTimer = 1f;

            // Target enemy!
            if (source != null)
                target = source;
        }

        // Armor
        if (!ignoreArmor)
        {
            // Reduce incoming damage by armor.
            healthLost -= armor;

            // Minimum of 1.
            if (healthLost < 1)
                healthLost = 1;
        }

        // Lose health.
        currentHealth -= healthLost;

        // Death.
        if (currentHealth <= 0)
        {
            // Set our killer as our target, for vengeance death effects.
            target = source;

            // Begin dying.
            BeginDying();
        }
    }

    // Begin dying.
    public void BeginDying(Explorer killer = null)
    {
        // Set state.
        state = -1;
        animator.SetInteger("State", state);
    }

    // Death.
    public void Death()
    {
        // For now, just destroy the game object.
        Destroy(gameObject);

        // TBD: Leave corpses on the ground, so they can be picked up and resurrected at the Dragon Shrine.
    }

    // + Movement

    // Sprint
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

    // Stealth
    public void Stealth()
    {
        // Set bool.
        isStealthing = true;

        // Add speed modifier.
        speedModifiers["Stealth"] = stealthMultiplier;

        // Animate slower(?)
        animator.speed = stealthMultiplier;
    }

    public void Unstealth()
    {
        // Set bool.
        isStealthing = false;

        // Remove speed modifier.
        speedModifiers.Remove("Stealth");

        // Reset animation speed.
        animator.speed = 1f;
    }

    // Dodge
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
