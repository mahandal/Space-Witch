using UnityEngine;
using System.Collections.Generic;

public class Gatherer : MonoBehaviour
{
    [Header("Gatherer")]
    public static float starsGathered = 0;
    public static int starComboIndex = 0;
    //private int starComboIndex = 0;
    public static int moonsGathered = 1;
    public static int credits = 0;


    [Header("Attributes")]
    public int mind = 1;
    public int body = 1;
    public int soul = 1;
    public int luck = 1;


    [Header("Health")]
    public float currentHealth = 100f;
    public int maxHealth = 100;
    public float healthRegenDelay = 6f;


    [Header("Mana")]
    public float currentMana = 100f;
    public int maxMana = 100;
    public float manaRegenDelay = 1f;


    [Header("Movement")]
    // Current effective stats (calculated, but also used to initialize base values)
    public float acceleration;
    public float maxSpeed;

    // Base stats
    private float baseAcceleration;
    private float baseMaxSpeed;


    [Header("Weapons")]
    public Transform firePoint;
    public LaserWeapon weapon;


    [Header("Death and Recovery")]
    public bool diesPermanently = false;
    public bool isDying = false;
    public float deathSpinSpeed = 720f; // Degrees per second

    // How long it takes before the game ends after the player dies.
    // Weird name cause it was something else. Should prolly be refactored.
    //public float recoveryTime = 3f;

    // How long this gatherer will need to recover.
    //public float recoveryTimer = 0f;

    [Header("Baby Time Effects")]
    public bool isInBabyTime = false;
    public float babyTimeFlashRate = 5f; // Flashes per second
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Star Time Effects")]
    public bool isInStarTime = false;
    private Color starTimeColor = new Color(1f, 0.8f, 0.2f);

    [Header("Gatherer States")]
    public bool isCalm = true;

    [Header("Gatherer Automated Machinery")]
    public Rigidbody2D rb2d;
    public float previousFrameVelocity = 0f;
    public Vector3 originalScale;

    // Speed modifier
    public Dictionary<string, float> maxSpeedModifiers = new Dictionary<string, float>();
    public Dictionary<string, float> accelerationModifiers = new Dictionary<string, float>();

    // Timers
    public float timeAlive = 0f;
    public float babyTime = 0f;
    public float starTime = 0f;
    public float manaRegenTimer = 0f;
    public float healthRegenTimer = 0f;


    void Awake()
    {
        // Get rigid body
        rb2d = GetComponent<Rigidbody2D>();

        // Get sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Remember our roots

        // Remember starting color
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Remember starting scale
        originalScale = transform.localScale;

        // Remember our speed
        baseMaxSpeed = maxSpeed;
        baseAcceleration = acceleration;
    }


    public void Start()
    {
        // Initialize stats
        CalculateStats();

        // Start at full
        FullRestore();

        // Initialize speed modifiers
        ApplySpeedModifiers();

        // Initialize weapons(?)
        if (weapon != null)
            weapon.SetStats();
    }

    // Calculate stats that can be derived directly from attributes:
    // Max Health
    // Max Mana
    // Health regen delay
    // Mana regen delay
    public void CalculateStats()
    {
        // Max health
        maxHealth = body * 50;

        // Max mana
        maxMana = mind * 12;

        // Health regen delay
        healthRegenDelay = 13f - (12 * (body / 100f));

        // Mana regen delay
        manaRegenDelay = 6f - (5 * (mind / 100f));
    }

    // Fully heals this gatherer for both health and mana.
    public void FullRestore(GameObject healer = null, bool shouldShow = false)
    {
        //currentHealth = maxHealth;

        // Get missing health
        float missingHealth = maxHealth - currentHealth;

        // Heal missing health
        Heal(missingHealth, healer, shouldShow);

        // Set mana to full
        currentMana = maxMana;

        // Exit dying state
        //recoveryTimer = 0f;
        isDying = false;
        rb2d.angularVelocity = 0f;
    }

