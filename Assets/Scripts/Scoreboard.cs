using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

// Data structures for our high scores
[System.Serializable]
public class HighScore
{
    public string myName;
    public int score;
    public string date;

    public HighScore(string _myName, int _score)
    {
        myName = _myName;
        score = _score;
        date = System.DateTime.Now.ToString("MM/dd/yyyy");
    }

    public HighScore(string _myName, int _score, string _date)
    {
        myName = _myName;
        score = _score;
        date = _date;
    }
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScore> scores = new List<HighScore>();
}

public class Scoreboard : MonoBehaviour
{
    [Header("Settings")]
    public int maxScores = 12;
    
    [Header("UI References")]
    //public GameObject scoreEntryPrefab;
    //public Transform scoreContainer;
    public TMP_InputField nameInput;
    public GameObject newHighScorePanel;
    public List<Ranking> rankings;
    public Ranking rankThirteen;
    
    // The current list of high scores
    private HighScoreList highScores = new HighScoreList();
    
    // File path for storing scores
    //private string savePath => Path.Combine(Application.persistentDataPath, "highscores.json");
    
    // Current score pending name entry
    private int currentScore;
    private bool hasNewHighScore = false;
    
    void Start()
    {
        // Hide high score input initially
        if (newHighScorePanel != null)
            newHighScorePanel.SetActive(false);
    }
    
    // Call this when the game ends to check for a new high score
    public void CheckForHighScore(int score)
    {
        // Load high scores?
        //LoadHighScores();

        // Store current score for some Claude reason
        currentScore = score;
        
        // Check if this is a high score
        if (score > highScores.scores.Min(s => s.score))
        {
            NewHighScore();
            /* hasNewHighScore = true;
            
            // Show name input panel
            if (newHighScorePanel != null)
                newHighScorePanel.SetActive(true);

            // Disable rank thirteen
            rankThirteen.gameObject.SetActive(false); */
        }
        else
        {
            RankThirteen();
            // Not a high score this time, just rank #13 again...
            /* rankThirteen.gameObject.SetActive(true);
            HighScore lowScore = new HighScore("Space Witch", score);
            SetRanking(rankThirteen, lowScore); */
        }
    }

    public void NewHighScore()
    {
        hasNewHighScore = true;
            
        // Show name input panel
        if (newHighScorePanel != null)
            newHighScorePanel.SetActive(true);

        // Disable rank thirteen
        rankThirteen.gameObject.SetActive(false);
    }

    public void RankThirteen()
    {
        // Not a high score this time, just rank #13 again...
        rankThirteen.gameObject.SetActive(true);
        int score = (int)(Gatherer.starsGathered);
        HighScore lowScore = new HighScore("Space Witch", score);
        SetRanking(rankThirteen, lowScore);
    }
    
    // Called when player submits their name for a high score
    public void SubmitHighScore()
    {
        if (!hasNewHighScore) return;
        
        // Get name from input field, default to "WITCH" if empty
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "WITCH";
            
        // Add new high score
        AddHighScore(playerName, currentScore);
        
        // Hide input panel
        if (newHighScorePanel != null)
            newHighScorePanel.SetActive(false);
            
        // Reset flag
        hasNewHighScore = false;
        
        // Show updated high scores
        DisplayHighScores();
    }
    
    // Add a new high score to the list
    private void AddHighScore(string name, int score)
    {
        // Create new high score entry
        HighScore newScore = new HighScore(name, score);
        
        // Add to list
        highScores.scores.Add(newScore);
        
        // Sort by score (descending)
        highScores.scores = highScores.scores.OrderByDescending(s => s.score).ToList();
        
        // Trim to max size
        if (highScores.scores.Count > maxScores)
            highScores.scores = highScores.scores.Take(maxScores).ToList();
            
        // Save updated list
        SaveHighScores();
    }
    
