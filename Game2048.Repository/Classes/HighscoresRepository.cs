using Game2048.Config;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Repository;

public class HighscoresRepository : IHighscoresRepository
{
    IList<IHighscore> highScores;
    public IList<IHighscore> HighScores => highScores;

    public HighscoresRepository()
    {
        highScores = new List<IHighscore>();
    }

    public void AddHighscore(IHighscore highscoreObject)
    {
        if (highScores.Count >= ConfigManager.GetConfigItem<int>("MaxHighscoresListLength"))
        {
            throw new ArgumentException("Highscore object can not be added because the list is full.");
        }
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).ToList();
    }

    public void AddNewHighscore(IHighscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).Take(ConfigManager.GetConfigItem<int>("MaxHighscoresListLength")).ToList();
    }
}