    // Should be called once per FixedUpdate
    public void Homeostasis()
    {
        // Check if we're dying
        if (isDying)
        {
            HandleDying();
            //return;
        } else
        {
            // - Life

            // Weapons
            if (weapon != null)
                weapon.HandleWeapon();
        }

        // Star time visual effect
        if (timeAlive < starTime)
        {
            isInStarTime = true;
            // Create a pulsing effect between gold and original color
            float starPulse = (Mathf.Sin(Time.time * babyTimeFlashRate * 1.5f * Mathf.PI) + 1f) / 2f;
            spriteRenderer.color = Color.Lerp(originalColor, starTimeColor, starPulse);
        }
        // Baby time visual effect
        else if (timeAlive < babyTime)
        {
            isInBabyTime = true;
            // Create a pulsing effect between black and original color
            float pulse = (Mathf.Sin(Time.time * babyTimeFlashRate * Mathf.PI) + 1f) / 2f;
            spriteRenderer.color = Color.Lerp(originalColor, Color.black, pulse);
        }
        else if (isInBabyTime || isInStarTime)
        {
            // Reset color when effect ends
            spriteRenderer.color = originalColor;
            isInBabyTime = false;
            isInStarTime = false;
        }

        // Count timers
        GathererTimers();

        // Regen health and mana
        Regen();
    }

    // Called at the end of fixed update by each Gatherer.
    // (or it should be! make sure it is!)
    public void LateFixedUpdate()
    {
        // Store previous frame velocity
        previousFrameVelocity = rb2d.linearVelocity.magnitude;
    }

    public void GathererTimers()
    {
        // Time alive
        timeAlive += Time.deltaTime;

        // Health regen timer
        if (healthRegenTimer > 0)
            healthRegenTimer -= Time.deltaTime;
        
        // Mana regen timer
        if (manaRegenTimer > 0)
            manaRegenTimer -= Time.deltaTime;
    }

    public void Regen(float multiplier = 1f)
    {
        // Regen health
        if (healthRegenTimer <= 0f)
            Heal(soul * Time.deltaTime * multiplier, gameObject, false);

        // Regen mana
        if (manaRegenTimer <= 0f)
            GainMana(soul * Time.deltaTime * multiplier);
    }

    // Enter the dying state with spin
    private void EnterDyingState(Asteroid killer)
    {
        // Set bool
        isDying = true;

        // If this is the player, all bees come to help
        if (this is Player)
        {
            foreach (Bee b in GM.I.bees)
            {
                if (b.gameObject.activeSelf)
                    b.destination = transform.position;
            }
        }

        // Set recovery time
        //recoveryTimer = recoveryTime;

        // Calculate direction away from the asteroid
        Vector2 knockbackDirection = (transform.position - killer.transform.position).normalized;
        
        // Add spin torque in the appropriate direction based on which side we were hit from
        float hitFromRight = Vector2.Dot(knockbackDirection, Vector2.right);
        float torqueDirection = hitFromRight > 0 ? 1f : -1f;
        deathSpinSpeed = -720f * torqueDirection;
        float torqueMagnitude = Random.Range(10f, 20f);
        rb2d.AddTorque(torqueDirection * torqueMagnitude, ForceMode2D.Impulse);
        
        // Apply knockback force away from asteroid
        rb2d.linearVelocity = Vector2.zero; // Clear existing velocity
        float force = killer.size * killer.previousFrameVelocity;
        rb2d.AddForce(knockbackDirection * force, ForceMode2D.Impulse);

        // - Witches
        if (this is Player || this is Familiar)
        {
            // Lose a life, unless we're at home.
            if (GM.I.nebula.myName != "Home")
                GM.I.player.livesRemaining--;
        }



        // - Bees
        Bee bee = GetComponent<Bee>();

        // Last words
        if (bee != null)
            bee.LastWords();
    }

