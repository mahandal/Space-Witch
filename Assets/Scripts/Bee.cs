using UnityEngine;

public class Bee : Gatherer
{
    [Header("Bee")]
    public int index;
    public int goalIndex;
    public Planet goal;
    public Vector3 destination;
    public Planet previousPlanet;
    //public float starTime = 1f;
    private float sprintManaCost = 25f;
    public float pollinationRange = 1f;

    // How long since we've been bumped
    public float timeSinceLastBump = 0f;

    // How many stars to generate per second unbumped
    public float bumpStarMultiplier = 0.5f;

    [Header("Nectar")]
    public int nectar = 0;

    // Timers
    //public float starTimer = 0f;

    [Header("Machinery")]
    // States
    public bool isBumpable = true;
    public bool isPollinating = false;
    public bool isSprinting = false;

    private string[] deathMessages = new string[] {
        "Bzz bzz!",
        "Buzz!",
        "Buzz buzz",
        "Buzz.",
        "buzz",
        "Buzz",
        "Bzz",
        "bzz",
        "Oh no!",
        "Not again!",
        "Bzzzzt!",
        "I'll bee back!",
        "Noooooo!",
        "Un-bee-lievable!",
        "They came from bee-hind!",
        "No more pollinatin' for me...",
        "So much for my honeymoon...",
        "Spinning to safety!",
        "Whoops!",
        "Ow!",
        "Dangatang!",
        "Mama warned me bout those...",
        "Shoulda tried harder in PE...",
        "Dang thought I dodged that one.",
        "Tarnation!",
        "Well that sucks.",
        "Aw.",
        "Oops.",
        "Dumb weak asteroid thinks it can stop me...",
        "That one was harder than it looked!",
        "I'm sorry, I was distracted by that beautiful witch over there.",
        "Hey witch, come save me!",
        "In need of some lovin'.",
        "Too much!",
        "Here I go spinnin again!",
        "Spin time!",
        "Spinny bee spinny bee what do you see?",
        "Ok I didn't like that one.",
        "Hrm.",
        "I can't fail now, my queen needs me!",
        "You spin me right round, baby!",
        "Spin me like a record!",
        "Once more, into the abyss...",
        "Bumbling back into the void..."
    };

    string[] recoveryMessages = new string[] {
        "For the queen!",
        "We back!",
        "I'm back, baby!",
        "Bzz bzz!",
        "Buzz!",
        "Buzz buzz",
        "Buzz.",
        "buzz",
        "Buzz",
        "Bzz",
        "bzz",
        "Flight systems operational.",
        "Destination acquired, navigation commencing",
        "Hull damage minimized. Control restored.",
        "Control restored.",
        "ok ok I'll stop spinning",
        "Enough of that nonsense.",
        "Back to bees-ness.",
        "Imagine the speed I could be spinning...",
        "Mama?",
        "Where was I going again?",
        "Victory lies just beyond the horizon.",
        "Make your own victory!",
        "You win every time you improve!",
        "Practice makes perfect!",
        "The way that can be written is not the eternal way.",
        "The eternal way is nameless, formless.",
        "Words bind the truth. Bee free.",
        "Bees make the best pilots, beecause we have flight experience!",
        "Don't bee sorry, bee better.",
        "Whoo!",
        "Yippee!",
        "Now this is real racing!",
        "Now where was that bird?",
        "You ever been to Denver?",
        "Love!",
        "Life!",
        "Dream big!",
        "Dream it, see it, believe it, bee it.",
        "Try it imperfectly! Or you'll never know how perfect you can bee!",
        "Don't worry, bee happy!",
        "Bumbling back out of the void!"
    };


    string[] thanksMessages = new string[] {
        "For the queen!",
        "The bees are back in town!",
        "Bzz bzz!",
        "Buzz!",
        "Buzz buzz",
        "Buzz.",
        "buzz",
        "Buzz",
        "Bzz",
        "bzz",
        "Thanks!",
        "Appreciate it!",
        "Thank you!",
        "Praise bee!",
        "Whoo!",
        "Yippee!",
        "The hive is grateful.",
        "You deserve to bee happy!",
        "Don't worry. Bee happy!",
        "I'll bumble extra just for you!"
    };

