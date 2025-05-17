using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Stats")]
    // Planets
    public float planetMinDistance;
    public float planetMaxDistance;
    public float despawnDistance;

    [Header("Manual Machinery")]

    // --- Nebulas
    public Nebula unknown;
    public Nebula eldest_ring;

    // --- Progenitors

    // - Misc
    public HitMarker progenitor_HitMarker;

    // - Universe
    public Star progenitor_Star;
    public Planet progenitor_Planet;
    public Moon progenitor_Moon;
    public Asteroid progenitor_Asteroid;
    public WormHole progenitor_WormHole;
    
    // - Potions
    public Potion progenitor_HealingPotion;
    public Potion progenitor_ManaPotion;
    public Potion progenitor_ExplosionPotion;

    // - Beasts
    public Crow progenitor_Crow;


    // - Machines
    // Tbd : fix to laser_drone? or smth
    public Drone progenitor_Drone;
    public Satellite laser_satellite;

    [Header("Automated Machinery")]
    public float spawnTimer = 1f;

    private Player player;
    private Familiar familiar;

    void Start()
    {
        player = GM.I.player;
        familiar = GM.I.familiar;
    }

    void FixedUpdate()
    {
        Timers();
    }

    void Timers()
    {
        // Wait for game to start
        if (GM.I.gameState != 1)
            return;

        // Pause spawning while leveling up
        if (!GM.I.universe.gameObject.activeSelf)
            return;
        
        // - Spawn
        /* spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            // Spawn new set of asteroids;
            SpawnAsteroids(GM.I.intensity);

            // Reset spawnTimer
            spawnTimer = 1f;
        } */
    }

