using UnityEngine;
using System.Collections.Generic;

// Granga's Guide to the Galaxy
public partial class GGG : MonoBehaviour
{
    // Singleton
    public static GGG I;

    // Parent of each chapter's GuidePage.
    public Transform chapterParent;

    // Dictionary mapping chapter names to their GuidePage.
    [System.NonSerialized]
    public Dictionary<string, GuidePage> chapters = new Dictionary<string, GuidePage>();

    [Header("Current Page")]
    // The current page we're on.
    public string currentPage = "";

    // + Initialization
    public void Initialize()
    {
        // Singleton.
        I = this;

        // Hide.
        gameObject.SetActive(false);

        // Connect chapter names.
        foreach (Transform t in chapterParent)
        {
            // Get guide page.
            GuidePage guidePage = t.GetComponent<GuidePage>();

            // Connect guide page, if extant.
            if (guidePage != null)
                chapters[t.name] = guidePage;
        }
    }

    // + Buttons

    // Select a page.
    public void B_GoToPage(string pageName)
    {
        // Fade out current page.
        if (currentPage != "")
            chapters[currentPage].FadeOut();

        // Set new current page.
        currentPage = pageName;

        // Fade in new page.
        chapters[pageName].FadeIn();
    }

    // Open Granga's Guide to the Galaxy.
    public void Open()
    {
        // Activate game object.
        gameObject.SetActive(true);
    }

    // Close Granga's Guide to the Galaxy.
    public void B_Close()
    {
        // Deactivate game object.
        gameObject.SetActive(false);
    }
}
