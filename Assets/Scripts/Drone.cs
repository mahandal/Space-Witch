using UnityEngine;
using System.Collections.Generic;

public class Drone : Gatherer
{
    [Header("Drone")]
    public float orbitDistance = 5f;
    public float orbitSpeed = 100f;
    private float currentAngle = 0f;
    
    [Header("Weapons")]
    public Transform firePoint;
    public LaserWeapon weapon;
    
    new void Start()
    {
        base.Start();
        
        // Initialize weapon component
        //weapon = new LaserWeapon(transform, this);
        
        // Set initial stats
        int talentLevel = 1; // Default
        SetStats(talentLevel);
    }
    
    void FixedUpdate()
    {
        // Gatherer shared homeostasis (health/mana handling)
        Homeostasis();

        // Drone movement
        OrbitPlayer();
        
        // Weapon systems
        weapon.HandleWeapon();

        // Gatherer shared late fixed update
        LateFixedUpdate();
    }

    void OrbitPlayer()
    {
        // Update orbit angle
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;
        
        // Calculate position
        float x = GM.I.player.transform.position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitDistance;
        float y = GM.I.player.transform.position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitDistance;
        
        // Move toward that position
        Vector3 targetPos = new Vector3(x, y, 0);
        Vector2 direction = (targetPos - transform.position).normalized;
        rb2d.AddForce(direction * acceleration);
        
        // Cap speed
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
        
        // Rotate to face movement direction when not targeting
        if (rb2d.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    
    // Sets this drone's stats
    public void SetStats(int talentLevel = 1)
    {
        // Set attributes based on player
        mind = GM.I.player.mind / 2;
        body = GM.I.player.body / 2;
        soul = 1;
        luck = 0;
        
        /* // Initialize weapon component if needed
        if (weapon == null)
            weapon = new LaserWeapon(transform, this); */
        
        // Set weapon stats
        weapon.SetStats(talentLevel, false);
    }
}