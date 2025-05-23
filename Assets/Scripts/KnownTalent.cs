using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KnownTalent : MonoBehaviour
{
    [Header("Known Talent")]
    public int id;
    public string currentTalent;
    public TMP_Text levelText;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    // Load a talent by name
    public void LoadTalent(string talentName)
    {
        // Set current talent
        currentTalent = talentName;

        // Make sure we have our image
        if (image == null)
            image = GetComponent<Image>();
            
        // Load image
        Utility.LoadImage(image, talentName);

        // Get level
        int level = 1;
        if (GM.I.nebula.myName != "Home")
            level = GM.I.player.talents[talentName];

        // Set level
        levelText.text = "Level: " + level.ToString();
    }

    // When a choice is hovered, load its details.
    public void Hover()
    {
        // Null check
        if (!GM.I.talents.ContainsKey(currentTalent))
        {
            Debug.LogWarning("Error! NULL talent!");
            return;
        }

        // Get talent
        Talent talent = GM.I.talents[currentTalent];

        // Load the details of the talent
        GM.I.ui.LoadDetails(talent);

        // Set image opacity
        image.color = new Color(1, 1, 1, 0.5f);
    }

    public void Unhover()
    {
        // Set image opacity
        image.color = new Color(1, 1, 1, 1);
    }
}
