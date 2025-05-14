using UnityEngine;
using System.Collections.Generic;

public class Nebula : MonoBehaviour
{
    // List of planets, to be fed to GM on game start.
    // Note: Must be wired manually for each nebula! (except unknown)
    public List<Planet> planets;
}
