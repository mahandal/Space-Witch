using UnityEngine;

public class Beacon : MonoBehaviour
{
    [Header("Beacon")]
    // Which beacon this is.
    // Note: Beacons are 1-indexed!
    public int index = 1; 

    // How strong this beacon's gravitational pull is.
    public float gravityStrength = 5f;

    // How fast beacons spin, multiplied by their index.
    private float baseSpinSpeed = 60f;

    [Header("Dragging")]
    public bool isBeingDragged = false;
    public float dragForce = 5f;
    private Gatherer dragger;

    
    [Header("Machinery")]
    public Rigidbody2D rb2d;

    // misc
    private SpriteRenderer sr;
    
    void Start()
    {
        // Get rigidbody for physics interactions
        rb2d = GetComponent<Rigidbody2D>();

        // Get sprite renderer
        sr = GetComponent<SpriteRenderer>();
    }
    
    void FixedUpdate()
    {
        // Spin like a dying bee
        float spinSpeed = baseSpinSpeed * index;
        transform.Rotate(0, 0, -spinSpeed * Time.deltaTime);

        UpdateColors();

        HandleDragging();
    }


    void HandleDragging()
    {
        // Get picked up?
        if (dragger == null)
        {
            Collider2D gathererCollider = Physics2D.OverlapPoint(transform.position);
            if (gathererCollider != null)
            {
                Gatherer gatherer = gathererCollider.GetComponent<Gatherer>();
                TryPickup(gatherer);
            }
        }

        // Get dragged.
        if (dragger != null && dragger.isCalm)
        {
            isBeingDragged = true;
            Vector2 direction = (dragger.transform.position - transform.position).normalized;
            rb2d.AddForce(direction * dragForce);
        }
        else
        {
            isBeingDragged = false;
            dragger = null;
        }
    }

    void UpdateColors()
    {
        // Pre-game
        if (GM.I.gameState == 0)
        {
            // Pulsing color
            float pulse = Mathf.PingPong(Time.time * 2f, 1f); // 2f controls pulse speed
            sr.color = new Color(1f, 1f, 1f, 0.2f + pulse * 0.6f); // Pulse between 0.2 and 0.8 alpha

            // Max distance
            if (transform.position.magnitude > GM.I.spawnManager.planetMaxDistance)
            {
                // Clamp position
                transform.position = transform.position.normalized * GM.I.spawnManager.planetMaxDistance;

                // Show boundary feedback
                //HitMarker.CreateCombatMarker(transform.position, "X");
            }
        }
        // Game
        else if (GM.I.gameState == 1)
        {
            Color current = sr.color;
            Color target = new Color(0.5f, 0.8f, 1f, 0.3f); // Dim hologram state
            sr.color = Color.Lerp(current, target, Time.deltaTime * 3f); // Fade speed
        }
    }

    // Called each beat by GM
    public static void Beat()
    {
        if (GM.I.gameState < 1) return;
        
        // Go through each beacon
        foreach (Beacon beacon in GM.I.beacons)
        {
            // Set pulse color
            beacon.sr.color = new Color(1f, 0f, 0f, 1f);
        }
    }

    // Enter
    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if a gatherer is grabbing us
        Gatherer gatherer = col.GetComponent<Gatherer>();
        TryPickup(gatherer);

        // Check if a gatherer is grabbing us
        // Gatherer gatherer = col.GetComponent<Gatherer>();
        // if (gatherer != null && dragger == null && gatherer.isCalm)
        // {
        //     // Check if this gatherer is already dragging another beacon
        //     bool alreadyDragging = false;
        //     foreach (Beacon beacon in GM.I.beacons)
        //     {
        //         if (beacon != this && beacon.dragger == gatherer)
        //         {
        //             alreadyDragging = true;
        //             break;
        //         }
        //     }
            
        //     if (!alreadyDragging)
        //     {
        //         dragger = gatherer;
        //     }
        // }

        // Gather stars
        Star star = col.gameObject.GetComponent<Star>();

        if (star != null)
        {
            // Gather star
            Gatherer.starsGathered += star.value;

            // Gain xp!
            GM.I.player.GainXP(star.value);

            // Hit marker
            HitMarker.CreateHitMarker(star.transform.position, star.value);

            // Clean up star object
            Object.Destroy(star.gameObject);
        }
    }

    /*
void OnTriggerEnter2D(Collider2D col)
{
    // Get star
    Star star = col.gameObject.GetComponent<Star>();

    if (star != null)
    {
        // Gather star
        Gatherer.starsGathered += star.value;

        // Hit marker
        HitMarker.CreateHitMarker(star.transform.position, star.value);

        // Clean up star object
        Object.Destroy(star.gameObject);
    }
}
    */

    void OnTriggerExit2D(Collider2D col)
    {
        Gatherer gatherer = col.GetComponent<Gatherer>();
        if (gatherer == dragger)
        {
            dragger = null;
            isBeingDragged = false;
        }
    }

    // 
    public bool TryPickup(Gatherer gatherer)
    {
        if (gatherer != null && dragger == null && gatherer.isCalm)
        {
            // Check if this gatherer is already dragging another beacon
            bool alreadyDragging = false;
            foreach (Beacon beacon in GM.I.beacons)
            {
                if (beacon != this && beacon.dragger == gatherer)
                {
                    alreadyDragging = true;
                    break;
                }
            }
            
            if (!alreadyDragging)
            {
                dragger = gatherer;
                return true;
            }
        }
        return false;
    }
}