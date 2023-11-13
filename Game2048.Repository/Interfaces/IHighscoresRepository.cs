using Game2048.Shared.Models;
using System.Collections.Generic;

namespace Game2048.Repository;

public interface IHighscoresRepository
{
    IList<IHighscore> HighScores { get; }

    void AddHighscore(IHighscore highscoreObject);

    void AddNewHighscore(IHighscore highscoreObject);
}