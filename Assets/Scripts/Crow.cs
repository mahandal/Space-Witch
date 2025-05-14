using UnityEngine;

public class Crow : Gatherer
{
    [Header("Crow")]
    public int starDetectionRadius = 8;
    public Star currentTarget;
    public float refreshTargetTime = 1f;
    private float refreshTimer = 0f;
    public float orbitDistance = 3f;
    public float orbitSpeed = 100f;
    private float currentAngle = 0f;
    
    [Header("Relationship")]
    public Player witch; // The witch who summoned this crow
    
    void FixedUpdate()
    {
        // Gatherer shared homeostasis (health/mana handling)
        Homeostasis();

        // Navigation and movement
        Navigate();

        // Gatherer shared late fixed update
        LateFixedUpdate();
    }

    void Navigate()
    {
        // When no target, orbit witch
        if (currentTarget == null)
        {
            OrbitWitch();
        }
        else
        {
            // Move toward target
            MoveTowardTarget();
        }
        
        // Periodically look for new targets
        refreshTimer -= Time.deltaTime;
        if (refreshTimer <= 0)
        {
            //FindFadingStar();
            FindNearestStar();
            refreshTimer = refreshTargetTime;
        }
        
        // Rotate to face movement direction
        if (rb2d.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    
    void OrbitWitch()
    {
        if (witch == null) return;
        
        // Update orbit angle
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;
        
        // Calculate position
        float x = witch.transform.position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitDistance;
        float y = witch.transform.position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitDistance;
        
        // Move toward that position
        Vector3 targetPos = new Vector3(x, y, 0);
        Vector2 direction = (targetPos - transform.position).normalized;
        rb2d.AddForce(direction * acceleration);
        
        // Cap speed
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }
    
    void MoveTowardTarget()
    {
        if (currentTarget == null) return;
        
        // Get direction to target
        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
        
        // Add force
        rb2d.AddForce(direction * acceleration);
        
        // Cap speed
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

    void FindNearestStar()
    {
        // Already have a target
        if (currentTarget != null) return;
        
        // Find all stars in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, starDetectionRadius);
        
        Star bestTarget = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider2D col in colliders)
        {
            Star star = col.GetComponent<Star>();
            if (star != null)
            {
                // Calculate distance to this star
                float distance = Vector3.Distance(transform.position, star.transform.position);
                
                // If this star is closer than the previous best target
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = star;
                }
            }
        }
        
        // Set new target
        currentTarget = bestTarget;
    }
    
    // Crow does what(!?)
    new void OnTriggerEnter2D(Collider2D col)
    {
        // First do our crow business
        Star star = col.GetComponent<Star>();
        if (star != null)
        {
            // Create a new star(?)
            GM.I.spawnManager.SpawnStar(star.transform.position.x, star.transform.position.y, 2f, star.value);
            
            // SFX
            //GM.I.dj.PlayEffect("crow_0", transform.position, 0.1f * star.value);
        }

        // Then call the base class method to handle regular gathering
        base.OnTriggerEnter2D(col);
    }

    public void Death()
    {
        // SFX
        GM.I.dj.PlayEffect("crow_0", transform.position);

        // Clean up
        Object.Destroy(gameObject);
    }

    // Sets this crow's stats
    // Beasts inherit from your familiar
    public void SetStats(int talentLevel = 1)
    {
        // Set attributes based on familiar
        mind = GM.I.familiar.mind / 2;
        body = GM.I.familiar.body / 2;
        soul = GM.I.familiar.soul / 2;
        luck = GM.I.familiar.luck / 2;

        // Scale stats with talent level
        mind *= (int)Mathf.Pow(1.1f, talentLevel);
        body *= (int)Mathf.Pow(1.1f, talentLevel);
        soul *= (int)Mathf.Pow(1.1f, talentLevel);
        luck *= (int)Mathf.Pow(1.1f, talentLevel);
        CalculateStats();

        //newCrow.starDetectionRadius = 8 + talentLevel * 2;
    }
}