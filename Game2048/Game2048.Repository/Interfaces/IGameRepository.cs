using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Repository.SaveDataObjects;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;

namespace Game2048.Repository;

/// <summary>
/// Represents a lower level manager for an active play.
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// The number of remaining lives.
    /// </summary>
    public int RemainingLives { get; }

    /// <summary>
    /// The number of remaining undos.
    /// </summary>
    public int RemainingUndos { get; }

    /// <summary>
    /// The highest number on the playing grid.
    /// </summary>
    public int HighestNumber { get; }

    /// <summary>
    /// The width of the playing grid.
    /// </summary>
    public int GridWidth { get; }

    /// <summary>
    /// The height of the playing grid.
    /// </summary>
    public int GridHeight { get; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string PlayerName { get; set; }

    /// <summary>
    /// The goal of the game. If the player reaches it, they win.
    /// </summary>
    public int Goal { get; }

    /// <summary>
    /// The current state of the playing grid.
    /// </summary>
    GameState CurrentGameState { get; }

    /// <summary>
    /// If an error happens during a <see cref="MoveGrid(MoveDirection)"/> call, then this property stores the message for that error.
    /// </summary>
    public string MoveResultErrorMessage { get; }

    /// <summary>
    /// Performs a move ont the current <see cref="GameState"/> object.
    /// </summary>
    /// <param name="input">The direction to move towards.</param>
    /// <returns>A <see cref="MoveResult"/> that represents if the move was successful.</returns>
    public MoveResult MoveGrid(MoveDirection input);

    /// <summary>
    /// Performs an undo.
    /// </summary>
    /// <returns>The new current <see cref="GameState"/> object or null if the undo could not be performed.</returns>
    public GameState? Undo();

    /// <summary>
    /// An event that is triggered when a repository event has happened. For repository event types, see <see cref="GameRepositoryEvent"/>.
    /// </summary>
    public event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;

    /// <summary>
    /// Gets a serializable representation of an <see cref="IGameRepository"/>.
    /// </summary>
    /// <returns>A serializable representation of an <see cref="IGameRepository"/>.</returns>
    public GameSaveData GetSaveDataObject();
}