using System;
using Game2048.Shared.Models;

namespace Game2048.Shared.EventHandlers;

public class UndoHappenedEventArgs : EventArgs
{
    public IGamePosition Position { get; }
    public UndoHappenedEventArgs(IGamePosition position)
    {
        Position = position;
    }
}