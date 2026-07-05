using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The game master!
// Runs Explore mode.
// See DM for running battles.
public class GM : MonoBehaviour
{
    [Header("Player")]
    // The player.
    public Explorer player;

    [Header("Planet")]
    // Which planet we are currently exploring.
    public Planet currentPlanet;

    // Singleton
    public static GM I;

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        if (I == null || I == this)
            I = this;
        else
            Destroy(gameObject);
    }

    // Set up the given planet for the player to explore!
    public void Explore(Planet p)
    {
        // Set current planet.
        currentPlanet = p;

        // Move player into starting position.
        player.transform.position = currentPlanet.exploreStart.position;
    }
}