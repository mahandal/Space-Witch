using UnityEngine;
using TMPro;

public class ExploreUI : MonoBehaviour
{
    [Header("Credits")]
    // Text object showing the player's credits.
    public TMP_Text playerCredits;

    // + Exploring!
    void Update()
    {
        // Credits
        playerCredits.text = MenuManager.I.saveData.credits.ToString();
    }
}
