using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Units, for battles.
// For explore mode, see Explorer.cs
public partial class Unit : MonoBehaviour
{
    [Header("Meta")]
    // This unit's name.
    public string myName;

    // This unit's mana cost.
    public int manaCost;

    // This unit's deploy time.
    public float deployTime;

    // This unit's card type.
    // Types:
    // - Unit
    // - Structure
    // - Spell
    // - Item
    public string cardType = "Unit";

    // This unit's role.
    public string role;


    [Header("Core Stats")]
    // The maximum amount of health this unit can have.
    public float maxHealth = 10f;

    // The current amount of health this unit has.
    public float currentHealth = 10f;

    // How much damage this unit does per attack.
    public float damage = 5f;

    // How much damage this unit negates per incoming attack.
    public float armor = 0f;

    // How fast this unit moves.
    // (In tiles per second?)
    public float speed = 3f;

    // How many tiles away this unit can attack.
    public float range = 1f;

    // How many tiles away this unit can see.
    public float vision = 3f;

    [Header("Keywords")]
    // Keywords
    public List<string> keywords = new List<string>();

    [Header("Production")]
    // For structures that produce other units periodically.

    // Which unit to spawn.
    public string producedUnit = "";

    // How many seconds to wait before spawning again.
    public float timePerSpawn = 6f;

    // The timer tracking time between spawns.
    public float spawnTimer = 0f;

    [Header("Machinery")]
    // Whether this unit is good or evil.
    // Also whether this unit is facing right or left, for movement, attacks, and animations.
    // (Unit animations are facing right by default, and mirrored for units facing left.)
    public bool good = true;

    // Which tile this unit is currently in.
    public Tile currentTile;

    // What lane this unit is in.
    // Assumed to be set on spawn.
    public int laneIndex = 0;

    // This unit's current state.
    // States:
    // =  0: Deploying
    // =  1: Moving
    // =  2: Attacking
    // =  3: Stunned
    // = -1: Dying
    public int state = 0;

    // How many more seconds this until has until it finishes deploying.
    public float deployTimer = 1f;

    // Show our full deployment, rather than hiding for the first half of it.
    public bool showFullDeployment = false;

    // Hurt damage flash timer.
    // How many seconds until this returns to normal color.
    public float hurtTimer = 0f;

    // This unit's current target.
    public Unit target;

    // This unit's vision circle.
    public SpriteMask visionCircle;

    // The Animator component for this unit's animations.
    public Animator animator;

    // The sprite renderer for this unit.
    public SpriteRenderer spriteRenderer;

    // The line renderer to display this unit's attacks.
    // (For lasers.)
    public LineRenderer attackLine;

    // + Initialization

    // Awaken!
    void Awake()
    {
        // Get animator.
        animator = GetComponent<Animator>();

        // Get sprite renderer.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set deploy timer.
        deployTimer = deployTime;

        // Set vision circle size.
        SetVision(vision);
        // Vision circles are children of the unit they're attached to so they scale with them, which has to be accounted for.
        // Also we scale by 2 for some reason?
        // float visionScale = 1 / transform.localScale.x;
        // visionScale *= vision * 2;
        // visionCircle.transform.localScale = new Vector3(visionScale, visionScale, visionScale);

        // Hide vision, until deployment finishes.
        visionCircle.gameObject.SetActive(false);

        // Set up attack line renderer.
        if (keywords.Contains("Laser"))
        {
            attackLine = gameObject.AddComponent<LineRenderer>();
            attackLine.positionCount = 2;
            attackLine.startWidth = 0.05f;
            attackLine.endWidth = 0.05f;
            attackLine.material = new Material(Shader.Find("Sprites/Default"));
            attackLine.startColor = Color.red;
            attackLine.endColor = Color.red;
            attackLine.sortingOrder = 1000;
            attackLine.enabled = false;
        }
    }

    // Start er up!
    void Start()
    {
        // Units, structures, and summons register with DM.
        if (cardType == "Unit" || cardType == "Structure" || role == "Summon")
        {
            if (good)
            {
                // Good units.
                DM.I.goodUnits[laneIndex].Add(this);
            }
            else
            {
                // Evil units.
                DM.I.evilUnits[laneIndex].Add(this);
            }
        }
    }

    // + Upkeep

