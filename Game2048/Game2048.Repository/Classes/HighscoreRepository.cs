using Game2048.Config;
using Game2048.Repository.SaveDataObjects;
using Game2048.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Repository;

/// <summary>
/// A class that represents a manager for the high scores.
/// </summary>
public class HighscoreRepository : IHighscoreRepository
{
    List<Highscore> highScores;
    public List<Highscore> HighScores => highScores;

    /// <summary>
    /// Creates a new instance for the <see cref="HighscoreRepository"/> class.
    /// </summary>
    public HighscoreRepository()
    {
        highScores = new List<Highscore>();
    }

    public HighscoreRepository(HighscoreSaveData saveData)
    {
        highScores = saveData.Highscores;
    }

    public void AddNewHighscore(Highscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderByDescending(item => item.PlayerScore).ThenBy(item => item.PlayerName)
            .Take(ConfigManager.GetConfigItemValue<int>("MaxHighscoresListLength")).ToList();
    }

    public HighscoreSaveData GetSaveDataObject()
    {
        HighscoreSaveData result = new();
        result.Populate(highScores);
        return result;
    }
}