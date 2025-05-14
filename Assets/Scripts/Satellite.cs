using UnityEngine;

public class Satellite : Gatherer
{
    [Header("Satellite")]
    public float offsetDistance = 8f; // How far from player to maintain position
    public Vector2 offsetDirection = Vector2.up; // Direction from player
    
    [Header("Weapons")]
    public Transform firePoint;
    public LaserWeapon weapon;
    
    new void Start()
    {
        base.Start();
        
        // Initialize weapon component
        //weapon = new LaserWeapon(transform, this);
        
        // Set initial stats
        SetStats(1);
    }
    
    void FixedUpdate()
    {
        // Gatherer shared homeostasis
        Homeostasis();

        // Weapon systems
        weapon.HandleWeapon();

        // Rotate
        transform.Rotate(0, 0, -0.1f * mind);

        // Gatherer shared late fixed update
        LateFixedUpdate();
    }

    
    // Sets this satellite's stats
    public void SetStats(int talentLevel = 1)
    {
        // Set attributes based on player
        mind = GM.I.player.mind / 3;
        body = GM.I.player.body / 3; // More fragile than drones
        soul = 1;
        luck = 0;
        
        /* // Initialize weapon component if needed
        if (weapon == null)
            weapon = new LaserWeapon(transform, this); */
        
        // Set weapon stats
        weapon.SetStats(talentLevel, true);
    }
}
