using UnityEngine;

public class MapButton : MonoBehaviour
{
    // Which map is this button for?
    public string mapName;

    // The canvas group for this button, including its image and name.
    public CanvasGroup canvasGroup;

    // On enable.
    void OnEnable()
    {
        HighlightOrFade();
    }

    // On select.
    public void OnSelect()
    {
        // Select if not selected.
        // Deselect if already selected.
        if (MainMenu.I.selectedMaps.Contains(mapName))
            MainMenu.I.selectedMaps.Remove(mapName);
        else
            MainMenu.I.selectedMaps.Add(mapName);

        // Save selected maps.
        MainMenu.I.SaveSelectedMaps();

        // Highlight or fade.
        HighlightOrFade();
    }

    // Highlight if selected, fade if not.
    public void HighlightOrFade()
    {
        if (MainMenu.I.selectedMaps.Contains(mapName))
            Highlight();
        else
            Fade();
    }

    public void Highlight()
    {
        canvasGroup.alpha = 1f;
    }

    public void Fade()
    {
        canvasGroup.alpha = 0.3f;
    }
}
