using System.Collections.Generic;
using Game2048.Shared.Models;

namespace Game2048.Repository;

public interface IHighscoresRepository
{
    IList<IHighscore> HighScores { get; }

    void AddHighscore(IHighscore highscoreObject);

    void AddNewHighscore(IHighscore highscoreObject);
}