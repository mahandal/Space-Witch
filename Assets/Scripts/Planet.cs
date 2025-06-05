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

    // How big this planet is mechanically
    private float radius;

    // This planet's initial scale factor
    private float initialScale;

    // How much this planet grows per pollen.
    private float growthFactor = 0.1f;


    [Header("Nectar")]
    public int nectar = 0;
    public float harvestTime = 1f;

    [Header("Orbital Mechanics")]
    public Transform sun; // Reference to the sun
    public float orbitSpeed = 10f; // degrees per second
    private float orbitRadius = 10f; // distance from sun

    private float currentAngle = 0f;
    private Vector3 sunPosition;

    [Header("Machinery")]
    public float harvestTimer = 0f;

    // States
    public bool isAlive = true;

    void Awake()
    {
        radius = GetComponent<CircleCollider2D>().radius;
        initialScale = transform.localScale.x;
        pollen = 13f;

        // Set random starting angle for variety
        currentAngle = Random.Range(0f, 360f);

        // Cache sun position (assuming sun doesn't move)
        if (sun != null)
            sunPosition = sun.position;
        else
            sunPosition = Vector3.zero; // Default to origin if no sun assigned

        // Set orbit radius
        // orbitRadius = transform.position.x;
        orbitRadius = transform.position.y;
    }

    void FixedUpdate()
    {
        // Original planet rotation
        transform.Rotate(0, 0, -0.01f * pollen);
        moonMama.Rotate(0, 0, -1f);

        // Orbital movement
        if (sun != null)
            UpdateOrbit();
    }

    void UpdateOrbit()
    {
        // Update orbital angle (scaled so orbit speeds use whole numbers)
        currentAngle += orbitSpeed / 10f * Time.deltaTime;

        // Check if we've completed a loop
        if (currentAngle >= 360f) 
        {
            // Reset angle
            currentAngle -= 360f;

            // Collect 200$ for passing Go!
            nectar++;
        }

        // Calculate new position around sun
        float x = sunPosition.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitRadius;
        float y = sunPosition.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitRadius;

        // Update position
        transform.position = new Vector3(x, y, 0);
    }

    public void Pollinate(int amount = 1)
    {
        // cap?
        if (pollen >= 13 * GM.I.planets.Count)
            return;

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
        //GM.I.planets.Remove(this);

        // Death!
        isAlive = false;

        // Deactivate
        gameObject.SetActive(false);

        // Not my bee!?
        GM.I.bees[index].gameObject.SetActive(false);

        // Remove from list of active planets
        GM.I.activePlanets.Remove(this);

        // GG?
        if (GM.I.activePlanets.Count == 0)
            GM.I.Lose();

        // GG?
        // if (GM.I.planets.Count == 0)
        //     GM.I.Lose();

        // GG?
        // bool gg = true;
        // foreach (Planet planet in GM.I.planets)
        // {
        //     if (planet.isAlive)
        //         gg = false;
        // }

        // if (gg)
        //     GM.I.Lose();

        // Clean up game object
        //Object.Destroy(gameObject);
    }

    public void Harvest()
    {
        int credits = nectar;
        Gatherer.credits += credits;
        nectar = 0;
        harvestTimer = 0f;

        // Visual feedback
        if (credits == 1)
            HitMarker.CreateHitMarker(transform.position, "+1 credit");
        else
            HitMarker.CreateHitMarker(transform.position, "+" + credits + " credits");

        // Save
        GM.I.SaveGame();
    }
}