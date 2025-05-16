using UnityEngine;
using System.Collections.Generic;

public class Witch : Gatherer
{
    [Header("Talents")]
    public List<string> teachableTalents = new List<string>();
    public int baseTalentCost = 100;

    [Header("Automated Machinery")]
    public Vector3 destination = Vector3.zero;
    

    // Super collision to train 
    new void OnCollisionEnter2D(Collision2D col)
    {
        // Base gatherer collision handling
        base.OnCollisionEnter2D(col);
        
        // Check if player
        if (col.gameObject.GetComponent<Player>() != null)
        {
            BeginTraining();
        }
    }

    public void BeginTraining()
    {
        // SFX
        string soundFileName = "witch_" + Random.Range(1, 14);
        GM.I.dj.PlayEffect(soundFileName, transform.position);

        // Set current witch
        GM.I.currentWitch = this;

        WheelChoice.LoadWheelOfTraining(this);
        
        // Open talent wheel
        GM.I.ui.OpenTrainingScreen();
    }

    new void Start()
    {
        // Gatherer start
        base.Start();

        // Start looking for ourself
        destination = transform.position;
    }

    // Fixed Update handles physics & stuf
    void FixedUpdate()
    {
        // Gatherer shared homeostasis
        Homeostasis();

        // Navigation
        Navigate();
        
        // Familiar's basic movement
        BasicMovement();

        // Gatherer shared late fixed update
        LateFixedUpdate();
    }

    public void Navigate()
    {
        // Check if close enough to destination to pick a new one
        float distToDestination = Vector3.Distance(transform.position, destination);
        if (distToDestination < 0.5f) // Adjust this threshold as needed
        {
            // Pick a new random destination nearby
            float randomRadius = Random.Range(1f, 2f); // Random distance from current position
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Random direction
            
            // Calculate new position
            float newX = transform.position.x + randomRadius * Mathf.Cos(randomAngle);
            float newY = transform.position.y + randomRadius * Mathf.Sin(randomAngle);
            
            // Set new destination
            destination = new Vector3(newX, newY, 0);
        }
    }

    // Accelerate toward our current destination, capped by max speed.
    public void BasicMovement()
    {
        // - Move toward destination

        // Get direction in 3d cause Unity is rude to me :(
        Vector3 direction = (destination - transform.position).normalized;

        // Convert 3d vector back to 2d because Unity does understand that 2D games exist, they just hate them
        Vector2 dir = new Vector2(direction.x, direction.y);

        // Accelerate toward dir
        rb2d.AddForce(dir * acceleration);


        if (rb2d.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        
        // Max speed
        if(rb2d.linearVelocity.magnitude > maxSpeed)
        {
               rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
        }
    }

}