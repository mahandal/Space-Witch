using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [Header("Manual Machinery")]
    // Reference to owner
    

    [Header("Stats (calculated mostly)")]
    // Weapon properties
    public float fireRange = 15f;
    public float fireRate = 1f;
    public float damage = 10f;
    private float fireTimer = 0f;

    public float chargeTime = 1f;
    public float currentChargeTime = 0f;
    public bool isCharging = false;

    [Header("Automated Machinery")]
    // Our shooter
    public Gatherer owner;

    // Where our shots fire from
    public Transform firePoint;

    // Our current target
    public Asteroid target = null;

    void Awake()
    {
        // Get our shooter
        if (owner == null)
            owner = GetComponent<Gatherer>();

        // Set fire point
        firePoint = owner.firePoint;
    }

    void Start()
    {
        // Initialize for drones
        // Drone drone = GetComponent<Drone>();
        // if (drone != null)
        // {
        //     // fire point
        //     firePoint = drone.firePoint;
        // }

        // // Initialize for satellites
        // Satellite satellite = GetComponent<Satellite>();
        // if (satellite != null)
        // {
        //     // fire point
        //     firePoint = satellite.firePoint;
        // }
    }

    public void HandleWeapon()
    {
        // Decrement fire timer
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;
        
        // Find target
        if (target == null || Vector3.Distance(target.transform.position, transform.position) > fireRange)
            target = FindNearestAsteroid();
        
        // If we have a target and can fire, start charging
        if (target != null && fireTimer <= 0 && !isCharging)
        {
            isCharging = true;
            currentChargeTime = 0f;
        }
        
        // We have a target?
        if (target != null)
        {
            // Ghetto
            // tbd: update
            bool shouldFaceTarget = false;

            Drone drone = GetComponent<Drone>();
            Satellite satellite = GetComponent<Satellite>();
            if (drone != null || satellite != null)
                shouldFaceTarget = true;

            // Face the target?
            if (shouldFaceTarget)
            {
                Vector3 targetDirection = target.transform.position - transform.position;
                float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
                
            
            // If charging, increase charge time
            if (isCharging)
            {
                currentChargeTime += Time.deltaTime;
                
                // Visual feedback for charging (optional)
                if (currentChargeTime <= chargeTime)
                {
                    // Create charge-up effect
                    //CreateChargingEffect(targetDirection, currentChargeTime / chargeTime);
                }
                
                // When fully charged, fire
                if (currentChargeTime >= chargeTime)
                {
                    Fire(target);
                    isCharging = false;
                    fireTimer = 1f / fireRate;
                }
            }
        }
        else
        {
            // No target, reset charging
            isCharging = false;
        }
    }
    
    private Asteroid FindNearestAsteroid()
    {
        // Find all asteroids in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fireRange);
        
        Asteroid bestTarget = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider2D col in colliders)
        {
            Asteroid asteroid = col.GetComponent<Asteroid>();
            if (asteroid != null && !asteroid.exploding)
            {
                // Calculate distance
                float distance = Vector3.Distance(transform.position, asteroid.transform.position);
                
                // If this asteroid is closer than the previous best target
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = asteroid;
                }
            }
        }
        
        return bestTarget;
    }
    
    public void Fire(Asteroid target)
    {
        // Reset fire timer
        fireTimer = 1f / fireRate;
        
        // Deal damage to asteroid
        target.ReceiveDamage(damage);
        
        // Get direction
        Vector3 direction = target.transform.position - transform.position;
        
        // Visual effect of laser/shot
        CreateLaserEffect(direction);
        
        // SFX
        string soundFileName = "laser_" + Random.Range(1,14);
        float soundScalar = Mathf.Min(5f, 1 + damage/100);
        GM.I.dj.PlayEffect(soundFileName, transform.position, soundScalar);
    }
    
    private LineRenderer CreateLaserEffect(Vector3 direction)
    {
        // Create temporary GameObject for laser
        GameObject laserObj = new GameObject("LaserShot");
        //laserObj.transform.position = transform.position;
        laserObj.transform.position = firePoint.position;
        
        // Add LineRenderer component
        LineRenderer laser = laserObj.AddComponent<LineRenderer>();

        // Set material with shader that respects vertex colors
        laser.material = new Material(Shader.Find("Sprites/Default"));
        
        // Configure the laser appearance
        laser.startWidth = 0.01f;
        laser.endWidth = 0.02f;
        laser.startColor = Color.red;
        laser.endColor = Color.red;
        
        // Set positions (from weapon to target)
        float length = Vector3.Distance(firePoint.position, target.transform.position);
        laser.positionCount = 2;
        laser.SetPosition(0, firePoint.position);
        laser.SetPosition(1, firePoint.position + direction.normalized * length);

        // Make laser render above other objects
        laser.sortingOrder = 100;
        
        // Destroy after short duration
        Object.Destroy(laserObj, 0.1f);
        
        return laser;
    }

    public void SetStats(int talentLevel, bool isSatellite = false)
    {
        // Base damage scales with talent level
        float baseDamage = 100f + 10f * talentLevel;
        
        // Mind influences damage (10% per point of mind)
        damage = baseDamage * (1f + (owner.mind * 0.1f));
        
        // Mind could also influence charge time (shorter with higher mind)
        chargeTime = 1f / (1f + (owner.mind * 0.05f));
        
        // Satellites do double damage but take twice as long to charge.
        if (isSatellite)
        {
            damage *= 2f;
            chargeTime *= 2f;
        }
    }
}