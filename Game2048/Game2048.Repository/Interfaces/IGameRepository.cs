using Game2048.Repository.Enums;
using Game2048.Repository.EventHandlers;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;

namespace Game2048.Repository;

/// <summary>
/// Represents a lower level manager for an active play.
/// </summary>
public interface IGameRepository : ISerializable
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
    /// The list that contains the accepted spawnable tiles/numbers.
    /// </summary>
    public IList<int>? AcceptedSpawnables { get; }

    /// <summary>
    /// The current state of the playing grid.
    /// </summary>
    GameState CurrentGameState { get; }

    /// <summary>
    /// A <see cref="LinkedList{GameState}"/> that is used for undoing.
    /// </summary>
    public LinkedList<GameState> UndoChain { get; }

    /// <summary>
    /// If an error happens during a <see cref="MoveGrid(MoveDirection)"/> call, then this property stores the message for that error.
    /// </summary>
    public string MoveResultErrorMessage { get; }

    /// <summary>
    /// Gets the score of the first <see cref="GameState"/> object in the <see cref="UndoChain"/>.
    /// </summary>
    /// <returns></returns>
    int GetScore();

    /// <summary>
    /// Performs a move ont the first <see cref="GameState"/> object in the <see cref="UndoChain"/>.
    /// </summary>
    /// <param name="input">The direction to move towards.</param>
    /// <returns>A <see cref="MoveResult"/> that represents if the move was successful.</returns>
    public MoveResult MoveGrid(MoveDirection input);

    /// <summary>
    /// Performs an undo. Removes the first <see cref="GameState"/> object from the <see cref="UndoChain"/> and returns the next.
    /// Does nothing if the <see cref="LinkedList{GameState}.Count"/> of the <see cref="UndoChain"/> is 1.
    /// </summary>
    /// <returns>The new first <see cref="GameState"/> object of the <see cref="UndoChain"/>
    /// or null if the <see cref="LinkedList{GameState}.Count"/> of the <see cref="UndoChain"/> is 1.</returns>
    public GameState? Undo();

    /// <summary>
    /// An event that is triggered when a repository event has happened. For repository event types, see <see cref="GameRepositoryEvent"/>.
    /// </summary>
    public event EventHandler<GameRepositoryEventHappenedEventArgs>? GameRepositoryEventHappened;
}