    string[] greetings = new string[] {
        "Bzz bzz!",
        "Buzz!",
        "Buzz buzz",
        "Buzz.",
        "buzz",
        "Buzz",
        "Bzz",
        "bzz",
        "buz",
        "zzz",
        "Zug zug"
    };

    void FixedUpdate()
    {
        // Timers
        Timers();

        // Homeostasis
        Homeostasis();

        if (isDying)
            return;

        // Handle sprinting
        HandleSprint();

        // Navigate
        Navigate();

        // Ambulate
        Ambulate();

        LateFixedUpdate();
    }

    private void Timers()
    {
        // Increment time since last bump
        timeSinceLastBump += Time.deltaTime;
    }

    // Seek out witches in need, or find our next planet otherwise.
    public void Navigate()
    {
        // First priority: dying witches
        Transform dyingTarget = null;
        float closestDistance = float.MaxValue;
        
        // Check if player is dying
        if (GM.I.player.isDying)
        {
            float playerDistance = Vector3.Distance(transform.position, GM.I.player.transform.position);
            if (playerDistance < closestDistance)
            {
                dyingTarget = GM.I.player.transform;
                closestDistance = playerDistance;
            }
        }
        
        // Check if familiar is dying
        if (GM.I.familiar.isDying)
        {
            float familiarDistance = Vector3.Distance(transform.position, GM.I.familiar.transform.position);
            if (familiarDistance < closestDistance)
            {
                dyingTarget = GM.I.familiar.transform;
                closestDistance = familiarDistance;
            }
        }
        
        // Normal planet navigation for when no one is dying
        if (goal == null)
            goal = GM.I.activePlanets[0];

        float goalDistance = Vector3.Distance(transform.position, goal.transform.position);

        if (goalDistance < pollinationRange && !isPollinating)
        {
            isPollinating = true;
            
            int pollenAmount = 1;
            if (previousPlanet != null)
            {
                float planetDistance = Vector3.Distance(previousPlanet.transform.position, goal.transform.position);
                pollenAmount = Mathf.Max(1, Mathf.FloorToInt(planetDistance / 5));
            }

            Pollinate(goal, pollenAmount);
            previousPlanet = goal;
            
            goalIndex++;
            if (goalIndex >= GM.I.activePlanets.Count)
                goalIndex = 0;
                
            goal = GM.I.activePlanets[goalIndex];
            isPollinating = false;
        }

        // - Destination
        // Finally, actually set our destination.
        // If we have a dying witch to help, go there.
        // Otherwise, go to our goal planet.
        if (dyingTarget != null)
        {
            // Move toward dying witch
            destination = dyingTarget.position;
        } else {
            destination = goal.transform.position;
        }
    }

    public void Pollinate(Planet planet, int pollenAmount)
    {
        // Planet pollen
        planet.Pollinate(pollenAmount);

        // Nectar
        nectar += pollenAmount;
    }

