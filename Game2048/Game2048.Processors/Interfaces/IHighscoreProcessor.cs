using 2048ish.Base.Models;
using Game2048.Processors.SaveDataObjects;
using System.Collections.Generic;

namespace Game2048.Processors;

/// <summary>
/// Represents a manager for the high scores.
/// </summary>
public interface IHighscoreProcessor
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
    /// Gets a serializable representation of an <see cref="IHighscoreProcessor"/>.
    /// </summary>
    /// <returns>A serializable representation of an <see cref="IHighscoreProcessor"/>.</returns>
    public HighscoreSaveData GetSaveDataObject();
}