    // Display high scores in the UI
    public void DisplayHighScores()
    {
        // Load high scores
        LoadHighScores();

        // Disable rank 13 in pre-game
        if (Gatherer.starsGathered <= 0)
            rankThirteen.gameObject.SetActive(false);
        
        // Go through each ranking
        for (int i = 0; i < rankings.Count; i++)
        {
            // Set ranking
            SetRanking(rankings[i], highScores.scores[i]);
        }
    }

    // Fills the given ranking with data from the given high score.
    public void SetRanking(Ranking ranking, HighScore highScore)
    {
        ranking.nameText.text = highScore.myName;
        ranking.scoreText.text = highScore.score.ToString();
        ranking.dateText.text = highScore.date;
    }
    
    // Save high scores to a file
    private void SaveHighScores()
    {
        // Get save path
        string savePath = Path.Combine(Application.persistentDataPath, "highscores_" + GM.I.nebula.myName + ".json");

        try
        {
            string json = JsonUtility.ToJson(highScores, true);
            File.WriteAllText(savePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save high scores: " + e.Message);
        }
    }
    
    // Load high scores from file
    private void LoadHighScores()
    {
        // Get save path
        string savePath = Path.Combine(Application.persistentDataPath, "highscores_" + GM.I.nebula.myName + ".json");
        
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                highScores = JsonUtility.FromJson<HighScoreList>(json);
            }
            else
            {
                Debug.Log("No high score file found. Creating new list.");
                highScores = new HighScoreList();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load high scores: " + e.Message);
            highScores = new HighScoreList();
        }

        // Initialize default scores
        // Note: Mostly for new games, but also in case I fuck up the list somehow. Shouldn't even happen tho so this is mostly so that right now I don't have to go delete the old JSON.
        if (highScores.scores.Count != 12)
            InitializeDefaultScores();
    }

    // Add default scores as goals for the player
    private void InitializeDefaultScores()
    {
        // Clear previous list
        // (in case of what? idk but y'know. safety)
        highScores.scores.Clear();

        // Create a list of default scores for players to aim for
        List<HighScore> defaultScores = new List<HighScore>
        {
            new HighScore("Moses", 13, "3/10/1913"),
            new HighScore("Ada", 12, "11/27/1852"),
            new HighScore("Grace and Hopper", 11, "1/1/1992"),
            new HighScore("Dr. Jemison", 10, "9/12/1992"),
            new HighScore("Matherine Katherine", 8, "2/20/1962"),
            new HighScore("Luna Lovelace", 7, "7/20/1969"),
            new HighScore("Valorous Valentina", 6, "6/16/1963"),
            new HighScore("Christina Long", 5, "12/28/2019"),
            new HighScore("Sally", 4, "6/18/1983"),
            new HighScore("Annie", 3, "6/25/2011"),
            new HighScore("Wally", 2, "7/20/2021"),
            new HighScore("Dorothy", 1, "11/10/2008")
        };
        /* {
            new HighScore("Moses", 1000000, "3/10/1913"),
            new HighScore("Ada", 500000, "11/27/1852"),
            new HighScore("Grace and Hopper", 250000, "1/1/1992"),
            new HighScore("Dr. Jemison", 100000, "9/12/1992"),
            new HighScore("Matherine Katherine", 50000, "2/20/1962"),
            new HighScore("Luna Lovelace", 35000, "7/20/1969"),
            new HighScore("Valorous Valentina", 25000, "6/16/1963"),
            new HighScore("Christina Long", 20000, "12/28/2019"),
            new HighScore("Sally", 15000, "6/18/1983"),
            new HighScore("Annie", 10000, "6/25/2011"),
            new HighScore("Wally", 5000, "7/20/2021"),
            new HighScore("Dorothy", 2500, "11/10/2008")
        }; */
        
        
        // Add them to our high scores list
        foreach (var score in defaultScores)
        {
            highScores.scores.Add(score);
        }
        
        // Save these default scores
        SaveHighScores();
    }
}