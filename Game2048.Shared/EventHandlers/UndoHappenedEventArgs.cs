using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

public class UndoHappenedEventArgs : EventArgs
{
    public IGamePosition Position { get; }
    public UndoHappenedEventArgs(IGamePosition position)
    {
        Position = position;
    }
}