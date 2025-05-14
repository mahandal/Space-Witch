using UnityEngine;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    [Header("Potion")]
    // This potion's name.
    // Note: Crucial! Used for some important wiring atm!
    public string myName = "Healing Potion";

    // The strength of this potion.
    // Used for different purposes with different potions.
    public float strength = 1f;

    // How long in total this potion will take to cook.
    public float cookTime = 6f;

    // How much longer this potion has until it's ready.
    public float cookTimer = 0f;

    // Whether this potion is ready to be consumed.
    public bool isReady = false;

    // Whether this potion should be treated like a land mine and detonate on contact with asteroids.
    // (default is false, potions should be drunk by gatherers!)
    public bool isMine = false;

    // This potion's sprite renderer.
    // (aka the object that shows what it looks like)
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cookTimer = cookTime;
        isReady = false;
    }
    void FixedUpdate()
    {
        if (isReady)
        {
            // Rotate
            transform.Rotate(0, 0, -1f);
        } else {
            // Timers
            Timers();

            // Get time elapsed
            float percentTimeElapsed = 1 - (cookTimer / cookTime);

            // Rotate
            transform.Rotate(0,0, 21f * percentTimeElapsed);

            // Set opacity
            float opacity = 0.5f;
            opacity += percentTimeElapsed / 2;
            spriteRenderer.color = new Color(opacity, opacity, opacity, opacity);
        }
        
    }

    void Timers()
    {
        cookTimer -= Time.deltaTime;

        // Ready!
        if (cookTimer <= 0f)
        {
            isReady = true;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    // Potion effects found here!
    public void Drink(Gatherer drinker)
    {
        switch(myName)
        {
            // Healing
            case "Healing Potion":
                drinker.Heal(strength, GM.I.player.gameObject);
                break;
            
            // Mana
            case "Mana Potion":
                drinker.GainMana(strength);
                break;

            /* // Explosion
            case "Explosion Potion":
                Asteroid.Explosion(strength, drinker.transform.position);
                break; */
        }

        // Clean up
        Object.Destroy(gameObject);
    }

    // Handle mine potions
    // Note: Gatherer drinking is handled in Gatherer.cs so this is just for asteroids
    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if it's an asteroid
        Asteroid asteroid = col.gameObject.GetComponent<Asteroid>();
        
        // Are you mine?
        if (asteroid != null && isReady && isMine)
            Splash(asteroid);
    }

    // Splashes this potion onto an asteroid.
    // Used for offensive potions e.g. mines.
    void Splash(Asteroid asteroid)
    {
        // Explosion!
        if (myName == "Explosion Potion")
        {
            // Trigger explosion
            Asteroid.Explosion(strength, transform.position);
            
            // SFX (optional)
            GM.I.dj.PlayEffect("explosion", transform.position, 1f);
            
            // Clean up
            Object.Destroy(gameObject);
        }
    }
}
