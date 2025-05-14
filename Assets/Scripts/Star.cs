using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Star")]
    public float lifeTime = 13f;
    public float timeRemaining = 13f;
    public float value = 1f;

    [Header("Automated Machinery")]
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Get sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize time remaining
        timeRemaining = lifeTime;
    }

    // FixedUpdate is for physics & stuff
    void FixedUpdate()
    {
        // Rotate counter clockwise when sucked into the vacuum
        if (GM.I.familiar.isStaying)
            transform.Rotate(0, 0, GM.I.familiar.stayPower);
        // Normal rotation
        else
            transform.Rotate(0, 0, -1f);

        Gravity();

        FadeOut();
    }

    // Stars are pulled toward your familiar.
    void Gravity()
    {
        // Get current gravity range
        float currentGravityRange = GM.I.familiar.gravityRange;

        // Stay bonus
        if (GM.I.familiar.isStaying)
            currentGravityRange *= GM.I.familiar.stayPower;

        // Get distance
        float distance = (GM.I.familiar.transform.position - transform.position).magnitude;
        
        // Get direction to pull star's transform to familiar
        Vector3 direction = (GM.I.familiar.transform.position - transform.position).normalized;

        // Set default gravity strength
        float currentGravityStrength = GM.I.familiar.gravityMinimum;

        // Check if we're in range for full strength
        if (distance <= currentGravityRange)
        {
            // Full strength
            currentGravityStrength = GM.I.familiar.gravityStrength;
        }
        // Check if we're in range for half strength
        if (distance > currentGravityRange && distance < currentGravityRange * 2)
        {
            // Half strength
            currentGravityStrength = GM.I.familiar.gravityStrength / 2;
        }

        // Stay bonus
        if (GM.I.familiar.isStaying)
            currentGravityStrength *= GM.I.familiar.stayPower;

        // Move the target in the direction multiplied by gravity strength
        transform.position = transform.position + (direction * currentGravityStrength);

        
    }

    // Fade out over time
    protected void FadeOut()
    {
        // Countdown time remaining
        timeRemaining -= Time.deltaTime;

        // Set opacity
        float percentRemaining = timeRemaining / lifeTime;
        spriteRenderer.color = new Color(1f, 1f, 1f, percentRemaining);

        // Check if we're gone
        if (timeRemaining <= 0f)
        {
            // Clean up game object
            Object.Destroy(gameObject);
        }
    }
}
