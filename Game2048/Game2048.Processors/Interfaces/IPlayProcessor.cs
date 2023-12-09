using _2048ish.Base.Enums;
using _2048ish.Base.Models;
using Game2048.Processors.Enums;
using Game2048.Processors.EventHandlers;
using Game2048.Processors.SaveDataObjects;
using System;

namespace Game2048.Processors;

/// <summary>
/// Represents a lower level manager for an active play.
/// </summary>
public interface IPlayProcessor
{
    /// <summary>
    /// The ID of the play.
    /// </summary>
    public Guid Id { get; }

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
    /// An event that is triggered when a processor level event has happened. For processor level event types, see <see cref="PlayProcessorEvent"/>.
    /// </summary>
    public event EventHandler<PlayProcessorEventHappenedEventArgs>? PlayProcessorEventHappened;

    /// <summary>
    /// Gets a serializable representation of an <see cref="IPlayProcessor"/>.
    /// </summary>
    /// <returns>A serializable representation of an <see cref="IPlayProcessor"/>.</returns>
    public GameSaveData GetSaveDataObject();
}