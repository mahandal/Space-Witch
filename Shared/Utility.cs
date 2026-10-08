using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;

public class Utility : MonoBehaviour
{
    // + Simple helper functions.

    // Read a unit's name and extract its level.

    // Convert a float representing time into a formatted string.
    public static string TimeString(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string timeString = t.TotalHours >= 1
            ? $"{(int)t.TotalHours}:{t.Minutes:00}:{t.Seconds:00}"
            : $"{t.Minutes}:{t.Seconds:00}";

        return timeString;
    }

    // Increment an int stored as a string and return the int.
    public static int IncrementString(string s, int amount = 1, int maxValue = int.MaxValue)
    {
        // Attempt to parse the string into an int.
        if (int.TryParse(s, out int value))
        {
            // Get new value.
            int newValue = value + amount;

            // Cap at max.
            if (newValue > maxValue)
                newValue = maxValue;

            // Return new value
            return newValue;
        }
        else
        {
            Debug.LogWarning($"INVALID INPUT: '{s}' is not an integer.");
            return 404;
        }
    }

    // Increment the value within a TMP_Text object, assuming it contains an integer, toward a given value.
    // Amount incremented depends on difference between current value and target value.
    public static void IncrementText(TMP_Text text, int targetValue)
    {
        // Attempt to parse the text object into an int.
        if (int.TryParse(text.text, out int value))
        {
            // Find difference between current value and target value.
            int difference = targetValue - value;

            // If we've reached the target, return.
            if (difference == 0)
                return;

            // + Decide how much to increment.
            // Default to 1.
            int amount = 1;

            // 100: Increase to 2
            if (difference > 100)
                amount = 2;
            // 500: Increase to 3
            if (difference > 500)
                amount = 3;
            // 1,000: Increase to 4
            if (difference > 1000)
                amount = 4;
            // 5,000: Increase to 5
            if (difference > 5000)
                amount = 5;
            // 10,000: Increase to 11
            if (difference > 10000)
                amount = 11;
            // 100,000: Increase to 111
            if (difference > 100000)
                amount = 111;
            // 1,000,000: Increase to 1,111
            if (difference > 1000000)
                amount = 1111;

            // Get new value.
            int newValue = value + amount;

            // Assign to text object.
            text.text = newValue.ToString();
        }
        else
        {
            Debug.LogWarning($"INVALID INPUT: '{text.text}' on {text.name} is not an integer.");
        }
    }

    // Convert an int to roman numeral format.
    public static string ToRomanNumeral(int number)
    {
        // 0.
        if (number == 0)
            return "";

        // Out of bounds.
        if (number < 0 || number > 3999)
        {
            Debug.LogError("Failing to convert int to roman numeral: " + number);
            return "";
        }

        int[] values =    { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        string[] symbols = { "M",  "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        var sb = new StringBuilder();
        for (int i = 0; i < values.Length; i++)
        {
            while (number >= values[i])
            {
                number -= values[i];
                sb.Append(symbols[i]);
            }
        }
        return sb.ToString();
    }

    // Shuffle a list.
    public static List<T> Shuffle<T>(List<T> list)
    {
        List<T> shuffled = new List<T>(list);
        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
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
        MenuManager.I.saveData.currentStarName = StarManager.I.currentStar.myName;

        // Update our save data's leader name.
        MenuManager.I.saveData.leaderName = DM.I.goodLeader.myName;

        // Convert to json.
        string json = JsonUtility.ToJson(MenuManager.I.saveData);

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
        MenuManager.I.saveData = new SaveData();

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
