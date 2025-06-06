using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Player : Gatherer
{
    [Header("Stats")]
    //public float moveSpeed = 5f;
    public float rotationSpeed = 1f;
    public float goManaCost = 1f;
    public float sprintManaCost = 1f;

    [Header("Progression")]
    public int level = 1;
    public float xp = 0;

    // Dictionary mapping our current talent levels
    public Dictionary<string, int> talents = new Dictionary<string, int>();

    // Our current spell
    public string currentSpell = "Jump";

    // A list of our unlocked spells
    public List<string> knownSpells = new List<string>();

    // How many talents we have purchased with our soul.
    public int talentsBought = 0;

    [Header("Manual Machinery")]
    // Camera
    public Camera mainCam;

    // Familiar
    public Familiar familiar;

    [Header("Automated Machinery")]
    // Rigid body
    //public Rigidbody2D rb2d;

    [Header("Meditation")]
    public float meditationFadeTime = 1f;
    public float meditationTimer = 0f;
    public bool isPreparingToMeditate = false;

    [Header("Timers")]
    //public float spellCooldown = 0f;
    public Dictionary<string, float> spellCooldowns = new Dictionary<string, float>();

    [Header("States")]
    public int livesRemaining = 9;
    public bool isShaking = false;
    //public bool isCalm = true;

    [Header("Input")]

    // Input

    // WASD movement
    public bool isMovingUp;
    public bool isMovingDown;
    public bool isMovingLeft;
    public bool isMovingRight;

    public bool isSprinting;
    public bool isMeditating;
    public bool isLeveling;

    // Cursor

    public Vector3 cursorPosition;

    // InputActions, cause Unity overcomplicates everything these days...
    private InputAction MoveUp_Action;
    private InputAction MoveDown_Action;
    private InputAction MoveLeft_Action;
    private InputAction MoveRight_Action;
    private InputAction Cursor_Action;
    private InputAction Go_Action;
    private InputAction Stay_Action;
    //private InputAction Jump_Action;
    private InputAction Cast_Action;
    private InputAction Pause_Action;
    private InputAction Sprint_Action;
    private InputAction Meditate_Action;
    private bool jumpPressed;
    private bool pausePressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        // Gatherer start
        base.Start();

        // Set rotation speed
        baseRotationSpeed = rotationSpeed;

        // Return if we already know our actions
        if (MoveUp_Action != null)
            return;

        // Get actions
        MoveUp_Action = InputSystem.actions.FindAction("Move Up");
        MoveDown_Action = InputSystem.actions.FindAction("Move Down");
        MoveLeft_Action = InputSystem.actions.FindAction("Move Left");
        MoveRight_Action = InputSystem.actions.FindAction("Move Right");
        Cursor_Action = InputSystem.actions.FindAction("Cursor");
        Go_Action = InputSystem.actions.FindAction("Go");
        Stay_Action = InputSystem.actions.FindAction("Stay");
        //Jump_Action = InputSystem.actions.FindAction("Jump");
        //Jump_Action.performed += JumpPerformed;
        //Jump_Action.canceled += JumpCanceled;
        Cast_Action = InputSystem.actions.FindAction("Cast");
        Cast_Action.performed += CastPerformed;
        Cast_Action.canceled += CastCanceled;
        Pause_Action = InputSystem.actions.FindAction("Pause");
        Pause_Action.performed += PausePerformed;
        Pause_Action.canceled += PauseCanceled;
        Sprint_Action = InputSystem.actions.FindAction("Sprint");
        Sprint_Action.performed += SprintPerformed;
        Sprint_Action.canceled += SprintCanceled;
        Meditate_Action = InputSystem.actions.FindAction("Meditate");
        Meditate_Action.performed += MeditatePerformed;
        Meditate_Action.canceled += MeditateCanceled;
    }

    void OnDestroy()
    {
        // Unsubscribe from all input actions
        if (Cast_Action != null)
        {
            Cast_Action.performed -= CastPerformed;
            Cast_Action.canceled -= CastCanceled;
        }

        if (Pause_Action != null)
        {
            Pause_Action.performed -= PausePerformed;
            Pause_Action.canceled -= PauseCanceled;
        }

        if (Sprint_Action != null)
        {
            Sprint_Action.performed -= SprintPerformed;
            Sprint_Action.canceled -= SprintCanceled;
        }

        if (Meditate_Action != null)
        {
            Meditate_Action.performed -= MeditatePerformed;
            Meditate_Action.canceled -= MeditateCanceled;
        }
    }

    private void CastPerformed(InputAction.CallbackContext context)
    {
        CastSpell();
    }

    private void CastCanceled(InputAction.CallbackContext context)
    {
        ReleaseSpell();
    }

    private void SprintPerformed(InputAction.CallbackContext context)
    {
        Sprint();
    }

    private void SprintCanceled(InputAction.CallbackContext context)
    {
        EndSprint();
    }

    // private void MeditatePerformed(InputAction.CallbackContext context)
    // {
    //     Meditate();
    //     //GM.I.ui.Meditate();
    // }

    // private void MeditateCanceled(InputAction.CallbackContext context)
    // {
    //     EndMeditation();
    //     //GM.I.ui.EndMeditation();
    // }

    private void MeditatePerformed(InputAction.CallbackContext context)
    {
        // Start the meditation timer
        isPreparingToMeditate = true;
        meditationTimer = 0f;
    }

    private void MeditateCanceled(InputAction.CallbackContext context)
    {
        // Cancel meditation if we're still preparing
        if (isPreparingToMeditate)
        {
            isPreparingToMeditate = false;
            meditationTimer = 0f;
            return;
        }
        
        // Otherwise end meditation normally
        EndMeditation();
    }

    private void PausePerformed(InputAction.CallbackContext context)
    {
        if (!pausePressed)
        {
            pausePressed = true;
            GM.I.TogglePause();
        }

        //GM.I.Pause();
    }

    private void PauseCanceled(InputAction.CallbackContext context)
    {
        pausePressed = false;
    }

    // Update is called once per frame
    // Used for input & visuals to make the game feel responsive.
    void Update()
    {
        // -
        // --- Input
        // -

        // - Cursor

        // Get our mouse position in 3D cause Unity does not support me.
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Set cursor position
        cursorPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        // - WASD movement

        // Move up
        if (MoveUp_Action.IsPressed())
        {
            isMovingUp = true;
        }
        else
        {
            isMovingUp = false;
        }

        // Move down
        if (MoveDown_Action.IsPressed())
        {
            isMovingDown = true;
        }
        else
        {
            isMovingDown = false;
        }

        // Move left
        if (MoveLeft_Action.IsPressed())
        {
            isMovingLeft = true;
        }
        else
        {
            isMovingLeft = false;
        }

        // Move right
        if (MoveRight_Action.IsPressed())
        {
            isMovingRight = true;
        }
        else
        {
            isMovingRight = false;
        }

        // --- Abilities

        // - Go
        if (Go_Action.IsPressed())
        {
            Go();
        }

        // - Stay
        if (Stay_Action.IsPressed())
        {
            Stay();
        }
        else
        {
            // Check if we just released the button
            if (familiar.isStaying)
                Come();
        }

        // -
        // --- Visuals
        // -

        // - Camera

        // Follow player
        Vector3 camPosition = new Vector3(transform.position.x, transform.position.y, -10f);

        if (!isShaking)
            mainCam.transform.position = camPosition;
    }

    // Fixed Update is for physics & stuff.
    void FixedUpdate()
    {
        // Gatherer shared homeostasis
        Homeostasis();

        // Count down timers
        // (called in GM so spells cool down while meditating? maybe not needed anymore huh)
        //Timers();

        // Call OnFixedUpdate for each of our talents
        TalentFixedUpdate();

        // Basic movement
        BasicMovement();

        // Gatherer shared late fixed update
        LateFixedUpdate();
    }

    public void BasicMovement()
    {
        // Ignore movement inputs while dying.
        if (isDying)
            return;

        // Spend mana while sprinting
        if (isSprinting)
        {
            float manaCost = sprintManaCost * Time.deltaTime;

            // OOM
            if (currentMana < manaCost)
            {
                EndSprint();
            }
            else
            {
                SpendMana(manaCost);
            }
        }


        // Cursor

        // Rotate toward cursor

        // Secrent ancient angle jutsu
        float relativeY = cursorPosition.y - transform.position.y;
        float relativeX = cursorPosition.x - transform.position.x;
        float angle = Mathf.Atan2(relativeY, relativeX) * Mathf.Rad2Deg;
        angle -= 90;

        // angle is now aligned, could set rotation directly like so:
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // but instead we wanna use rotationSpeed!

        // So we get to use quaternions!

        // Get desired rotation
        Quaternion desiredRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Get the rotation we'll really rotate to, if we're too far away.
        Quaternion farRotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed);

        // Get angle to check if we're close enough to snap straight
        // (cause otherwise we would constantly overshoot slightly)
        angle = Quaternion.Angle(farRotation, desiredRotation);

        // Close enough to snap straight
        if (angle < rotationSpeed)
        {
            // Snap to desired location
            transform.rotation = desiredRotation;
        }
        else
        {
            // Far rotation
            transform.rotation = farRotation;
        }


        // Movement

        /*
        // WASD Movement
        if (isMovingUp)
        {
            rb2d.AddForceY(acceleration);
        }

        if (isMovingDown)
        {
            rb2d.AddForceY(-acceleration);
        }

        if (isMovingLeft)
        {
            rb2d.AddForceX(-acceleration);
        }

        if (isMovingRight)
        {
            rb2d.AddForceX(acceleration);
        }
        */

        // --- Go faster in the direction you are facing
        Vector2 movementDirection = Vector2.zero;

        // Build movement direction from WASD inputs
        if (isMovingUp) movementDirection.y += 1;
        if (isMovingDown) movementDirection.y -= 1;
        if (isMovingLeft) movementDirection.x -= 1;
        if (isMovingRight) movementDirection.x += 1;

        // Normalize to prevent diagonal movement from being faster
        if (movementDirection.magnitude > 0)
        {
            movementDirection.Normalize();

            // Calculate forward direction (where player is facing)
            Vector2 facingDirection = transform.up;

            // Calculate the dot product to determine alignment
            // Dot product gives 1 when perfectly aligned, -1 when opposite, 0 when perpendicular
            float alignmentFactor = Vector2.Dot(movementDirection, facingDirection);

            // Only apply bonus when movement is somewhat aligned with facing (positive dot product)
            float speedMultiplier = 1.0f;
            if (alignmentFactor > 0)
            {
                // Boost speed by up to 50% when perfectly aligned
                //speedMultiplier = 1.0f + (alignmentFactor * 0.5f);

                // Boost speed by up to 200% when perfectly aligned
                speedMultiplier = 1.0f + (alignmentFactor * 2f);
            }

            // Apply movement forces with alignment boost
            rb2d.AddForce(movementDirection * acceleration * speedMultiplier);
        }


        // Max speed
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

    // Call OnFixedUpdate for each talent the player has learned.
    public void TalentFixedUpdate()
    {
        // Loop through each talent.
        foreach (string talentName in talents.Keys)
        {
            // Get talent
            Talent talent = GM.I.talents[talentName];

            // Call OnFixedUpdate
            talent.OnFixedUpdate(this);
        }
    }

    // Decrements timers by Time.deltaTime
    // Assumed to be called once per Update (or FixedUpdate maybe? but not both!)
    public void Timers()
    {
        // - Meditation
        if (isPreparingToMeditate)
        {
            meditationTimer += Time.deltaTime;
            
            if (meditationTimer >= meditationFadeTime)
            {
                Meditate();
            }
        }

        // - Spells

        // Create a temporary list of spell names that need cooldown updates
        List<string> spellsToUpdate = new List<string>(spellCooldowns.Keys);

        // Iterate through our copy of the keys
        foreach (string spellName in spellsToUpdate)
        {
            // Update the cooldown in the original dictionary
            spellCooldowns[spellName] -= Time.deltaTime;

            // If cooldown complete, remove from original dictionary
            if (spellCooldowns[spellName] <= 0f)
                spellCooldowns.Remove(spellName);
        }
    }

    // Go!
    public void Go()
    {
        // Check familiar is alive
        if (familiar.isDying)
            return;
        
        // Mana Check
        if (familiar.currentMana < goManaCost * Time.deltaTime)
            return;

        // Spend mana
        familiar.SpendMana(goManaCost * Time.deltaTime);

        // Tell familiar to move toward our cursor.
        familiar.destination = new Vector3(cursorPosition.x, cursorPosition.y, 0);

        // Give familiar a lil boost
        familiar.BasicMovement();
    }

    // Tell your familiar to stay still, enhancing the power of its Gravity.
    public void Stay()
    {
        // Check familiar is alive
        if (familiar.isDying)
            return;

        // Check mana
        if (familiar.currentMana < familiar.stayCost * Time.deltaTime)
        {
            // Can't stay without mana!
            Come();
            return;
        }
        else
        {
            // Drain mana
            familiar.SpendMana(familiar.stayCost * Time.deltaTime);
        }

        // Start staying
        if (!familiar.isStaying)
        {
            // Set bool
            familiar.isStaying = true;
            familiar.isCalm = !familiar.isCalm;

            // Stop rotation
            familiar.rb2d.angularVelocity = 0f;

            // Disable linear movement
            familiar.AddAccelerationModifier("stay", 0f);
        }
    }

    // Tell your familiar to return to your current position.
    // Also enables movement, disabled from Stay.
    // Called when you release Stay.
    public void Come()
    {
        // Set bool
        familiar.isStaying = false;

        // Stop rotation spin from staying
        familiar.rb2d.angularVelocity = 0f;

        // Set destination
        familiar.destination = transform.position;

        // Enable movement
        familiar.RemoveAccelerationModifier("stay");
    }

    // Cast your currently selected spell
    // Called when the 'cast spell' button is pressed.
    public void CastSpell()
    {
        // Get talent
        Talent talent = GM.I.talents[currentSpell];

        // Check cooldown for this specific spell
        if (spellCooldowns.ContainsKey(currentSpell) && spellCooldowns[currentSpell] > 0f)
        {
            // VFX
            GM.I.ui.TriggerCooldownFlash();

            // SFX
            GM.I.dj.PlayEffect("no", transform.position);

            // return
            return;
        }

        // Check mana cost
        if (currentMana < talent.manaCost)
        {
            // VFX
            GM.I.ui.TriggerManaFlash();

            // SFX
            GM.I.dj.PlayEffect("no", transform.position);

            // return
            return;
        }

        // Spend mana
        SpendMana(talent.manaCost);

        // Set cooldown for this specific spell
        spellCooldowns[currentSpell] = talent.cooldown;

        // Store for UI (will need updating later)
        //GM.I.ui.lastSpellCastCooldown = talent.cooldown;

        // Call OnCast
        talent.OnCast();
    }

    // Called when the 'cast spell' button is released.
    public void ReleaseSpell()
    {

    }

    // Gain xp
    public void GainXP(float xpGained)
    {
        // Gain xp!
        xp += xpGained;
    }

    // Start leveling up!
    // Called when petting your familiar with enough xp.
    public void BeginLevelUp()
    {
        // Stop time
        GM.I.StopTime();

        // Set bool
        isLeveling = true;

        // Open talent screen
        GM.I.ui.OpenLevelUpScreen();
    }

    // Level up!
    public void LevelUp()
    {
        // Stop time.
        //GM.I.StopTime();

        // Spend xp
        xp -= level * 100;

        // Gain level
        level++;

        // Heal up!
        FullRestore();
        familiar.FullRestore();

        // Explode nearby asteroids
        Asteroid.Explosion(level, transform.position);

        // set bool
        isLeveling = false;

        // Open talent screen
        //GM.I.ui.OpenLevelUpScreen();
    }

    // Learn a new talent
    public void LearnTalent(string talentName)
    {
        // Actually level up upon learning a talent.
        LevelUp();

        // Check if talent exists
        if (!GM.I.talents.ContainsKey(talentName))
        {
            Debug.LogWarning("Error! talent not found : " + talentName);
            return;
        }

        // Get the talent
        Talent talent = GM.I.talents[talentName];

        // Check if it's new
        if (!talents.ContainsKey(talentName))
        {
            // Initialize first level
            talents[talentName] = 1;

            // Add component
            gameObject.AddComponent(GM.I.talents[talentName].GetType());

            // If it's a spell, add it to our spell list
            if (talent.isSpell)
                knownSpells.Add(talentName);
        }
        else
        {
            // Level up talent
            talents[talentName]++;
        }

        // Add stats
        GM.I.player.mind += talent.witchMind;
        GM.I.player.body += talent.witchBody;
        GM.I.player.soul += talent.witchSoul;
        GM.I.player.luck += talent.witchLuck;
        GM.I.familiar.mind += talent.familiarMind;
        GM.I.familiar.body += talent.familiarBody;
        GM.I.familiar.soul += talent.familiarSoul;
        GM.I.familiar.luck += talent.familiarLuck;

        GM.I.player.CalculateStats();
        GM.I.familiar.CalculateStats();

        // Call the talent's OnLearn method
        talent.OnLearn();

        // Visual feedback
        HitMarker.CreateLearnMarker(transform.position, "Learned: " + talentName);

        // Sound effect
        GM.I.dj.PlayEffect(talent.myClass, transform.position);
    }

    public void Sprint()
    {
        // Set bools
        isSprinting = true;
        isCalm = false;

        // Add move speed modifier
        AddSpeedModifier("sprint", 2f);
    }

    public void EndSprint()
    {
        // Set bool
        isSprinting = false;

        // Remove move speed modifier
        RemoveSpeedModifier("sprint");
    }

    public void Meditate()
    {
        // Set bools
        isMeditating = true;
        isCalm = true;

        // Open UI
        GM.I.ui.Meditate();

        // Stop time
        GM.I.StopTime();

        // Reset preparatation
        isPreparingToMeditate = false;
    }

    public void EndMeditation()
    {
        // Set bool
        isMeditating = false;

        // Close UI
        GM.I.ui.CloseMeditationUI();

        // Start time
        GM.I.StartTime();
    }


    // --- Rotation modifiers
    public Dictionary<string, float> rotationModifiers = new Dictionary<string, float>();
    private float baseRotationSpeed = 1f; // Store the original rotation speed

    // Add these methods
    public void AddRotationModifier(string id, float multiplier)
    {
        rotationModifiers[id] = multiplier;
        ApplyRotationModifiers();
    }

    public void RemoveRotationModifier(string id)
    {
        if (rotationModifiers.ContainsKey(id))
        {
            rotationModifiers.Remove(id);
            ApplyRotationModifiers();
        }
    }

    private void ApplyRotationModifiers()
    {
        // Reset to base
        rotationSpeed = baseRotationSpeed;

        // Apply all rotation modifiers
        foreach (float multiplier in rotationModifiers.Values)
        {
            rotationSpeed *= multiplier;
        }
    }

    // Track if we're in a black hole.
    public BlackHole currentBlackHole;

    // Track if we're flying over a planet.
    public Planet currentPlanet;

    new void OnTriggerEnter2D(Collider2D col)
    {
        // base gatherer
        base.OnTriggerEnter2D(col);

        // Enter a black hole
        BlackHole blackHole = col.GetComponent<BlackHole>();
        if (blackHole != null)
            EnterBlackHole(blackHole);

        // Enter a planet
        Planet planet = col.GetComponent<Planet>();
        if (planet != null)
            EnterPlanet(planet);
    }

    // Enter a black hole, beginning its dilation effects.
    public void EnterBlackHole(BlackHole blackHole)
    {
        // Set current black hole.
        currentBlackHole = blackHole;
    }

    // Exit a black hole, ending its dilation effects.
    public void ExitBlackHole(BlackHole blackHole)
    {
        // Reset current black hole.
        currentBlackHole = null;
    }

    // Enter a planet, beginning harvesting nectar if it has any.
    public void EnterPlanet(Planet planet)
    {
        // Set current planet.
        currentPlanet = planet;
    }

    public void ExitPlanet(Planet planet)
    {
        // Reset current planet.
        currentPlanet = null;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Exit a black hole.
        BlackHole blackHole = col.GetComponent<BlackHole>();
        if (blackHole != null)
        {
            ExitBlackHole(blackHole);
        }

        // Exit a planet.
        Planet planet = col.GetComponent<Planet>();
        if (planet != null)
        {
            ExitPlanet(planet);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (GM.I.nebula.myName != "Home") return;
        
        Planet planet = col.GetComponent<Planet>();
        if (planet != null && planet.nectar > 0)
        {
            if (currentPlanet != planet)
            {
                EnterPlanet(planet);
            }
            
            planet.harvestTimer += Time.deltaTime;

            if (planet.harvestTimer >= planet.harvestTime)
            {
                // Harvest complete
                planet.Harvest();
                /* int credits = planet.nectar;
                Gatherer.credits += credits;
                planet.nectar = 0;
                planet.harvestTimer = 0f;
                
                // Visual feedback
                HitMarker.CreateHitMarker(planet.transform.position, "+" + credits + " credits"); */
            }
        }
    }
    
    /*

    void OnTriggerStay2D(Collider2D col)
    {
        if (GM.I.nebula.myName != "Home") return;
        
        Planet planet = col.GetComponent<Planet>();
        if (planet != null && planet.nectar > 0)
        {
            if (harvestPlanet != planet)
            {
                harvestPlanet = planet;
                harvestTimer = 0f;
            }
            
            harvestTimer += Time.deltaTime;
            
            if (harvestTimer >= harvestTime)
            {
                // Harvest complete
                int credits = Mathf.FloorToInt(planet.nectar);
                Gatherer.credits += credits;
                planet.nectar = 0f;
                harvestPlanet = null;
                harvestTimer = 0f;
                
                // Visual feedback
                HitMarker.CreateHitMarker(planet.transform.position, "+" + credits + " credits");
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Planet planet = col.GetComponent<Planet>();
        if (planet == harvestPlanet)
        {
            harvestPlanet = null;
            harvestTimer = 0f;
        }
    }

    */
}
