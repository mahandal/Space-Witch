using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Stats")]
    public float acceleration = 1f;
    public float size = 1f;
    public float maxSpeed = 1f;
    public float damage = 13f;

    [Header("Facts")]
    public Vector2 direction = Vector2.zero;
    //public float velocity = 0f;
    public float previousFrameVelocity = 0f;

    [Header("Automated Machinery")]
    public bool exploding = false;
    public Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Fixed update is for physiques & stuff.
    void FixedUpdate()
    {
        // Accelerate
        rb2d.AddForce(direction.normalized * acceleration);

        // Max speed
        if(rb2d.linearVelocity.magnitude > maxSpeed)
        {
               rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }

        // Store velocity
        //velocity = rb2d.linearVelocity.magnitude;

        // Max distance
        float distanceToPlayer = Vector3.Distance(transform.position, GM.I.player.transform.position);
        if (distanceToPlayer > GM.I.spawnManager.despawnDistance)
        {
            Explode();
        }

        // Song end
        if (GM.I.songTimer < 1f)
        {
            Explode();
        }

        LateFixedUpdate();
    }

    public void LateFixedUpdate()
    {
        // Store previous frame velocity
        previousFrameVelocity = rb2d.linearVelocity.magnitude;
    }

    public void Explode()
    {
        // - Spawn stars

        // Start with our size
        float numStars = size * previousFrameVelocity + size + previousFrameVelocity;

        // Multiply based on luck?
        float ourLuck = (GM.I.player.luck + GM.I.familiar.luck) / 2;
        if (ourLuck > Random.Range(0, 100))
        {
            numStars *= 2;
        }

        // Spawn stars
        GM.I.spawnManager.SpawnStars(transform.position, (int)numStars, Mathf.Sqrt(size * previousFrameVelocity));

        // SFX
        //GM.I.dj.PlayEffect("asteroid_collision", transform.position);

        // Clean up object
        Object.Destroy(gameObject);
    }

    // An AOE explosion detonating all nearby asteroids.
    // With a range equal to radius and a position to originate from
    public static void Explosion(float radius, Vector3 origin)
    {
        // Get all asteroids within range and Explode them
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(origin, radius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Asteroid asteroid = hitCollider.GetComponent<Asteroid>();
            if (asteroid != null && !asteroid.exploding)
            {
                asteroid.Explode();
            }
        }
    }

    public void ReceiveDamage(float damage, GameObject attacker = null)
    {
        // Spawn stars based on damage
        int numberOfStars = (int)(damage / 20f);
        float spreadRadius = Mathf.Log(damage + 1) * 0.1f; 
        GM.I.spawnManager.SpawnStars(transform.position, numberOfStars, spreadRadius);

        // Reduce size based on damage
        size -= damage / 100f;
        
        // If size is too small, destroy the asteroid
        if (size <= 0.2f)
        {
            Explode();
            return;
        }
        
        // Update scale to match new size
        //transform.localScale = new Vector3(size, size, size);
        float newScale = GM.I.spawnManager.progenitor_Asteroid.transform.localScale.x * size;
        transform.localScale = new Vector3(newScale, newScale, newScale);
        
        // SFX
        GM.I.dj.PlayEffect("asteroid_hit", transform.position);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Check for planet collision
        Planet planet = col.gameObject.GetComponent<Planet>();
        if (planet != null && !exploding)
        {
            // Set exploding
            exploding = true;

            // Calculate damage based on asteroid properties
            float totalDamage = damage * size * previousFrameVelocity;
            
            // Apply damage to planet
            planet.ReceiveDamage(totalDamage);
            
            // Destroy the asteroid
            Explode();
            
            return;
        }
    }
}
