using System.Collections.Generic;
using Game2048.Backend.Models;

namespace Game2048.Backend.Repository;

public interface IHighscoresData
{
    IList<IHighscore> HighScores { get; }

    void AddHighscore(IHighscore highscoreObject);

    void AddNewHighscore(IHighscore highscoreObject);
}