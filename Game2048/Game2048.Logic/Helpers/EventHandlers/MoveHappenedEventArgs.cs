using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;

namespace Game2048.Logic.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a movement that happened on the playing grid.
/// </summary>
public class MoveHappenedEventArgs : EventArgs
{
    /// <summary>
    /// The new state of the playing grid.
    /// </summary>
    public GameState State { get; }

    /// <summary>
    /// The direction the move happened.
    /// </summary>
    public MoveDirection Direction { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="MoveHappenedEventArgs"/> class.
    /// </summary>
    /// <param name="state">The new state of the playing grid.</param>
    /// <param name="direction">The direction the move happened.</param>
    public MoveHappenedEventArgs(GameState state, MoveDirection direction)
    {
        State = state;
        Direction = direction;
    }
}