    void FixedUpdate()
    {
        // Game over?
        if (DM.I.gameState != 1)
        {
            // Death!
            if (myName != "Dragon Statue")
                Death();

            // Return.
            return;
        }

        // Deploying.
        if (state == 0)
        {
            // Count down deploy timer.
            deployTimer -= Time.fixedDeltaTime;

            // Get percent deployed.
            float percentDeployed = 1f - (deployTimer / deployTime);

            // Set opacity, once halfway done.
            // (Some things always show, e.g. Violet Flowers).
            if (percentDeployed >= 0.5f || showFullDeployment)
                Utility.SetOpacity(spriteRenderer, percentDeployed);

            // Set health.
            currentHealth = maxHealth * percentDeployed;

            // Done?
            if (deployTimer <= 0f)
            {
                // Set state.
                state = 1;

                // Enable vision for good.
                if (good)
                    visionCircle.gameObject.SetActive(true);
            }

            // Return.
            return;
        }

        // Hover
        if (this == InputBattle.I.hoveredUnit)
            Utility.SetOpacity(spriteRenderer, 0.5f);
        else
            Utility.SetOpacity(spriteRenderer, 1f);

        // TBD: Add OnUpdate and similar functions to handle keywords.

        // Summon
        if (keywords.Contains("Summon"))
            LoseHealth(maxHealth * 0.05f * Time.fixedDeltaTime, this, true, false);

        // Poisoned
        if (keywords.Contains("Poisoned"))
            LoseHealth(maxHealth * 0.1f * Time.fixedDeltaTime, this, true);

        // Troll regeneration.
        else if (keywords.Contains("Troll"))
            GainHealth(maxHealth * 0.1f * Time.fixedDeltaTime);

        // Dying.
        if (state == -1)
            return;

        // Hurt?
        if (hurtTimer > 0)
        {
            // Decrement.
            hurtTimer -= Time.fixedDeltaTime;

            // Done?
            if (hurtTimer <= 0)
            {
                // Reset color.
                spriteRenderer.color = Color.white;

                // Cleanly set hurt timer to 0.
                hurtTimer = 0f;
            }
        }

        

        // Spells.
        if (cardType == "Spell")
        {
            state = 1;
            animator.SetInteger("State", state);
            return;
        }

        // Items.
        if (cardType == "Item")
        {
            // Look for someone to pick us up.
            Item();

            // Return.
            return;
        }

        // Stunned?
        if (state == 3)
        {
            // Roll to unstun.
            int d100 = Random.Range(1, 101);
            if (d100 == 100)
                Unstun();
            else
                return;
        }

        // Look for an enemy unit, and target them if we can.
        LookForEnemy();

        // If an enemy unit is found, attack.
        // Otherwise, move on.
        if (target != null)
            state = 2;
        else
            state = 1;

        // Animations.
        animator.SetInteger("State", state);

        // Structures.
        if (cardType == "Structure")
        {
            // Production?
            if (role == "Production")
            {
                // Decrement spawn timer.
                spawnTimer -= Time.fixedDeltaTime;

                // Check if it is time to spawn again.
                if (spawnTimer <= 0f)
                {
                    // Produce a unit!
                    // Produce();
                    GetLeader().SpawnUnit(producedUnit, currentTile);

                    // Reset spawn timer.
                    spawnTimer = timePerSpawn;
                }
            }
            // Return.
            return;
        }

        // - Movement
        if (state == 1)
        {
            // Water
            float waterMultiplier = 1f;
            if (laneIndex == 2)
            {
                // Aquatic creatures move faster in water!
                if (keywords.Contains("Aquatic"))
                    waterMultiplier = 2f;
                // Everyone else moves slower.
                else
                    waterMultiplier = 0.5f;
            }

            // Direction
            if (good)
                transform.position += Vector3.right * speed * waterMultiplier * Time.fixedDeltaTime;
            else
                transform.position -= Vector3.right * speed * waterMultiplier * Time.fixedDeltaTime;

            // Update current tile.
            int tileX = Mathf.Clamp(Mathf.FloorToInt(transform.position.x), 0, DM.I.gridWidth - 1);
            currentTile = DM.I.grid[tileX, laneIndex];
        }
    }

    // Return this unit's leader.
    public Leader GetLeader()
    {
        if (good)
            return DM.I.goodLeader;
        else
            return DM.I.evilLeader;
    }

    // + Stunned
    // Stun.
    public void Stun()
    {
        // Set state.
        state = 3;

        // Freeze animations.
        animator.speed = 0f;
    }

    // Unstun.
    public void Unstun()
    {
        // Set state.
        state = 1;

        // Resume animations.
        animator.speed = 1f;
    }

    // + Vision

