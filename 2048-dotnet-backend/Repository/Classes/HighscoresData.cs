using System;
using System.Linq;
using System.Collections.Generic;
using Game2048.Backend.Models;
using Game2048.Backend.Helpers.Config;

namespace Game2048.Backend.Repository;

public class HighscoresData : IHighscoresData
{
    IList<IHighscore> highScores;
    public IList<IHighscore> HighScores => highScores;

    public HighscoresData()
    {
        highScores = new List<IHighscore>();
    }

    public void AddHighscore(IHighscore highscoreObject)
    {
        if (highScores.Count >= GameConfiguration.MaxHighscoresListLength)
        {
            throw new ArgumentException("Highscore object can not be added because the list is full.");
        }
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).ToList();
    }

    public void AddNewHighscore(IHighscore highscoreObject)
    {
        highScores.Add(highscoreObject);
        highScores = highScores.OrderBy(item => item).Take(GameConfiguration.MaxHighscoresListLength).ToList();
    }
}