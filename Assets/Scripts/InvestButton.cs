using UnityEngine;

public class InvestButton : MonoBehaviour
{
    public string industry;

    // Attempt to invest in a planet.
    // Note: Delegates largely to Planet.Invest()
    public void Invest()
    {
        // Get planet.
        Planet planet = GM.I.player.currentPlanet;

        // Delegate.
        planet.Invest(industry);
    }
}