/*
    // Spawn stars around the given coordinates
    public void SpawnStars(Vector3 coordinates, int numberToSpawn, float offsetLimit = 1f)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            SpawnStar(coordinates.x, coordinates.y, offsetLimit, 1);
        }
    }
*/

    public void SpawnStars(Vector3 coordinates, int numberToSpawn, float offsetLimit = 1f)
    {
        // For small batches, keep it simple
        if (numberToSpawn <= 3)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                SpawnStar(coordinates.x, coordinates.y, offsetLimit, 1);
            }
            return;
        }

        // Total star value to maintain
        int remainingValue = numberToSpawn;
        
        // Probability increases with more stars to spawn
        if (Random.value < Mathf.Min(0.7f, numberToSpawn / 10f))
        {
            // How many large stars to create
            int maxLargeStars = Mathf.Min(3, numberToSpawn / 4);
            int numLargeStars = Random.Range(1, maxLargeStars + 1);
            
            for (int i = 0; i < numLargeStars && remainingValue > 3; i++)
            {
                // Values between 2-5, but never more than half remaining
                int starValue = Random.Range(2, Mathf.Min(6, remainingValue / 2 + 1));
                
                // Bigger stars get wider spread
                float starSpread = offsetLimit * starValue;
                SpawnStar(coordinates.x, coordinates.y, starSpread, starValue);
                remainingValue -= starValue;
            }
        }
        
        // Spawn remaining value as normal stars
        for (int i = 0; i < remainingValue; i++)
        {
            SpawnStar(coordinates.x, coordinates.y, offsetLimit, 1);
        }
    }

    // Spawn a new star at the given coordinates offset randomly up to the provided limit
    public void SpawnStar(float x, float y, float offsetLimit, float value = 1f)
    {
        // Set x position randomly
        x = x + Random.Range(-offsetLimit, offsetLimit);

        // Set y position randomly
        y = y + Random.Range(-offsetLimit, offsetLimit);

        // Spawn new star
        SpawnStar(x, y, value);
    }

    // - Spawn a new star at the given location
    public void SpawnStar(float x, float y, float value = 1f)
    {
        // Initialize desired position
        Vector3 desiredPosition = new Vector3(x, y, 0);
            
        // Instantiate new star
        Star newStar = Object.Instantiate(progenitor_Star, GM.I.universe);

        // Set new star's position
        newStar.transform.position = new Vector3(x, y, 0);

        // Set new star's value
        //newStar.value = Random.Range(1f, 5f);
        newStar.value = value;

        // Set new star's size
        float size = progenitor_Star.transform.localScale.x * newStar.value;
        newStar.transform.localScale = new Vector3(size, size, size);

        // Activate new star
        newStar.gameObject.SetActive(true);
    }

    // --- Moons

    // - Spawn a new moon
    public void SpawnMoon(int index = 0)
    {
        // Pick a random planet
        //int index = Random.Range(0, planets.Count);
        //Planet planet = planets[index];

        // Choose the next planet
        if (index >= GM.I.planets.Count)
            index = 0;
        
        Planet planet = GM.I.planets[index];

        // Instantiate a new moon
        Moon newMoon = Object.Instantiate(progenitor_Moon, planet.moonMama);

        // Set its planet
        newMoon.planet = planet;

        // Set its position
        newMoon.transform.localPosition = new Vector3(0, 10f, 0);

        // Activate
        newMoon.gameObject.SetActive(true);
    }

    // --- Planets
    public List<Planet> SpawnPlanets(int numberToSpawn)
    {
        List<Planet> returnList = new List<Planet>();

        for (int i = 0; i < numberToSpawn; i++)
        {
            // Spawn planet
            Planet newPlanet = SpawnPlanet();

            // Set planet's index
            newPlanet.index = i;

            // Assign new planet to bee
            GM.I.bees[i].goal = newPlanet;

            // Add to return list
            returnList.Add(newPlanet);
        }

        return returnList;
    }

    // Spawns a new planet in a random location.
    // Note: Delegates to SpawnPlanet(float x, float y), largely to help me understand the recursion.
    public Planet SpawnPlanet()
    {
        /* // Set x position randomly
        float x = Random.Range(-planetMaxDistance, planetMaxDistance);

        // Set y position randomly
        float y = Random.Range(-planetMaxDistance, planetMaxDistance); */

        // Generate random angle
        float angle = Random.Range(0f, 2f * Mathf.PI);

        // For uniform distribution in a circle, we need to take square root of the random value
        float randomValue = Random.value; // Returns value between 0 and 1
        float radius = planetMinDistance + (planetMaxDistance - planetMinDistance) * Mathf.Sqrt(randomValue);


        // Convert to Cartesian coordinates
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        // Spawn new planet
        return SpawnPlanet(x, y);
    }

    // Spawns a new planet at the given coordinates.
    public Planet SpawnPlanet(float x, float y)
    {
        // Initialize desired position
        Vector3 desiredPosition = new Vector3(x, y, 0);
        
        // TBD: Avoid spawning planets near each other?

        // Instantiate new planet
        Planet newPlanet = Object.Instantiate(progenitor_Planet, GM.I.universe);

        // Set new planet's position
        newPlanet.transform.position = new Vector3(x, y, 0);

        // Set new planet's size
        //float size = progenitor_Planet.transform.localScale.x * Random.Range(0.1f, 1f);
        //newPlanet.transform.localScale = new Vector3(size, size, size);

        // Document new planet
        GM.I.planets.Add(newPlanet);

        // Activate new planet
        newPlanet.gameObject.SetActive(true);

        // Return!
        return newPlanet;
    }

    // --- Asteroids

    // - Spawn a new asteroid at the given location
    /* void SpawnNewAsteroid(float x, float y)
    {
        // Initialize desired position
        Vector3 desiredPosition = new Vector3(x, y, 0);
        

        // Instantiate new asteroid
        Asteroid newAsteroid = Object.Instantiate(progenitor_Asteroid, GM.I.universe);

        // Set new asteroid's position
        newAsteroid.transform.position = desiredPosition;

        // Set new asteroid's direction
        newAsteroid.direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        // Set new asteroid's speed
        newAsteroid.acceleration = Random.Range(0.5f, Mathf.Sqrt(GM.I.intensity));
        newAsteroid.maxSpeed = Random.Range(0.5f, Mathf.Sqrt(GM.I.intensity));

        // Set new asteroid's size
        newAsteroid.size = Random.Range(0.5f, Mathf.Sqrt(GM.I.intensity));
        float newScale = progenitor_Asteroid.transform.localScale.x * newAsteroid.size;
        newAsteroid.transform.localScale = new Vector3(newScale, newScale, newScale);

        // Set new asteroid's damage
        //newAsteroid.damage = progenitor_Asteroid.damage * newAsteroid.acceleration * newAsteroid.size;

        // Activate new asteroid
        newAsteroid.gameObject.SetActive(true);
    } */

