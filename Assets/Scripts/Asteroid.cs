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
    public float previousAngularVelocity = 0f;

    [Header("Manual Machinery")]
    public Rigidbody2D rb2d;

    [Header("Automated Machinery")]
    public bool exploding = false;
    public bool isFirstFrame = true;


    // Fixed update is for physiques & stuff.
    void FixedUpdate()
    {

        // First frame special handling
        if (isFirstFrame)
        {
            // Force velocity directly and skip acceleration
            rb2d.linearVelocity = direction.normalized * maxSpeed;
            isFirstFrame = false;

            // Start spinning
            rb2d.angularVelocity = Random.Range(-180f, 180f);

            // Skip the rest of the method for this frame
            LateFixedUpdate();
            return;
        }

        // Accelerate
        rb2d.AddForce(direction.normalized * acceleration);

        // Max speed
        if (rb2d.linearVelocity.magnitude > maxSpeed)
        {
            rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }

        // Store velocity
        //velocity = rb2d.linearVelocity.magnitude;

        // Max distance
        float distanceFromOrigin = Vector3.Distance(transform.position, Vector3.zero);
        if (distanceFromOrigin > 35f)
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
        previousAngularVelocity = rb2d.angularVelocity;
    }

    public void Explode()
    {
        // - Spawn stars

        // Start with our size
        float numStars = size * previousFrameVelocity + size * Mathf.Log(previousAngularVelocity);

        // Multiply based on luck?
        float ourLuck = (GM.I.player.luck + GM.I.familiar.luck) / 2;
        if (ourLuck > Random.Range(0, 100))
        {
            numStars *= 2;
        }

        // Spawn stars
        GM.I.spawnManager.SpawnStars(transform.position, (int)numStars, Mathf.Sqrt(size * previousFrameVelocity));

        // SFX
        GM.I.dj.PlayEffect("asteroid_collision", transform.position);

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

    public void ReceiveDamage(float damageReceived, GameObject attacker = null)
    {
        // Spawn stars based on damage
        int numberOfStars = (int)(damageReceived / 20f);
        float spreadRadius = Mathf.Log(damageReceived + 1) * 0.1f;
        GM.I.spawnManager.SpawnStars(transform.position, numberOfStars, spreadRadius);

        // Reduce size based on damage
        size -= damageReceived / 100f;

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
            //float totalDamage = damage * size * previousFrameVelocity;
            float totalDamage = CalculateDamage();

            // Apply damage to planet
            planet.ReceiveDamage(totalDamage);

            // Destroy the asteroid
            Explode();

            return;
        }
    }

    public float CalculateDamage()
    {
        return damage * size * previousFrameVelocity * Mathf.Log(Mathf.Abs(previousAngularVelocity) + 1);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the other asteroid
        Asteroid otherAsteroid = collision.gameObject.GetComponent<Asteroid>();

        // Check if it's an asteroid and neither is already exploding
        if (otherAsteroid != null && !exploding && !otherAsteroid.exploding)
        {
            // Big bank take little bank.
            if (size > otherAsteroid.size)
            {
                ConsumeAsteroid(otherAsteroid);
            }
        }
    }
    
    void ConsumeAsteroid(Asteroid prey)
    {
        // Mark prey as being consumed
        prey.exploding = true;

        // Roll size efficiency
        //float sizeEfficiency = Random.Range(0f, 1f);
        float sizeEfficiency = Random.Range(0.1f, 0.2f);

        // Gain size proportional to the consumed asteroid
        size += prey.size * sizeEfficiency;
        
        // Update scale
        float newScale = GM.I.spawnManager.progenitor_Asteroid.transform.localScale.x * size;
        transform.localScale = new Vector3(newScale, newScale, newScale);

        // Inherit some velocity from the consumed asteroid
        //rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, prey.rb2d.linearVelocity, 0.3f);

        // Gain prey's speed
        float speedEfficiency = 1f - sizeEfficiency;
        maxSpeed += prey.maxSpeed * speedEfficiency;
        acceleration += prey.acceleration * speedEfficiency;
        rb2d.linearVelocity += prey.rb2d.linearVelocity.magnitude * rb2d.linearVelocity.normalized;
        rb2d.angularVelocity += prey.rb2d.angularVelocity;

        // Explode
        prey.Explode();
    }
}