    // Handle behavior while in dying state
    private void HandleDying()
    {
        // Count down recovery timer
        //recoveryTimer -= Time.deltaTime;

        // Set baby time
        babyTime = timeAlive + 7f;

        // Apply spin manually to ensure it keeps spinning
        transform.Rotate(0, 0, deathSpinSpeed * Time.deltaTime);

        // Apply drag to slow down
        rb2d.linearVelocity *= 0.98f;

        // Check if we've recovered
        if (currentHealth >= maxHealth)
        {
            // Handle real death
            if (diesPermanently)
            {
                Object.Destroy(gameObject);
                return;
            }

            // Fully restore and exit dying state
            FullRestore();

            // Bee-specific recovery messages
            Bee bee = GetComponent<Bee>();
            if (bee != null)
                bee.FirstWords();

            // Check if it's the end.
            if (this is Player || this is Familiar)
            {
                if (GM.I.player.livesRemaining <= 0)
                    GM.I.Lose();
            }
        }
    }

    // Collisions are for asteroids and also bumping into your familiar and/or bees.
    public void OnCollisionEnter2D(Collision2D col)
    {
        // Avoid collisions while dying?
        // if (isDying)
        //     return;

        // --- Specific

        // - Player
        if (this is Player)
        {
            // Familiar
            Familiar fam = col.gameObject.GetComponent<Familiar>();
            if (fam != null)
            {
                // Check if we should level
                if (GM.I.player.xp >= GM.I.player.level * 100)
                {
                    // Level up!
                    GM.I.player.BeginLevelUp();

                    // Play sfx!
                    string soundFileName = "big_purr";
                    GM.I.dj.PlayEffect(soundFileName, fam.transform.position);
                } else {
                    // Play sfx!
                    string soundFileName = "purr_" + Random.Range(1, 14);
                    GM.I.dj.PlayEffect(soundFileName, fam.transform.position);
                }
            }

            // Bees
            Bee bee = col.gameObject.GetComponent<Bee>();
            if (bee != null)
                bee.Bump();
        }

        // Familiar
        if (this is Familiar)
        {
            // Bees
            Bee bee = col.gameObject.GetComponent<Bee>();
            if (bee != null)
                bee.Bump();
        }

        // - Asteroids

        // Get asteroid
        Asteroid asteroid = col.gameObject.GetComponent<Asteroid>();

        // null check asteroid
        if (asteroid != null && !asteroid.exploding)
        {
            // Check if we're a baby (and not a star!)
            if (timeAlive < babyTime && timeAlive > starTime)
                return;
            
            // Set exploding
            asteroid.exploding = true;

            // Calculate damage
            //float damage = asteroid.damage * asteroid.size * asteroid.previousFrameVelocity * asteroid.previousAngularVelocity;
            float damage = asteroid.CalculateDamage();

            // Do damage, unless we're a star?!
            if (timeAlive > starTime)
                ReceiveDamage(damage, asteroid.gameObject);
            else
                HitMarker.CreateCombatMarker(transform.position, "Invincible!");

            // Explode!
            asteroid.Explode();

            // SFX
            //GM.I.dj.PlayEffect("asteroid_collision", transform.position);
        }
    }

