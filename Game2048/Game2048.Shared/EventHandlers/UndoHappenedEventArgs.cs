using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about an undo happened on the playing grid.
/// </summary>
public class UndoHappenedEventArgs : EventArgs
{
    /// <summary>
    /// The new state of the playing grid.
    /// </summary>
    public IGameState Position { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="UndoHappenedEventArgs"/> class.
    /// </summary>
    /// <param name="position">The new state of the playing grid.</param>
    public UndoHappenedEventArgs(IGameState position)
    {
        Position = position;
    }
}