using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("Planet")]
    // A hidden parent to rotate a moon, for when this planet has one.
    public Transform moonMama;

    // This planet's index in the list of planets.
    public int index;

    // How much pollen this planet currently has.
    public float pollen = 1;

    // How many stars this planet has stored up.
    public int stars = 0;

    // How big this planet is mechanically
    private float radius;

    // This planet's initial scale factor
    private float initialScale;

    // How much this planet grows per pollen.
    private float growthFactor = 0.1f;

    // How long it takes for this planet to spawn stars
    public float starTime = 1f;

    // Timers
    public float starTimer = 0f;

    void Awake()
    {
        radius = GetComponent<CircleCollider2D>().radius;
        initialScale = transform.localScale.x;
        pollen = 13f;
    }
    
    void FixedUpdate()
    {
        // Timers
        //Timers();

        // Rotate
        transform.Rotate(0,0, -0.01f * pollen);
        moonMama.Rotate(0, 0, -1f);
    }

    // Timers
    void Timers()
    {
        // Spawn
        starTimer -= Time.deltaTime;
        if (starTimer <= 0f)
        {
            // Check if player is close enough to spawn stars
            float distance = Vector3.Distance(transform.position, GM.I.player.transform.position);
            if (distance < radius && stars > 0)
            {
                // Spawn
                float size = Random.Range(1f, Mathf.Sqrt(pollen));
                GM.I.spawnManager.SpawnStar(transform.position.x, transform.position.y, radius, size);
            } else {
                stars++;
            }
            
            // Reset
            starTimer = starTime;
        }
    }

    public void Pollinate(int amount = 1)
    {
        // Increase pollen
        pollen += amount;

        // Set new scale
        UpdateSize();
    }

    // Sets this planet's scale based off its current pollen count
    public void UpdateSize()
    {
        float newScale = initialScale * (pollen * growthFactor);
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    // Receive damage from an asteroid collision
    public void ReceiveDamage(float damage)
    {
        // Reduce pollen based on damage
        pollen -= damage / 100f;
        
        // Death
        if (pollen <= 0)
        {
            Death();
            return;
        }
        
        // Update size
        UpdateSize();
        
        // Create visual feedback
        HitMarker.CreateDamageMarker(transform.position, damage);
        
        // SFX
        GM.I.dj.PlayEffect("asteroid_hit", transform.position);
    }

    public void Death()
    {
        // Remove from list of planets
        GM.I.planets.Remove(this);

        // GG?
        if (GM.I.planets.Count == 0)
            GM.I.Lose();

        // Clean up game object
        Object.Destroy(gameObject);
    }
}
