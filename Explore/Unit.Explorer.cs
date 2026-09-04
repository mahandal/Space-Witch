using UnityEngine;
using System.Collections.Generic;

public partial class Unit
{
    [Header("Explore Mode")]
    // How likely is this explorer to spawn?
    public float spawnRate = 0.5f;

    // How many credits does this explorer cost to recruit?
    public int creditCost = -1;

    // This explorer's description.
    [TextArea(10, 30)]
    public string description;

    [Header("Squad")]
    // Our squad, if we are a squad leader.
    public List<Unit> squad;

    // Our squad leader, if we are in a squad.
    public Unit squadLeader;

    // [Header("Explorer Specific Stats")]
    // // How much faster this explorer moves while sprinting.
    // public float sprintMultiplier = 2f;

    // // How much slower this explorer moves while stealthing.
    // public float stealthMultiplier = 0.5f;

    [Header("States")]
    // Are we currently going toward a specific destination?
    public bool hasDestination = false;

    // Our current destination, if we are going somewhere.
    public Vector3 destination;

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

    // This explorer's collider.
    public BoxCollider2D collider;

    // +++ Initialization
    void InitializeExplorer()
    {
        // Roll whether this unit should despawn.
        float spawnRoll = Random.Range(0f, 1f);
        if (spawnRoll > spawnRate)
        {
            // Deactivate game object.
            gameObject.SetActive(false);

            // Avoid other setup(?)
            return;
        }
        // Initialize this unit, connecting its rigid body and whatnot.
        Initialize();

        // Load this explorer's stats from its progenitor unit.
        LoadUnit(myName);

        // Enable vision circle(?)
        visionCircle.gameObject.SetActive(true);

        // Randomize animation speed(?)
        animator.speed = Random.Range(0.5f, 1f);
    }

    // Load the given unit's stats into this explorer.
    public void LoadUnit(string unitName)
    {
        // Get the progenitor unit.
        Unit p = Progenitors.I.GetProgenitor(unitName);

        // Null check.
        if (p == null) return;

        // Set meta data.
        myName = unitName;
        // description = p.description;
        manaCost = p.manaCost;
        deployTime = p.deployTime;
        cardType = p.cardType;
        role = p.role;

        // Set stats.
        maxHealth = p.maxHealth;
        currentHealth = p.currentHealth;
        armor = p.armor;
        SetVision(p.vision);
        speed = p.speed;
        damage = p.damage;
        attackTime = p.attackTime;
        SetRange(p.range);
        keywords = p.keywords;

        // Set animator.
        animator.runtimeAnimatorController = p.animator.runtimeAnimatorController;

        // Get attack time, for units.
        if (cardType == "Unit")
            attackTime = CalculateAttackTime();

        // Set collider size.
        collider.offset = p.collider.offset;
        collider.size = p.collider.size;
    }

    // +++ Exploring!

    // Fixed update.
    void ExploreUpdate()
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
            Explore();


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

        // Do we have a destination?
        if (hasDestination)
        {
            // Override destination if we input other movement.
            if (direction != Vector2.zero)
            {
                hasDestination = false;
            } else {
                // Go toward destination.
                direction = TryMoveToward(destination);
            }
        }

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
    public void AddToSquad(Unit newSquadMember)
    {
        // Add to squad.
        squad.Add(newSquadMember);

        // Set squad leader.
        newSquadMember.squadLeader = this;

        // Set alignment(?)
        newSquadMember.good = good;

        // Reset targeting(?)
        target = null;
        newSquadMember.target = null;

        // If this is for the player's squad, also add to exploring list.
        if (this == GM.I.player)
            GM.I.exploring.Add(newSquadMember.myName);
    }

    // + Explore AI
    // Decide what to do.
    // If we see an enemy, move toward them and attack.
    // (Actual movement is handled elsewhere already, so just input direction)
    public void Explore()
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
                    TryMoveToward(squadLeader.transform.position);
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

                TryMoveToward(target.transform.position);
            }
        }
    }

    // Set our movement toward a given position.
    // Returns the direction of movement.
    public Vector2 TryMoveToward(Vector3 newDestination)
    {
        // Set destination.
        destination = newDestination;

        // Mark that we're using a destination.
        hasDestination = true;

        // Reset directional movement.
        isPressingLeft = false;
        isPressingRight = false;
        isPressingUp = false;
        isPressingDown = false;

        // Is the destination to our left?
        // (with a bit of leeway)
        if (newDestination.x - transform.position.x < -0.1f)
            isPressingLeft = true;
        else if (newDestination.x - transform.position.x > 0.1f)
            isPressingRight = true;

        // Is the destination below us?
        if (newDestination.y - transform.position.y < -0.1f)
            isPressingDown = true;
        else if (newDestination.y - transform.position.y > 0.1f)
            isPressingUp = true;

        // Close enough?
        if (Vector3.Distance(newDestination, transform.position) < 0.1f)
        {
            // Done!
            hasDestination = false;
            return Vector2.zero;
        }

        // Return.
        // A lil redundant with how it's handled above in FixedUpdate...
        // I dunno how to do it better rn though! Fix it up when you know what to do!
        Vector2 direction = Vector2.zero;

        if (isPressingUp)
            direction.y += 1f;
        if (isPressingDown)
            direction.y -= 1f;
        if (isPressingLeft)
            direction.x -= 1f;
        if (isPressingRight)
            direction.x += 1f;

        return direction;
    }

    // Look for an enemy.
    // Return the nearest enemy within our vision range, or null if there is none.
    public Unit NearestEnemy()
    {
        // Get nearby colliders.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, vision);

        // Remember nearest enemy.
        Unit nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        // Look through each collider.
        foreach (Collider2D col in colliders)
        {
            // Check if the collider is attached to a unit.
            Unit e = col.GetComponent<Unit>();

            // Ignore non-units.
            if (e == null) continue;

            // Ignore allies.
            if (e.good == good) continue;

            // Ignore dying enemies.
            if (e.state == -1) continue;

            // Ignore deploying enemies.
            if (e.deployTimer > 0f) continue;

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

    // Look for the nearest other unit.
    // Return the nearest other explorer within our vision range, or null if there is none.
    public Unit NearestUnit()
    {
        // Get nearby colliders.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, vision);

        // Remember nearest unit.
        Unit nearestUnit = null;
        float nearestDistance = float.MaxValue;

        // Look through each collider.
        foreach (Collider2D col in colliders)
        {
            // Check if the collider is attached to a unit.
            Unit unit = col.GetComponent<Unit>();

            // Ignore non-units.
            if (unit == null) continue;

            // Ignore self.
            if (unit == this) continue;

            // Ignore dying units.
            if (unit.state == -1) continue;

            // Get distance.
            float distance = Vector3.Distance(unit.transform.position, transform.position);

            // Compare distance.
            if (distance < nearestDistance)
            {
                // New nearest.
                nearestDistance = distance;
                nearestUnit = unit;
            }
        }

        // Return.
        return nearestUnit;
    }

    // + Combat
    // Begin attacking.
    public void BeginAttack()
    {
        // Set state.
        state = 2;
        animator.SetInteger("State", state);
    }



    // + Movement

    // Sprint
    public void Sprint()
    {
        float sprintMultiplier = 2f;

        if (myName == "Roaming Warrior")
            sprintMultiplier *= 2f;

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

        float stealthMultiplier = 0.5f;

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