    // Handle gathering!
    // Note: Could be refactored prolly p easily to be significantly cleaner.
    public void OnTriggerEnter2D(Collider2D col)
    {
        // for now bees don't gather. hmmmm...
        if (this is Bee)
            return;
        
        // - Stars

        // Get star
        Star star = col.gameObject.GetComponent<Star>();

        // null check star
        if (star != null)
        {
            // Gather star
            starsGathered += star.value;

            // OnGather
            OnGather(star);

            // Star combo!
            StarCombo(star.transform.position, star.value);

            // Gain xp!
            GM.I.player.GainXP(star.value);

            // Hit marker!
            HitMarker.CreateHitMarker(star.transform.position, star.value);

            // Clean up star object
            Object.Destroy(star.gameObject);
        }

        // - Moons

        // Get moon
        Moon moon = col.gameObject.GetComponent<Moon>();

        // null check moon
        if (moon != null)
        {
            // Avoid double collecting(?)
            if (moon.isBeingGathered)
                return;
            moon.isBeingGathered = true;
        
            // Gather moon!
            moonsGathered++;
            
            // OnGather
            OnGather(moon);

            // Spawn another one
            GM.I.spawnManager.SpawnMoon(moon.planet);

            // Spawn some stars!?
            //GM.I.spawnManager.SpawnStars(transform.position, 7 * moonsGathered, moonsGathered);

            // Add time to gameTimer
            if (moonsGathered <= 7)
            {
                // GM.I.songTimer += 180f;

                // Increment unlocked song index
                GM.I.unlockedSongIndex++;

                // Loop if it overflows
                if (GM.I.unlockedSongIndex >= GM.I.songs.Count)
                    GM.I.unlockedSongIndex = 0;
                
                // Increase game timer & total time
                GM.I.gameTimer += GM.I.songs[GM.I.unlockedSongIndex].duration;
                GM.I.totalTime += GM.I.songs[GM.I.unlockedSongIndex].duration;

                // Hit marker
                HitMarker.CreateSongMarker(moon.transform.position, GM.I.songs[GM.I.unlockedSongIndex].myName + " (" + moonsGathered.ToString() + "/7)");

                // SFX
                GM.I.dj.PlayGatherSound("moon_gathered", moon.transform.position);
            } else {
                // Hit marker
                HitMarker.CreateSongMarker(moon.transform.position, "(" + moonsGathered.ToString() + "/7)");

                // SFX
                GM.I.dj.PlayGatherSound("moon_2", moon.transform.position);
            }

            // Clean up moon
            Object.Destroy(moon.gameObject);
        }

        // Potions
        Potion potion = col.gameObject.GetComponent<Potion>();
        if (potion != null && potion.isReady && !potion.isMine)
        {
            // Drink up!
            potion.Drink(this);

            // Clean up(?)
            //Object.Destroy(potion.gameObject);
        }
    }

    // Used to track when we are on a planet
    void OnTriggerStay2D(Collider2D col)
    {
        // Get planet
        Planet planet = col.GetComponent<Planet>();

        // Planets only!
        if (planet != null)
        {

        }
    }

    // Speed modifiers

    public void AddSpeedModifier(string id, float multiplier)
    {
        AddMaxSpeedModifier(id, multiplier);
        AddAccelerationModifier(id, multiplier);
    }

    public void RemoveSpeedModifier(string id)
    {
        RemoveMaxSpeedModifier(id);
        RemoveAccelerationModifier(id);
    }
    public void AddMaxSpeedModifier(string id, float multiplier)
    {
        maxSpeedModifiers[id] = multiplier;
        ApplySpeedModifiers();
    }

    public void RemoveMaxSpeedModifier(string id)
    {
        if (maxSpeedModifiers.ContainsKey(id))
        {
            maxSpeedModifiers.Remove(id);
            ApplySpeedModifiers();
        }
    }

    public void AddAccelerationModifier(string id, float multiplier)
    {
        accelerationModifiers[id] = multiplier;
        ApplySpeedModifiers();
    }

    public void RemoveAccelerationModifier(string id)
    {
        if (accelerationModifiers.ContainsKey(id))
        {
            accelerationModifiers.Remove(id);
            ApplySpeedModifiers();
        }
    }

    private void ApplySpeedModifiers()
    {
        // Reset to base
        maxSpeed = baseMaxSpeed;
        acceleration = baseAcceleration;
        
        // Apply all speed modifiers
        foreach (float multiplier in maxSpeedModifiers.Values)
        {
            maxSpeed *= multiplier;
        }
        
        // Apply all acceleration modifiers
        foreach (float multiplier in accelerationModifiers.Values)
        {
            acceleration *= multiplier;
        }
    }

