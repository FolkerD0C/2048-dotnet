using Game2048.Config;
using Game2048.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Game2048.Repository;

/// <summary>
/// A class that represents a manager for the high scores.
/// </summary>
public class HighscoresRepository : IHighscoresRepository
{
    IList<Highscore> highScores;
    public IList<Highscore> HighScores => highScores;

    /// <summary>
    /// Creates a new instance for the <see cref="HighscoresRepository"/> class.
    /// </summary>
    public HighscoresRepository()
    {
        highScores = new List<Highscore>();
    }

    /// <summary>
    /// Adds a new high score to the <see cref="HighScores"/> list. While <see cref="AddNewHighscore(IHighscore)"/> limits the <see cref="HighScores"/> list to <see cref="GameConfiguration.MaxHighscoresListLength"/> this method can add a highscore beyond that limit.
    /// </summary>
    /// <param name="highscoreObject">The high score object that needs to be added to <see cref="HighScores"/>.</param>
    void AddHighscore(Highscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).ToList();
    }

    public void AddNewHighscore(Highscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderByDescending(item => item.PlayerScore).ThenBy(item => item.PlayerName)
            .Take(ConfigManager.GetConfigItemValue<int>("MaxHighscoresListLength")).ToList();
    }

    public string Serialize()
    {
        return "";
        /*
        StringBuilder jsonBuilder = new();
        jsonBuilder.Append('[');
        jsonBuilder.AppendJoin(",", highScores.Select(hsdo => hsdo.Serialize()));
        jsonBuilder.Append(']');
        return jsonBuilder.ToString();
        */
    }

    public void Deserialize(string deserializee)
    {
        return;
        /*using var jsonDoc = JsonDocument.Parse(deserializee);
        var jsonArray = jsonDoc.RootElement.EnumerateArray();
        foreach (var item in jsonArray)
        {
            IHighscore highscore = new Highscore();
            highscore.Deserialize(item.GetRawText());
            AddHighscore(highscore);
        }*/
    }
}