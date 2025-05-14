using UnityEngine;
using System.Collections.Generic;

public class Familiar : Gatherer
{
    [Header("Stats")]
    // - Gravity

    // How far away a star can be before gravity begins to use its real strength.
    public float gravityRange;

    // The strength with which stars are pulled in.
    public float gravityStrength;

    // The minimum amount of gravity applied to stars outside of our range.
    public float gravityMinimum;

    // - Stay

    // How much mana stay costs while holding it down.
    public float stayCost = 1f;

    // How much stay multiplies your gravity strength.
    public float stayPower = 2f;
    
    // How long it takes for stay to reach its max strength.
    public float stayDelay = 3f;

    [Header("Automated Machinery")]
    public Vector3 destination = Vector3.zero;
    //public Rigidbody2D rb2d;
    public Player player;

    [Header("States")]
    public bool isStaying = false;
    //public bool isComing = false;

    [Header("Timers")]
    public float stayTimer = 0f;

    new void Start()
    {
        // Gatherer start
        base.Start();

        // Get player
        player = GM.I.player;
        
        // Remember our roots
        baseGravityStrength = gravityStrength;
        baseGravityRange = gravityRange;
        baseStayPower = stayPower;
    }

    // Fixed Update handles physics & stuf
    void FixedUpdate()
    {
        // Gatherer shared homeostasis
        Homeostasis();

        // Timers
        Timers();

        // Navigation
        Navigate();
        
        // Familiar's basic movement
        BasicMovement();

        // Gatherer shared late fixed update
        LateFixedUpdate();
    }

    public void Timers()
    {
        // Stay
        if (isStaying)
        {
            // Increment timer
            stayTimer += Time.deltaTime;

            // Check percentage
            float percent = stayTimer / stayDelay;
            if (percent > 1f)
                percent = 1f;
            
            //
            AddStayPowerModifier("Charging", percent);
        } else {
            // Reset
            stayTimer = 0f;
        }
    }

    public void Navigate()
    {
        // Check if close enough to destination to pick a new one
        float distToDestination = Vector3.Distance(transform.position, destination);
        if (distToDestination < 0.5f && !isStaying) // Adjust this threshold as needed
        {
            // Pick a new random destination nearby
            float randomRadius = Random.Range(1f, 2f); // Random distance from current position
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Random direction
            
            // Calculate new position
            float newX = transform.position.x + randomRadius * Mathf.Cos(randomAngle);
            float newY = transform.position.y + randomRadius * Mathf.Sin(randomAngle);
            
            // Set new destination
            destination = new Vector3(newX, newY, 0);
        }
    }

    // Accelerate toward our current destination, capped by max speed.
    public void BasicMovement()
    {
        // - Move toward destination

        // Get direction in 3d cause Unity is rude
        Vector3 direction = (destination - transform.position).normalized;

        // Convert 3d vector back to 2d because Unity does understand that 2D games exist, they just hate them
        Vector2 dir = new Vector2(direction.x, direction.y);

        // Accelerate toward dir
        rb2d.AddForce(dir * acceleration);



        // Rotate?
        if (isStaying)
        {
            //rb2d.rotation += (gravityStrength + gravityRange + gravityMinimum) * stayPower;
        } else {
            //rb2d.rotation -= 5f;
            //rb2d.rotation -= (gravityStrength + gravityRange + gravityMinimum) / stayPower;
        }

        // Rotate?
        if (isStaying)
        {
            float stayTorque = (gravityStrength + gravityRange + gravityMinimum) * stayPower / 100f;
            rb2d.AddTorque(stayTorque, ForceMode2D.Force);
        } else {
            float moveTorque = (gravityStrength + gravityRange + gravityMinimum) / stayPower;
            //rb2d.AddTorque(-moveTorque, ForceMode2D.Force);
        }

        if (rb2d.linearVelocity.magnitude > 0.1f && !isStaying)
        {
            float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        
        // Max speed
        if(rb2d.linearVelocity.magnitude > maxSpeed)
        {
               rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

    // Gravity modifiers
    public Dictionary<string, float> gravityModifiers = new Dictionary<string, float>();
    private float baseGravityStrength;
    private float baseGravityRange;

    public void AddGravityModifier(string id, float multiplier)
    {
        gravityModifiers[id] = multiplier;
        ApplyGravityModifiers();
    }

    public void RemoveGravityModifier(string id)
    {
        if (gravityModifiers.ContainsKey(id))
        {
            gravityModifiers.Remove(id);
            ApplyGravityModifiers();
        }
    }

    private void ApplyGravityModifiers()
    {
        // Reset to base values
        gravityStrength = baseGravityStrength;
        gravityRange = baseGravityRange;
        
        // Apply all modifiers
        foreach (float multiplier in gravityModifiers.Values)
        {
            gravityStrength *= multiplier;
            gravityRange *= multiplier;
        }
    }

    // Staying power modifiers
    private float baseStayPower;
    public Dictionary<string, float> stayPowerModifiers = new Dictionary<string, float>();

    public void AddStayPowerModifier(string id, float multiplier)
    {
        stayPowerModifiers[id] = multiplier;
        ApplyStayPowerModifiers();
    }

    public void RemoveStayPowerModifier(string id)
    {
        if (stayPowerModifiers.ContainsKey(id))
        {
            stayPowerModifiers.Remove(id);
            ApplyStayPowerModifiers();
        }
    }

    private void ApplyStayPowerModifiers()
    {
        // Reset to base value
        stayPower = baseStayPower;
        
        // Apply all modifiers
        foreach (float multiplier in stayPowerModifiers.Values)
        {
            stayPower *= multiplier;
        }
    }
}
