using UnityEngine;

public class WormHole : MonoBehaviour
{
    [Header("Worm Hole")]
    public float spawnRate = 1f; // How many asteroids to spawn per second
    public float asteroidSizeMultiplier = 1f; // Modifier for asteroid size
    public float asteroidSpeedMultiplier = 1f; // Modifier for asteroid speed
    
    private float spawnTimer = 0f;
    
    void FixedUpdate()
    {
        // Only spawn when game is active
        if (GM.I.gameState != 1)
            return;
            
        // Count down timer
        spawnTimer -= Time.deltaTime;
        
        // Spawn asteroid when timer expires
        if (spawnTimer <= 0)
        {
            // Spawn asteroid
            SpawnAsteroid();
            
            // Reset timer (add randomness to make it less predictable)
            spawnTimer = 1f / spawnRate + Random.Range(-0.2f, 0.2f);
        }
        
        // Rotate
        transform.Rotate(0, 0, 1f);
    }
    
    private void SpawnAsteroid()
    {
        // Spawn asteroid at worm hole position
        GM.I.spawnManager.SpawnNewAsteroidFromWormHole(transform.position, asteroidSizeMultiplier, asteroidSpeedMultiplier);
    }
}