/*
    public void SetUpBees()
    {
        // Loop through each bee
        for (int i = 0; i < GM.I.bees.Count; i++)
        {
            // Get bee
            Bee bee = GM.I.bees[i];

            Invoke("SetupBeeDelays", 0.1f);
        }

        ShuffleBeePositions();
    }
*/

    public void SetUpBees()
    {
        // First, categorize planets by quadrant
        Dictionary<int, List<Planet>> planetsByQuadrant = new Dictionary<int, List<Planet>>();
        // Initialize dictionary with 4 quadrants
        for (int i = 0; i < 4; i++)
        {
            planetsByQuadrant[i] = new List<Planet>();
        }

        // Sort planets into quadrants
        foreach (Planet planet in GM.I.planets)
        {
            int quadrant = GetQuadrant(planet.transform.position);
            planetsByQuadrant[quadrant].Add(planet);
        }

        // Assign planets to bees based on quadrant
        for (int i = 0; i < GM.I.bees.Count; i++)
        {
            Bee bee = GM.I.bees[i];
            int beeQuadrant = GetQuadrant(bee.transform.position);
            
            // Get a planet from the bee's quadrant if available
            if (planetsByQuadrant[beeQuadrant].Count > 0)
            {
                // Use the first planet in this quadrant
                bee.goal = planetsByQuadrant[beeQuadrant][0];
                bee.goalIndex = GM.I.planets.IndexOf(bee.goal);
                
                // Remove the planet from the available list
                planetsByQuadrant[beeQuadrant].RemoveAt(0);
            }
            else
            {
                // Fallback: use any available planet
                foreach (var quadrant in planetsByQuadrant.Keys)
                {
                    if (planetsByQuadrant[quadrant].Count > 0)
                    {
                        bee.goal = planetsByQuadrant[quadrant][0];
                        bee.goalIndex = GM.I.planets.IndexOf(bee.goal);
                        planetsByQuadrant[quadrant].RemoveAt(0);
                        break;
                    }
                }
            }
        }

        // Set up the spinning effect and delays
        Invoke("SetupBeeDelays", 0.1f);
    }

    // Helper method to determine quadrant (0: ++, 1: -+, 2: --, 3: +-)
    public int GetQuadrant(Vector3 position)
    {
        if (position.x >= 0 && position.y >= 0) return 0; // Quadrant 1 (++)
        if (position.x < 0 && position.y >= 0) return 1;  // Quadrant 2 (-+)
        if (position.x < 0 && position.y < 0) return 2;   // Quadrant 3 (--)
        return 3;                                         // Quadrant 4 (+-)
    }

    private void SetupBeeDelays()
    {
        for (int i = 0; i < GM.I.bees.Count; i++)
        {
            // Override the default state that was set in Start()
            GM.I.bees[i].isDying = true;
            GM.I.bees[i].recoveryTimer = GM.I.bees[i].maxSpeed;
            GM.I.bees[i].deathSpinSpeed = (i % 2 == 0) ? 720f : -720f;
            GM.I.bees[i].deathSpinSpeed *= GM.I.bees[i].acceleration;
        }
    }

    private void ShuffleBeePositions()
    {
        // Get all current bee positions
        List<Vector3> positions = new List<Vector3>();
        foreach (Bee bee in GM.I.bees)
        {
            positions.Add(bee.transform.position);
        }
        
        // Shuffle the positions
        for (int i = 0; i < positions.Count; i++)
        {
            int randomIndex = Random.Range(i, positions.Count);
            Vector3 temp = positions[i];
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }
        
        // Assign shuffled positions back to bees
        for (int i = 0; i < GM.I.bees.Count; i++)
        {
            GM.I.bees[i].transform.position = positions[i];
        }
    }

    // 
    public void SetUpNebula(string nebulaName = "Unknown")
    {
        // First deactivate all nebulas
        DeactivateNebulas();

        // Then get the right nebula
        if (nebulaName == "Eldest Ring")
        {
            GM.I.nebula = eldest_ring;
        }
        else
        {
            // Unknown nebula
            GM.I.nebula = unknown;

            // Generate new random arrangement of planets
            GM.I.nebula.planets = SpawnPlanets(GM.I.bees.Count);
            
            // Generate new worm hole
            //SpawnWormHoles(1);
            GM.I.nebula.wormHoles = new List<WormHole>();
            GM.I.nebula.wormHoles.Add(SpawnWormHole(0,0));
        }

        // Activate
        GM.I.nebula.gameObject.SetActive(true);
        
        // Track planets
        SetUpPlanets(GM.I.nebula.planets);
        
        // Track worm holes
        SetUpWormHoles(GM.I.nebula.wormHoles);

        // Spawn Moon
        Gatherer.moonsGathered = 1;
        SpawnMoon();
    }

    // Set up worm holes from a nebula
    public void SetUpWormHoles(List<WormHole> wormHoles)
    {
        // Set GM's list of worm holes
        GM.I.wormHoles = wormHoles;
        
        // Set up each worm hole
        for (int i = 0; i < wormHoles.Count; i++)
        {
            // Set customized properties here if needed
            //wormHoles[i].spawnRate = 0.2f + (GM.I.intensity * 0.1f);
        }
    }

    // Initialize a list of planets for this game.
    public void SetUpPlanets(List<Planet> planets)
    {
        // Set GM's list of planets
        GM.I.planets = planets;

        // Set up each planet
        for (int i = 0; i < planets.Count; i++)
        {
            // Set planet's index
            planets[i].index = i;

            // Set planet's size
            planets[i].UpdateSize();
        }
    }

    // Deactivate all nebulas
    public void DeactivateNebulas()
    {
        GM.I.home.gameObject.SetActive(false);
        unknown.gameObject.SetActive(false);
        eldest_ring.gameObject.SetActive(false);
    }

    public void SpawnWormHoles(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            // Spawn worm hole at a distance from the origin(?)
            float distance = Random.Range(planetMinDistance, planetMaxDistance);
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;
            
            SpawnWormHole(x, y);
        }
    }

    public WormHole SpawnWormHole(float x, float y)
    {
        // Initialize desired position
        Vector3 desiredPosition = new Vector3(x, y, 0);
        
        // Instantiate new worm hole
        WormHole newWormHole = Object.Instantiate(progenitor_WormHole, GM.I.universe);
        
        // Set new worm hole's position
        newWormHole.transform.position = desiredPosition;
        
        // Set properties (can be randomized)
        //newWormHole.spawnRate = Random.Range(0.2f, 0.5f) * GM.I.intensity;
        //newWormHole.asteroidSizeMultiplier = Random.Range(0.8f, 1.2f);
        //newWormHole.asteroidSpeedMultiplier = Random.Range(0.8f, 1.2f);
        
        // Add to GM's list
        GM.I.wormHoles.Add(newWormHole);
        
        // Activate
        newWormHole.gameObject.SetActive(true);
        
        return newWormHole;
    }

    // Spawn asteroid from a worm hole
    public void SpawnNewAsteroidFromWormHole(Vector3 position, float sizeMultiplier, float speedMultiplier)
    {
        // Instantiate new asteroid
        Asteroid newAsteroid = Object.Instantiate(progenitor_Asteroid, GM.I.universe);
        
        // Set position
        newAsteroid.transform.position = position;

        // Choose a random planet as target
        int randomIndex = Random.Range(0, GM.I.planets.Count + 1);

        // Target player
        if (randomIndex == GM.I.planets.Count)
        {
            newAsteroid.direction = (GM.I.player.transform.position - position).normalized;
        }
        else
        {
            // Target planet
            Planet targetPlanet = GM.I.planets[randomIndex];
            newAsteroid.direction = (targetPlanet.transform.position - position).normalized;
        }
        
        // Set speed & size

        // Base values that scale directly with intensity
        float baseAcceleration = 0.5f + (0.07f * GM.I.intensity);
        float baseSpeed = 0.5f + (0.07f * GM.I.intensity);
        float baseSize = 0.5f + (0.13f * GM.I.intensity);

        // Add some random variation (±50%)
        float randomVariation = Random.Range(0.5f, 1.5f);

        // Get inverse of random variation, so speed and size will scale inverse to each other.
        float inverseVariation = 2f - randomVariation;

        // Set speed with controlled scaling
        newAsteroid.acceleration = baseAcceleration * speedMultiplier * randomVariation;
        newAsteroid.maxSpeed = baseSpeed * speedMultiplier * randomVariation;


        // Start at max speed
        newAsteroid.rb2d.linearVelocity = newAsteroid.direction * newAsteroid.maxSpeed;
        
        // Set size
        newAsteroid.size = baseSize * sizeMultiplier * inverseVariation;
        float newScale = progenitor_Asteroid.transform.localScale.x * newAsteroid.size;
        newAsteroid.transform.localScale = new Vector3(newScale, newScale, newScale);
        
        // Activate
        newAsteroid.gameObject.SetActive(true);
    }
}
