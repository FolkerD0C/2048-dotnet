using 2048ish.Base.Models;
using Game2048.Config;
using Game2048.Processors.SaveDataObjects;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Processors;

/// <summary>
/// A class that represents a manager for the high scores.
/// </summary>
public class HighscoreProcessor : IHighscoreProcessor
{
    List<Highscore> highScores;
    public List<Highscore> HighScores => highScores;

    /// <summary>
    /// Creates a new instance for the <see cref="HighscoreProcessor"/> class.
    /// </summary>
    public HighscoreProcessor()
    {
        highScores = new List<Highscore>();
    }

    public HighscoreProcessor(HighscoreSaveData saveData)
    {
        highScores = saveData.Highscores;
    }

    public void AddNewHighscore(Highscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderByDescending(item => item.PlayerScore).ThenBy(item => item.PlayerName)
            .Take(ConfigRepository.GetConfigItemValue<int>("MaxHighscoresListLength")).ToList();
    }

    public HighscoreSaveData GetSaveDataObject()
    {
        HighscoreSaveData result = new();
        result.Populate(highScores);
        return result;
    }
}