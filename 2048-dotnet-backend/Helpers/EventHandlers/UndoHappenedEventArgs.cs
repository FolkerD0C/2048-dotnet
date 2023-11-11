using System;
using Game2048.Backend.Models;

namespace Game2048.Backend.Helpers.EventHandlers;

public class UndoHappenedEventArgs : EventArgs
{
    public IGamePosition Position { get; }
    public UndoHappenedEventArgs(IGamePosition position)
    {
        Position = position;
    }
}