using Game2048.Repository;

namespace Game2048.Logic.Saving;

/// <summary>
/// Represents a manager for high score saving and loading.
/// </summary>
public interface IHighscoreHandler
{
    /// <summary>
    /// The repository to serialize or desiarilize.
    /// </summary>
    IHighscoresRepository HighscoresData { get; }

    /// <summary>
    /// Performs a save action.
    /// </summary>
    void Save();

    /// <summary>
    /// Perform a load action.
    /// </summary>
    void Load();

    /// <summary>
    /// Adds a new highscore to <see cref="HighscoresData"/>.
    /// </summary>
    /// <param name="playerName">The name of the player who set a high score.</param>
    /// <param name="score">The score that has been set by <paramref name="playerName"/>.</param>
    void AddNewHighscore(string playerName, int score);
}