    // Look for an enemy unit to attack.
    // Targets the enemy unit, if a valid one is found.
    public void LookForEnemy()
    {
        // Reset our target(?)
        target = null;

        // Get list of enemy units.
        List<Unit> enemyUnits = DM.I.GetEnemiesInLane(this);

        // Look through each enemy unit.
        foreach (Unit enemy in enemyUnits)
        {
            // Ignore deploying and dying units.
            if (enemy.state <= 0) continue;

            // Ignore spells and items.
            if (enemy.cardType == "Spell" || enemy.cardType == "Item") continue;

            // Ignore units behind us.
            if (good && transform.position.x > enemy.transform.position.x)
                continue;
            if (!good && transform.position.x < enemy.transform.position.x)
                continue;
            
            // Check if they are within our range.
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= range)
            {
                // If we have no target, set this enemy as our new target.
                if (target == null)
                {
                    target = enemy;
                } else {
                    // If this enemy is closer than our current target, set it as our new target.
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                    if (distance < distanceToTarget)
                        target = enemy;
                }
            }
        }
    }

    // Are we visible to the enemy?
    // Look through every enemy unit and compare its vision with its distance.
    public bool IsVisible()
    {
        // Get list of all enemy units.
        List<Unit> enemyUnits = DM.I.GetAllEnemies(this);

        // Iterate through each enemy.
        foreach(Unit enemy in enemyUnits)
        {
            // Get distance.
            float distance = Vector3.Distance(enemy.transform.position, transform.position);

            // Compare distance with enemy's vision.
            if (distance <= enemy.vision)
                return true;
        }

        // No enemy unit could see us, return false.
        return false;
    }

    // + Items

    // Look for a nearby unit to pick us up.
    // (of either side!)
    public void Item()
    {
        // + Look for a nearby unit.
        // Get all colliders within 0.5f of our position.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        // Look through each collider.
        foreach (Collider2D hit in hits)
        {
            // Get the unit attached to the collider.
            Unit unit = hit.GetComponent<Unit>();

            // Ignore non-units.
            if (unit == null) continue;

            // Ignore deploying and dead units.
            if (unit.state < 1) continue;

            // Ignore items, structures, and spells.
            if (unit.cardType != "Unit") continue;

            // We got someone to pick us up!
            unit.PickUp(this);
        }
    }

    // Pick up an item.
    public void PickUp(Unit item)
    {
        // + Treasure?
        if (item.role == "Treasure")
            GetLeader().mana += item.manaCost;
        else
            // Get a bit bigger.
            transform.localScale *= 1.1f;

        // + Gain stats.

        // Meta
        manaCost += item.manaCost;
        deployTimer += item.deployTime;

        // Health.
        maxHealth += item.maxHealth;
        GainHealth(item.currentHealth);

        // Damage.
        damage += item.damage;

        // Armor.
        armor += item.armor;

        // Vision.
        if (item.vision > 0f)
            SetVision(vision + item.vision);

        // Range.
        range += item.range;

        // Speed.
        speed += item.speed;

        // Keywords.
        foreach(string keyword in item.keywords)
        {
            keywords.Add(keyword);
        }

        // Clean up item object.
        item.BeginDying();
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

    // - Combat
    // Attack our target!
    public void Attack()
    {
        // Fail if we have no target.
        if (target == null) return;

        // Handle keywords and other attack triggers.
        OnAttack();

        // Deal damage to our target.
        target.LoseHealth(damage, this);
    }

    // On attack triggers.
    public void OnAttack()
    {
        // Bloodthirst?
        if (keywords.Contains("Bloodthirst"))
        {
            damage *= 1.01f;
        }

        // Cleave?
        if (keywords.Contains("Cleave"))
        {
            // Iterate through each other enemy unit nearby.
            foreach(Unit enemy in GetEnemiesNear(target))
            {
                // Deal half damage.
                enemy.LoseHealth(damage / 2f, this);
            }
        }

        // Laser?
        if (keywords.Contains("Laser"))
        {
            // Draw a red line to our target.
            attackLine.SetPosition(0, transform.position);
            attackLine.SetPosition(1, target.transform.position);
            attackLine.enabled = true;

            // Fade the laser after a brief delay.
            StartCoroutine(FadeLaser());
        }
    }

    IEnumerator FadeLaser()
    {
        yield return new WaitForSeconds(0.1f);
        attackLine.enabled = false;
    }

    // Get a list of all enemy units near the target.
    // Used for cleave and similar effects.
    // Note: Does NOT include the target!
    public List<Unit> GetEnemiesNear(Unit target, float radius = 0.5f)
    {
        // Initialize a new list of units.
        List<Unit> nearbyEnemies = new List<Unit>();

        // Get all colliders within radius of the target.
        Collider2D[] hits = Physics2D.OverlapCircleAll(target.transform.position, radius);

        // Look through each collider.
        foreach (Collider2D hit in hits)
        {
            // Get the unit attached to the collider.
            Unit unit = hit.GetComponent<Unit>();

            // Ignore non-units.
            if (unit == null) continue;

            // Ignore allies.
            if (unit.good == good) continue;

            // Ignore the target.
            if (unit == target) continue;

            // Ignore deploying and dead units.
            if (unit.state < 1) continue;

            // Add to list.
            nearbyEnemies.Add(unit);
        }

        // Return list.
        return nearbyEnemies;
    }

    // Fully heal.
    public void FullHeal()
    {
        currentHealth = maxHealth;
    }

    // Gain health.
    public void GainHealth(float healthGained, Unit source = null)
    {
        // Gain health.
        currentHealth += healthGained;

        // Cap at max.
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    // Lose health.
    public void LoseHealth(float healthLost, Unit source = null, bool ignoreArmor = false, bool damageFlash = true)
    {
        // Treasure can't be killed.
        if (role == "Treasure") return;

        // Flash red when hurt.
        if (damageFlash)
        {
            spriteRenderer.color = Color.red;
            hurtTimer = 0.1f;
        }
            
        // Armor
        if (!ignoreArmor)
        {
            healthLost -= armor;
            if (healthLost < 1)
                healthLost = 1;
        }

        // Other modifiers.
        healthLost *= DamageReceivedModifiers();

        // + TBD: Move keywords elsewhere?

        // Getting stunned?
        if (source != null && source.keywords.Contains("Stuns"))
            Stun();

        // Vital?
        if (keywords.Contains("Vital"))
        {
            // Leader loses health.
            if (good)
                DM.I.goodLeader.LoseHealth(healthLost, source);
            else
                DM.I.evilLeader.LoseHealth(healthLost, source);

            // Return.
            return;
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

            // Our killer moves on.
            if (source != null && source.target == this)
                source.target = null;
        }
    }

    // Begin dying.
    public void BeginDying()
    {
        // Unstun, if we were stunned.
        if (state == 3)
            Unstun();
        
        // Charming
        if (target != null && target.keywords.Contains("Charming"))
        {
            // Charm.
            ChangeSides();

            // Return.
            return;
        }

        // Set state.
        state = -1;
        animator.SetInteger("State", state);
    }


    // Death.
    public void Death()
    {
        // On death triggers.
        OnDeath();

        // Remove from DM's list of units.
        if (good)
            DM.I.goodUnits[laneIndex].Remove(this);
        else
            DM.I.evilUnits[laneIndex].Remove(this);

        // If an item, remove from leader's list of items.
        if (cardType == "Item")
            GetLeader().items.Remove(this);
            
        // Clean up game object.
        Destroy(gameObject);
    }

    // Called when this unit dies.
    // Handles on death triggers.
    public void OnDeath()
    {
        // Prevent the lich from surviving through battles.
        if (DM.I.gameState != 1) return;
        
        // Skeleton
        if (keywords.Contains("Skeleton"))
        {
            // Spawn a skull.
            Unit newUnit = GetLeader().SpawnUnit("Skull", currentTile);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }

        // Big Skeleton
        if (keywords.Contains("Big Skeleton"))
        {
            // Spawn a skull.
            Unit newUnit = GetLeader().SpawnUnit("Skeleton Warrior", currentTile);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }

        // Lich
        if (keywords.Contains("Lich"))
        {
            // The lich respawns!
            Unit newUnit = GetLeader().SpawnUnit("Lich", currentTile);

            // Show its full deployment.
            newUnit.showFullDeployment = true;
        }
    }

    // + Magic
    // Cast a spell.
    // Called from a spell's cast animation.
    public void CastSpell()
    {
        // Avoid repeat casting(?).
        if (cardType != "Spell") return;

        // Cast this spell.
        Spell.Cast(this);
    }

    // Change sides.
    public void ChangeSides(bool fullHeal = true)
    {
        // Full heal!
        if (fullHeal)
            FullHeal();

        // Good to evil?
        if (good)
        {
            // Set bool.
            good = false;

            // Set rotation.
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

            // Hide vision.
            visionCircle.gameObject.SetActive(false);

            // Remove from DM's list of good units.
            DM.I.goodUnits[laneIndex].Remove(this);

            // Add to DM's list of evil units.
            DM.I.evilUnits[laneIndex].Add(this);
        } else {
            // Evil to good?

            // Set bool.
            good = true;

            // Set rotation.
            transform.eulerAngles = new Vector3(0f, 0f, 0f);

            // Reveal vision.
            visionCircle.gameObject.SetActive(true);

            // Remove from DM's list of evil units.
            DM.I.evilUnits[laneIndex].Remove(this);

            // Add to DM's list of good units.
            DM.I.goodUnits[laneIndex].Add(this);
        }
    }
}
