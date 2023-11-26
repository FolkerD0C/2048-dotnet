using Game2048.Shared.Models;
using System.Collections.Generic;

namespace Game2048.Repository;

public interface IHighscoresRepository : ISerializable
{
    IList<IHighscore> HighScores { get; }

    void AddNewHighscore(IHighscore highscoreObject);
}