    public void Ambulate()
    {
        // Face goal
        Vector3 targ = destination - transform.position;
        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        angle -= 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Move toward goal
        Vector2 direction = (destination - transform.position).normalized;
        rb2d.AddForce(direction * acceleration);

        // Max speed
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

    public void LastWords()
    {
        // Show death message
        string randomMessage = deathMessages[Random.Range(0, deathMessages.Length)];
        HitMarker.CreateHitMarker(transform.position, randomMessage, previousFrameVelocity);
    }

    public void FirstWords()
    {
        string recoveryMessage = recoveryMessages[Random.Range(0, recoveryMessages.Length)];
        HitMarker.CreateHitMarker(transform.position, recoveryMessage, acceleration);
    }

    public void SayThanks()
    {
        string thanksMessage = thanksMessages[Random.Range(0, thanksMessages.Length)];
        HitMarker.CreateHitMarker(transform.position, thanksMessage, acceleration);
    }

    public void SayHi()
    {
        string hiMessage = greetings[Random.Range(0, greetings.Length)];
        HitMarker.CreateHitMarker(transform.position, hiMessage, previousFrameVelocity);
    }

    public void Bump()
    {
        // Check bumpability
        if (!isBumpable && !isDying)
            return;

        // Bee friendly!
        if (isDying)
        {
            // Heal up.
            //FullRestore();

            // Bee grateful!
            SayThanks();
        } else {
            // Say hi!
            SayHi();
        }
        
        // Get whoever bumped us
        Gatherer bumper = GetBumper();

        // Heal up
        //FullRestore(bumper.gameObject, true);
        float healYouAmount = soul;
        bumper.Heal(healYouAmount, gameObject);
        float healMeAmount = bumper.soul;
        Heal(healMeAmount, bumper.gameObject);

        // Spawn some stars!
        BumpStars(bumper);
        
        // Set bumpable
        isBumpable = false;

        // Reset timer
        timeSinceLastBump = 0f;
    }

    public void SpawnStar()
    {
        // Convert current speed into a range from 1 to 5
        float value = 1f + (rb2d.linearVelocity.magnitude - 1f) / 3f;

        // Spawn a star
        GM.I.spawnManager.SpawnStar(transform.position.x, transform.position.y, value);
    }

    // Spawn stars around this bee when bumped.
    public void BumpStars(Gatherer bumper)
    {
        // Base amount
        float baseStars = 1f;
        
        // Velocity factors (both yours and the bee's)
        float beeVelocityFactor = Mathf.Min(rb2d.linearVelocity.magnitude * 0.5f, 5f);
        
        // Get the player or familiar that bumped this bee
        //Gatherer bumper = GetBumper();
        float bumperVelocityFactor = bumper != null ? 
            Mathf.Min(bumper.previousFrameVelocity * 0.5f, 5f) : 0f;
        
        // Time factor with diminishing returns
        float timeFactor = Mathf.Min(timeSinceLastBump, 10f) * 0.3f;
        
        // Recovery bonus
        // TBD: re-implement. could use health percent?
        //float recoveryBonus = isDying ? recoveryTimer * 1.5f : 0f;
        
        // Calculate total with additive bonuses rather than multiplication
        int numberToSpawn = Mathf.RoundToInt(baseStars + 
                                            beeVelocityFactor + 
                                            bumperVelocityFactor + 
                                            timeFactor);
        
        // Spawn stars
        GM.I.spawnManager.SpawnStars(transform.position, numberToSpawn);
    }

    // Helper method to identify what bumped the bee
    private Gatherer GetBumper()
    {
        // Simple proximity check - could be refined with actual collision data
        float playerDist = Vector3.Distance(transform.position, GM.I.player.transform.position);
        float familiarDist = Vector3.Distance(transform.position, GM.I.familiar.transform.position);
        
        if (playerDist < 1.5f) return GM.I.player;
        if (familiarDist < 1.5f) return GM.I.familiar;
        return null;
    }

    // Handle sprinting, mechanically and emotionally.
    void HandleSprint()
    {
        // Start sprinting when at full mana
        if (!isSprinting && currentMana >= maxMana)
        {
            isSprinting = true;
            AddSpeedModifier("sprint", 2f);
        }
        
        // Continue sprinting until out of mana
        if (isSprinting)
        {
            float manaCost = sprintManaCost * Time.deltaTime;
            if (currentMana < manaCost)
            {
                isSprinting = false;
                RemoveSpeedModifier("sprint");
            }
            else
            {
                SpendMana(manaCost);
            }
        }
    }

    public void GetStats()
    {
        if (index < 9) // Bees 0-8 care about their specific planet
        {
            Planet myPlanet = GM.I.home.planets[index];
            mind = myPlanet.science;
            body = myPlanet.environment;
            soul = myPlanet.culture;
            luck = myPlanet.economy;
        }
        else // Bees 9-11 use averages
        {
            int totalScience = 0;
            int totalEnvironment = 0; 
            int totalCulture = 0;
            int totalEconomy = 0;
            
            foreach (Planet planet in GM.I.home.planets)
            {
                totalScience += planet.science;
                totalEnvironment += planet.environment;
                totalCulture += planet.culture;
                totalEconomy += planet.economy;
            }
            
            mind = totalScience / GM.I.home.planets.Count;
            body = totalEnvironment / GM.I.home.planets.Count;
            soul = totalCulture / GM.I.home.planets.Count;
            luck = totalEconomy / GM.I.home.planets.Count;
        }
        
        CalculateStats();
    }
}
