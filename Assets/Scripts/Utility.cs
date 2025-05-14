using UnityEngine;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    // Loads a sprite into an Image
    public static void LoadImage(Image image, string talentName)
    {
        // Get file name
        string fileName = talentName;

        // Look for sprite
        Sprite sprite = Resources.Load<Sprite>(fileName);

        // Check if sprite exists
        if (sprite == null)
        {
            // If sprite doesn't exist, get talent to find default
            Talent talent = GM.I.talents[talentName];

            // Use class for default
            sprite = Resources.Load<Sprite>(talent.myClass);
        }

        // Load sprite into image
        image.sprite = sprite;
    }

    // Loads a texture into a RawImage
    public static void LoadImage(RawImage image, string fileName)
    {
        // Get file name
        //string fileName = talentName;

        // Look for texture
        Texture2D texture = Resources.Load<Texture2D>(fileName);

        // Check if texture exists
        /* if (texture == null)
        {
            // If texture doesn't exist, get talent to find default
            Talent talent = GM.I.talents[talentName];

            // Use class for default image
            texture = Resources.Load<Texture2D>(talent.myClass);
        } */

        // Load texture into image
        image.texture = texture;
    }

    public static void PreloadImages()
    {
        // Preload spell images
        foreach(string talentName in GM.I.talents.Keys)
        {
            if (GM.I.talents[talentName].isSpell)
            {
                //Resources.Load<Sprite>(talentName);
                Resources.Load<Sprite>(talentName + " (Spell)");
            }
        }
    }
}
