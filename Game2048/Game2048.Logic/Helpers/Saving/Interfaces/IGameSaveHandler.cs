using Game2048.Repository;
using Game2048.Shared.Models;

namespace Game2048.Logic.Saving;

/// <summary>
/// Represents a manager for game saving and loading.
/// </summary>
public interface IGameSaveHandler
{
    /// <summary>
    /// The repository to serialize or desiarilize.
    /// </summary>
    IGameRepository GameRepository { get; }

    /// <summary>
    /// Sets the file path to save to.
    /// </summary>
    /// <param name="filePath">The full path of the file to save to.</param>
    void UpdateFilePath(string filePath);

    /// <summary>
    /// Performs a save action.
    /// </summary>
    /// <returns>The result of the save action.</returns>
    SaveResult Save();

    /// <summary>
    /// Performs a load action.
    /// </summary>
    void Load();
}