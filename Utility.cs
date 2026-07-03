using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Utility : MonoBehaviour
{
    // + Simple helper functions.

    public static List<T> Shuffle<T>(List<T> list)
    {
        List<T> shuffled = new List<T>(list);
        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = shuffled[k];
            shuffled[k] = shuffled[n];
            shuffled[n] = value;
        }
        return shuffled;
    }

    // Loads an image from file.
    public static void LoadImage(Image image, string filePath)
    {
        Sprite sprite = Resources.Load<Sprite>(filePath);
        image.sprite = sprite;
    }

    // Loads an image from file.
    public static void LoadImage(SpriteRenderer spriteRenderer, string filePath)
    {
        Sprite sprite = Resources.Load<Sprite>(filePath);
        spriteRenderer.sprite = sprite;
    }

    // SetOpacity
    // Set the opacity for a sprite renderer.
    public static void SetOpacity(SpriteRenderer spriteRenderer, float opacity)
    {
        Color c = spriteRenderer.color;
        c.a = opacity;
        spriteRenderer.color = c;
    }

    // Pop the first element out of a list.
    public static T Pop<T>(List<T> list)
    {
        T first = list[0];
        list.RemoveAt(0);
        return first;
    }

    // + Saving & Loading

    // Get the file path to our save file.
    public static string GetSaveFilePath()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, "SaveData.json");
    }

    // Get save data.
    public static SaveData GetSaveData()
    {
        // Get save file path.
        string path = GetSaveFilePath();

        // If file doesn't exist, return blank save data.
        if (!System.IO.File.Exists(path))
            return new SaveData();

        // Read file into string (in json format).
        string json = System.IO.File.ReadAllText(path);

        // Convert string into object and return.
        return JsonUtility.FromJson<SaveData>(json);
    }

    // Save our current progress.
    // Note: This is where we update our save data's current star, but NOT where we update its deck list.
    // (The deck list is edited in CardOnPlanet, and tbd in the shop.)
    public static void SaveGame()
    {
        // Update our saveData's current star.
        MainMenu.I.saveData.currentStarName = StarManager.I.currentStar.myName;

        // Update our save data's leader name.
        MainMenu.I.saveData.leaderName = GM.I.goodLeader.myName;

        // Convert to json.
        string json = JsonUtility.ToJson(MainMenu.I.saveData);

        // Get save file path.
        string path = GetSaveFilePath();

        // Save to file.
        System.IO.File.WriteAllText(path, json);
    }

    // Reset save data.
    public static void ResetSave()
    {
        // Get file path.
        string path = GetSaveFilePath();

        // Delete the file!
        System.IO.File.Delete(path);

        // Reset our save data object.
        MainMenu.I.saveData = new SaveData();

        // Reset our current star.
        StarManager.I.currentStar = StarManager.I.startingStar;
        StarManager.I.currentStarName = "";

        // Reset our current planet.
        StarManager.I.planetIndex = 1;
    }

    // + Scene loading

    // Re-load the Game scene.
    public static void LoadGameScene()
    {
        // Load the scene.
        SceneManager.LoadScene("Game");
    }

    // Exit the game.
    public static void ExitGame()
    {
        // Close the application.
        Application.Quit();
    }
}
