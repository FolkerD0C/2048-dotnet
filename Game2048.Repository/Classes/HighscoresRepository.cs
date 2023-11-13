using System;
using System.Linq;
using System.Collections.Generic;
using Game2048.Shared.Models;
using Game2048.Config;

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
        if (highScores.Count >= ConfigManager.GetConfigItem("MaxHighscoresListLength", default(int)))
        {
            throw new ArgumentException("Highscore object can not be added because the list is full.");
        }
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).ToList();
    }

    public void AddNewHighscore(IHighscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).Take(ConfigManager.GetConfigItem("MaxHighscoresListLength", default(int))).ToList();
    }
}