    // Receive Damage
    public virtual void ReceiveDamage(float damage, GameObject attacker = null)
    {
        // Check reflection
        // (for now just The Tower)
        if (GM.I.player.talents.ContainsKey("The Tower"))
        {
            TheTower theTower = (TheTower)GM.I.talents["The Tower"];
            theTower.ReflectDamage(damage, attacker);
        }

        // Check evasion
        float evasionRoll = Random.Range(1, 100);
        if (luck > evasionRoll)
        {
            // TBD: Sound effect, visuals?
            HitMarker.CreateCombatMarker(transform.position, "Evaded!");

            // No damage to be done!
            return;
        }

        // Check crit
        if (-luck > evasionRoll)
        {
            HitMarker.CreateCombatMarker(transform.position, "CRITICAL!");

            damage *= 2f;
        }

        // Create damage marker
        HitMarker.CreateDamageMarker(transform.position, damage);

        // Shake camera!
        if (this is Player || this is Familiar)
            GM.I.ShakeCamera(damage / 100f, 0.5f, this.transform.position);

        // Set health regen timer
        healthRegenTimer = healthRegenDelay;
            
        // Delegate to Heal
        Heal(-damage, attacker, false);
    }

    // Heal health!
    public void Heal(float healing, GameObject healer = null, bool shouldShow = true)
    {
        // Create healing marker
        if (healing > 0 && shouldShow && currentHealth < maxHealth)
            HitMarker.CreateHealMarker(transform.position, healing);

        // Stop dying
        if (healing > 0 && isDying && shouldShow)
            isDying = false;

        // Reset health regen timer
        if (healing > 0 && shouldShow)
            healthRegenTimer = 0f;

        // Change health
        currentHealth += healing;

        // Check minimum health
        if (healing < 0 && currentHealth <= 0)
        {
            // Enforce minimum
            currentHealth = 0f;

            // Death camera shake
            if (this is Player || this is Familiar)
            {
                float shakeStrength = 9.5f - GM.I.player.livesRemaining;
                GM.I.ShakeCamera(shakeStrength, 2f, this.transform.position);
            }

            // Enter dying state
            Asteroid asteroid = healer.GetComponent<Asteroid>();
            EnterDyingState(asteroid);
        }

        // Check maximum health
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    // Gain mana!
    public void GainMana(float manaGained)
    {
        // Change mana
        currentMana += manaGained;

        // Check minimum mana
        if (currentMana < 0)
            currentMana = 0;

        // Check maximum mana
        if (currentMana > maxMana)
            currentMana = maxMana;
    }

    // Spend mana!
    // Wrapper of gain mana except it sets manaRegenTimer
    public void SpendMana(float manaSpent)
    {
        // Set timer
        manaRegenTimer = manaRegenDelay;

        // Ungain main
        GainMana(-manaSpent);
    }

    // Star combo!
    public void StarCombo(Vector3 position, float value)
    {
        // Increment index
        starComboIndex++;

        // Overflow reset
        if (starComboIndex > 13)
            starComboIndex = 1;
        
        // Get sound file name
        // Default to familiar eating sound
        //string soundFileName = "eat_" + starComboIndex.ToString();
        string soundFileName = "bass_" + starComboIndex.ToString();

        // Player piano
        if (this is Player)
            soundFileName = "star_" + starComboIndex.ToString();
        
        // Player ngoni
        if (this is Player && GM.I.player.isSprinting)
            soundFileName = "eat_" + starComboIndex.ToString();

        // Crow caw
        if (this is Crow)
            soundFileName = "crow_" + starComboIndex.ToString();

        // Play sound
        GM.I.dj.PlayGatherSound(soundFileName, position, value/5f);
    }

    // Call this when gathering a star
    public void OnGather(Star star)
    {
        // Call OnGather for each active talent
        foreach (string key in GM.I.player.talents.Keys)
        {
            Talent talent = GM.I.talents[key];
            talent.OnGather(star, this);
        }
    }

    // Call this when gathering a moon
    public void OnGather(Moon moon)
    {
        // Call OnGather for each active talent
        foreach (string key in GM.I.player.talents.Keys)
        {
            Talent talent = GM.I.talents[key];
            talent.OnGather(moon, this);
        }
    }
}
