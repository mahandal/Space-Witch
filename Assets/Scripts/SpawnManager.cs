using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Asteroids")]
    public float baseAsteroidAcceleration = 0.5f;
    public float baseAsteroidMaxSpeed = 0.5f;
    public float baseAsteroidSize = 0.5f;
    public float asteroidAccelerationHypeScalar = 0.05f;
    public float asteroidMaxSpeedHypeScalar = 0.05f;
    public float asteroidSizeHypeScalar = 0.05f;

    [Header("Planet Spawning")]
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
    public Beacon progenitor_Beacon;
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
    public LineRenderer boundaryCircle;


    void Start()
    {
        player = GM.I.player;
        familiar = GM.I.familiar;

        // Create boundary circle
        //CreateBoundaryCircle();
    }

    // Called by GM.GoHome
    public void GoHome()
    {
        // Create boundary circle
        CreateBoundaryCircle();
    }

    void CreateBoundaryCircle()
    {
        GameObject circleObj = new GameObject("BoundaryCircle");
        boundaryCircle = circleObj.AddComponent<LineRenderer>();
        
        // Simple white outline
        boundaryCircle.material = new Material(Shader.Find("Sprites/Default"));
        boundaryCircle.startColor = new Color (0f, 0f, 1f, 0.01f);
        boundaryCircle.endColor = new Color (0f, 0f, 1f, 0.01f);
        boundaryCircle.startWidth = 0.1f;
        boundaryCircle.endWidth = 0.1f;
        boundaryCircle.useWorldSpace = true;
        
        // Draw circle at max planet distance
        int segments = 64;
        boundaryCircle.positionCount = segments + 1;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2f * Mathf.PI / segments;
            float x = Mathf.Cos(angle) * planetMaxDistance;
            float y = Mathf.Sin(angle) * planetMaxDistance;
            boundaryCircle.SetPosition(i, new Vector3(x, y, 0));
        }
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
    }


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
    public void SpawnMoon(Planet previousPlanet = null)
    {
        int index = 0;
    
        if (previousPlanet != null)
        {
            // Find current planet in active list and get next one
            int currentIndex = GM.I.activePlanets.IndexOf(previousPlanet);
            index = (currentIndex + 1) % GM.I.activePlanets.Count;
        }
        
        Planet planet = GM.I.activePlanets[index];

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

    // - Spawn a new cluster of planets
    public void SpawnPlanets(int numberToSpawn)
    {
        List<Planet> planetList = new List<Planet>();

        for (int i = 0; i < numberToSpawn; i++)
        {
            // Spawn planet
            Planet newPlanet = SpawnPlanet();

            // Set planet's index
            newPlanet.index = i;

            // Assign new planet to bee
            GM.I.bees[i].goal = newPlanet;

            // Add to return list
            planetList.Add(newPlanet);
        }

        GM.I.nebula.planets = planetList;
        //GM.I.planets = planetList;
        //GM.I.activePlanets = planetList;

        //return returnList;
    }

    // - Curate down the list of planets by the given number
    public void CuratePlanets(int numCut)
    {
        // Loop numCut times
        for (int i = 0; i < numCut; i++)
        {
            Debug.Log("Cutting a planet...");
            // Cut a random planet
            int randomIndex = Random.Range(0, GM.I.activePlanets.Count);
            Planet cutPlanet = GM.I.activePlanets[randomIndex];
            cutPlanet.Death();
        }
    }

    // Spawns a new planet in a random location.
    // Note: Delegates to SpawnPlanet(float x, float y) to actually spawn the planet.
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
        // GM.I.planets.Add(newPlanet);

        // Activate new planet
        newPlanet.gameObject.SetActive(true);

        // Return!
        return newPlanet;
    }

    // --- Asteroids


    // --- Bees

    public void SetUpBees()
    {
        // Shuffle bees
        ShuffleBees();

        // Activate each planet's bee
        for (int i = 0; i < GM.I.activePlanets.Count; i++)
        {
            Planet planet = GM.I.activePlanets[i];
            Bee bee = GM.I.bees[planet.index]; // Use planet's original index to find its bee
            
            // Set starting goal to this planet's position in the active list
            bee.goalIndex = i;
            bee.goal = planet;

            // Bee stats
            bee.GetStats();
            
            // Activate
            bee.gameObject.SetActive(true);
        }

        // Set up the spinning effect and delays.
        Invoke("SetupBeeDelays", 0.1f);
    }

    private void SetupBeeDelays()
    {
        for (int i = 0; i < GM.I.bees.Count; i++)
        {
            // Override the default state that was set in Start() so they start spinning!
            GM.I.bees[i].currentHealth = 0f;
            GM.I.bees[i].isDying = true;
            //GM.I.bees[i].recoveryTimer = GM.I.bees[i].maxSpeed;
            GM.I.bees[i].deathSpinSpeed = (i % 2 == 0) ? 720f : -720f;
            GM.I.bees[i].deathSpinSpeed *= GM.I.bees[i].acceleration;
        }
    }

    private void ShuffleBees()
    {
        // Shuffle the bees list using Fisher-Yates
        for (int i = 0; i < GM.I.bees.Count; i++)
        {
            int randomIndex = Random.Range(i, GM.I.bees.Count);
            Bee temp = GM.I.bees[i];
            GM.I.bees[i] = GM.I.bees[randomIndex];
            GM.I.bees[randomIndex] = temp;
        }
    }

    // private void ShuffleBeePositions()
    // {
    //     // Get all current bee positions
    //     List<Vector3> positions = new List<Vector3>();
    //     foreach (Bee bee in GM.I.bees)
    //     {
    //         positions.Add(bee.transform.position);
    //     }
        
    //     // Shuffle the positions
    //     for (int i = 0; i < positions.Count; i++)
    //     {
    //         int randomIndex = Random.Range(i, positions.Count);
    //         Vector3 temp = positions[i];
    //         positions[i] = positions[randomIndex];
    //         positions[randomIndex] = temp;
    //     }
        
    //     // Assign shuffled positions back to bees
    //     for (int i = 0; i < GM.I.bees.Count; i++)
    //     {
    //         GM.I.bees[i].transform.position = positions[i];
    //     }
    // }

    // --- Nebulas
    
    // Nebula
    public void SetUpNebula(string nebulaName = "Unknown")
    {
        // Deactivate all nebulas
        DeactivateNebulas();

        // Get the right nebula
        if (nebulaName == "Eldest Ring")
        {
            GM.I.nebula = eldest_ring;
        }
        
        // Set up unknown nebulas special
        if (nebulaName == "Unknown")
        {
            // Unknown nebula
            GM.I.nebula = unknown;
            GM.I.nebula.myName = "Unknown";

            // Generate a dozen planets
            // GM.I.nebula.planets = SpawnPlanets(GM.I.bees.Count);
            SpawnPlanets(GM.I.bees.Count);

            // Track planets
            SetUpPlanets(GM.I.nebula.planets);

            // Curate down to however many we want
            CuratePlanets(Random.Range(0, 9));

            // Spawn beacons
            SpawnBeacons(Random.Range(3, 7));

            // Generate new worm hole
            //SpawnWormHoles(1);
            GM.I.nebula.wormHoles = new List<WormHole>();
            GM.I.nebula.wormHoles.Add(SpawnWormHole(0, 0));
        }
        // Daily nebula
        else if (nebulaName == "Daily")
        {
            // New day begins at 4:20am PST.
            // For the whole world, so we can all dance together.
            System.DateTime utcNow = System.DateTime.UtcNow;
            System.DateTime adjustedTime = utcNow.AddHours(-12).AddMinutes(-20); // Subtract 12:20 to offset for 4:20am PST
            System.DateTime seedDate = adjustedTime.Date;
            
            int dailySeed = seedDate.Year * 10000 + seedDate.Month * 100 + seedDate.Day;

            // Set seed based on today's date
            // System.DateTime today = System.DateTime.Today;
            // int dailySeed = today.Year * 10000 + today.Month * 100 + today.Day;
            Random.InitState(dailySeed);
            
            // Generate deterministic nebula
            GM.I.nebula = unknown;
            GM.I.nebula.myName = "Daily";

            // Spawn planets
            SpawnPlanets(GM.I.bees.Count);

            // Track planets
            SetUpPlanets(GM.I.nebula.planets);

            // Curate down to however many we want
            CuratePlanets(Random.Range(0, 9));

            // Spawn beacons
            SpawnBeacons(Random.Range(3, 7));

            // Spawn worm hole
            GM.I.nebula.wormHoles = new List<WormHole>();
            GM.I.nebula.wormHoles.Add(SpawnWormHole(0, 0));
        } else {
            // Set up known nebulas

            // tbd: other nebulas
            SpawnBeacons(5);

            // Track planets
            SetUpPlanets(GM.I.nebula.planets);
        }

        // Activate nebula
        GM.I.nebula.gameObject.SetActive(true);

        // Track planets
        //SetUpPlanets(GM.I.nebula.planets);

        // Track worm holes
        SetUpWormHoles(GM.I.nebula.wormHoles);

        // Spawn Moon
        //Gatherer.moonsGathered = 1;
        //SpawnMoon();

        // Spawn beacons
        // SpawnBeacons(Random.Range(2, 7));

        // Create preview of bee path
        CreateBeePath();
    }

    // --- Beacons

    public void SpawnBeacons(int count)
    {
        GM.I.beacons.Clear();
        
        for (int i = 1; i <= count; i++)
        {
            // Spawn between planets and origin
            float distance = planetMinDistance * 1f;
            float angle = (360f / count) * i; // Spread them evenly
            
            // Calculate position
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
            
            // Instantiate
            Beacon newBeacon = Object.Instantiate(progenitor_Beacon, GM.I.universe);

            // Set position
            newBeacon.transform.position = new Vector3(x, y, 0);

            // Set index
            newBeacon.index = i;
            
            // Track beacon
            GM.I.beacons.Add(newBeacon);

            // Set tooltip name
            Tooltip tooltip = newBeacon.GetComponent<Tooltip>();
            tooltip.myName = "Beacon " + i;

            // Activate
            newBeacon.gameObject.SetActive(true);
        }

        CreateAsteroidPath();
    }

    // Set up worm holes from a nebula
    public void SetUpWormHoles(List<WormHole> wormHoles)
    {
        // Set GM's list of worm holes
        GM.I.wormHoles = wormHoles;
    }

    // Initialize a list of planets for this game.
    public void SetUpPlanets(List<Planet> planets)
    {
        // Set GM's list of planets
        GM.I.planets = planets;
        GM.I.activePlanets = planets;

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
        
        // Add to GM's list
        GM.I.wormHoles.Add(newWormHole);
        
        // Activate
        newWormHole.gameObject.SetActive(true);
        
        return newWormHole;
    }

    // Spawn an asteroid.
    // Generally from a worm hole?
    public void SpawnAsteroid(Vector3 position, float sizeMultiplier, float speedMultiplier)
    {
        // Instantiate new asteroid
        Asteroid newAsteroid = Object.Instantiate(progenitor_Asteroid, GM.I.universe);

        // Set position
        newAsteroid.transform.position = position;

        // Set initial target to first beacon
        if (GM.I.beacons.Count > 0)
        {
            Beacon firstBeacon = GM.I.beacons[0];
            newAsteroid.direction = (firstBeacon.transform.position - position).normalized;
            newAsteroid.currentBeaconIndex = 1;
        }
        else
        {
            // No beacons, target planets directly
            int randomIndex = Random.Range(0, GM.I.activePlanets.Count);
            Planet targetPlanet = GM.I.activePlanets[randomIndex];
            newAsteroid.direction = (targetPlanet.transform.position - position).normalized;
        }

        // Set speed & size

        // Base values scale with hype & difficulty
        float[] sizeMultipliers = {0.5f, 1.0f, 2f};
        float[] speedMultipliers = {0.5f, 1.0f, 2f};

        float size = baseAsteroidSize * sizeMultipliers[GM.I.difficulty.asteroidSize];
        float maxSpeed = baseAsteroidMaxSpeed * speedMultipliers[GM.I.difficulty.asteroidSpeed];
        float acceleration = maxSpeed;
        // float acceleration = baseAsteroidAcceleration + (asteroidAccelerationHypeScalar * GM.I.hype);
        // float maxSpeed = baseAsteroidMaxSpeed + (asteroidMaxSpeedHypeScalar * GM.I.hype);
        // float size = baseAsteroidSize + (asteroidSizeHypeScalar * GM.I.hype);

        // Add some random variation
        float randomVariation = Random.Range(0.1f, 2.9f);

        // Get inverse of random variation, so speed and size will scale inverse to each other.
        float inverseVariation = 3f - randomVariation;

        // Set speed with controlled scaling
        newAsteroid.acceleration = acceleration * speedMultiplier * randomVariation;
        newAsteroid.maxSpeed = maxSpeed * speedMultiplier * randomVariation;


        // Start at max speed
        newAsteroid.rb2d.linearVelocity = newAsteroid.direction * newAsteroid.maxSpeed;

        // Set size
        newAsteroid.size = size * sizeMultiplier * inverseVariation;
        float newScale = progenitor_Asteroid.transform.localScale.x * newAsteroid.size;
        newAsteroid.transform.localScale = new Vector3(newScale, newScale, newScale);

        // Activate
        newAsteroid.gameObject.SetActive(true);
    }

    [Header("Paths")]
    public LineRenderer beePath;
    public LineRenderer asteroidPath;

    // Show the path bees will take.
    void CreateBeePath()
    {
        GameObject pathObj = new GameObject("BeePath");
        beePath = pathObj.AddComponent<LineRenderer>();
        
        // Yellow line
        beePath.material = new Material(Shader.Find("Sprites/Default"));
        beePath.startColor = new Color(0f, 1f, 0f, 0.02f);
        beePath.endColor = new Color(0f, 1f, 0f, 0.02f);
        beePath.startWidth = 0.02f;
        beePath.endWidth = 0.02f;
        beePath.useWorldSpace = true;

        
        // Connect all planets in a loop
        beePath.positionCount = GM.I.activePlanets.Count + 1;
        
        for (int i = 0; i < GM.I.activePlanets.Count; i++)
        {
            beePath.SetPosition(i, GM.I.activePlanets[i].transform.position);
        }
        
        // Close the loop - back to first planet
        beePath.SetPosition(GM.I.activePlanets.Count, GM.I.activePlanets[0].transform.position);
    }

    public void UpdateBeePath()
    {
        if (beePath == null || GM.I.activePlanets.Count == 0) return;
        
        // Connect all living planets in a loop
        beePath.positionCount = GM.I.activePlanets.Count + 1;
        
        for (int i = 0; i < GM.I.activePlanets.Count; i++)
        {
            beePath.SetPosition(i, GM.I.activePlanets[i].transform.position);
        }
        
        // Close the loop - back to first planet
        beePath.SetPosition(GM.I.activePlanets.Count, GM.I.activePlanets[0].transform.position);
    }

    // Create the path asteroids will take.
    void CreateAsteroidPath()
    {
        GameObject pathObj = new GameObject("AsteroidPath");
        asteroidPath = pathObj.AddComponent<LineRenderer>();
        
        // Red line
        asteroidPath.material = new Material(Shader.Find("Sprites/Default"));
        asteroidPath.startColor = new Color(1f, 0f, 0f, 0.1f);
        asteroidPath.endColor = new Color(1f, 0f, 0f, 0.01f);
        asteroidPath.startWidth = 0.05f;
        asteroidPath.endWidth = 0.01f;
        asteroidPath.useWorldSpace = true;
        
        // Set points: origin -> beacon 1 -> beacon 2 -> etc
        asteroidPath.positionCount = GM.I.beacons.Count + 1;
        
        // Start at origin
        asteroidPath.SetPosition(0, Vector3.zero);
        
        // Connect each beacon in order
        for (int i = 0; i < GM.I.beacons.Count; i++)
        {
            asteroidPath.SetPosition(i + 1, GM.I.beacons[i].transform.position);
        }
    }

    public void UpdateAsteroidPath()
    {
        if (asteroidPath == null) return;
        
        // Origin -> beacons
        int totalPoints = 1 + GM.I.beacons.Count;
        asteroidPath.positionCount = totalPoints;
        
        int index = 0;
        
        // Start at origin
        asteroidPath.SetPosition(index++, Vector3.zero);
        
        // Through each beacon
        for (int i = 0; i < GM.I.beacons.Count; i++)
        {
            asteroidPath.SetPosition(index++, GM.I.beacons[i].transform.position);
        }
    }
}
