using UnityEngine;

public class WormHole : MonoBehaviour
{
    [Header("Asteroids")]
    public float spawnRate = 1f; // How many asteroids to spawn per second
    public float asteroidSizeMultiplier = 1f; // Modifier for asteroid size
    public float asteroidSpeedMultiplier = 1f; // Modifier for asteroid speed

    [Header("Anti-Gravity")]
    public float repulsionRange = 8f;
    public float repulsionStrength = 5f;
    public float minimumRepulsion = 0.5f;

    [Header("Machinery")]
    // Timers

    public float spawnTimer = 0f;

    void FixedUpdate()
    {
        // Anti-gravity
        AntiGravity();

        // Rotate
        transform.Rotate(0, 0, spawnRate * (1 + GM.I.hype));
    }

    public void SpawnAsteroid()
    {
        // Spawn asteroid at worm hole position
        GM.I.spawnManager.SpawnAsteroid(transform.position, asteroidSizeMultiplier, asteroidSpeedMultiplier);
    }

    public void AntiGravity()
    {
        // Find all gatherers within range
        // TBD: Optimize!
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, repulsionRange);
        
        foreach (Collider2D col in colliders)
        {
            // Gatherers
            Gatherer gatherer = col.GetComponent<Gatherer>();
            if (gatherer != null)
            {
                // Calculate distance and direction
                float distance = Vector3.Distance(transform.position, gatherer.transform.position);
                Vector3 direction = (gatherer.transform.position - transform.position).normalized;
                
                // Calculate force:

                // - stronger when closer
                float force = minimumRepulsion;
                if (distance <= repulsionRange)
                {
                    // Full strength when close
                    force = repulsionStrength * (1 - (distance / repulsionRange)) + minimumRepulsion;
                }
                
                // Apply force
                gatherer.transform.position += direction * force * Time.deltaTime;
            }

            // Beacons
            Beacon beacon = col.GetComponent<Beacon>();
            if (beacon != null)
            {
                // Calculate distance and direction
                float distance = Vector3.Distance(transform.position, beacon.transform.position);
                Vector3 direction = (beacon.transform.position - transform.position).normalized;
                
                // Calculate force:

                // - stronger when closer
                float force = minimumRepulsion;
                if (distance <= repulsionRange)
                {
                    // Full strength when close
                    force = repulsionStrength * (1 - (distance / repulsionRange)) + minimumRepulsion;
                }

                // beacons are stronger
                force *= 2;
                
                // Apply force
                beacon.transform.position += direction * force * Time.deltaTime;
            }
        }
    }
}