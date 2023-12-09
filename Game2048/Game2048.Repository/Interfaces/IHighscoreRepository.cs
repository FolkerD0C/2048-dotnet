using Game2048.Repository.SaveDataObjects;
using Game2048.Shared.Models;
using System.Collections.Generic;

namespace Game2048.Repository;

/// <summary>
/// Represents a manager for the high scores.
/// </summary>
public interface IHighscoreRepository
{
    /// <summary>
    /// A list that contains all the high scores.
    /// </summary>
    List<Highscore> HighScores { get; }

    /// <summary>
    /// Adds a new high score to the <see cref="HighScores"/> list.
    /// </summary>
    /// <param name="highscoreObject">The high score object that needs to be added to <see cref="HighScores"/>.</param>
    void AddNewHighscore(Highscore highscoreObject);

    /// <summary>
    /// Gets a serializable representation of an <see cref="IHighscoreRepository"/>.
    /// </summary>
    /// <returns>A serializable representation of an <see cref="IHighscoreRepository"/>.</returns>
    public HighscoreSaveData GetSaveDataObject();
}