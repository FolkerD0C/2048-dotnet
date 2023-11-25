using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

public class UndoHappenedEventArgs : EventArgs
{
    public IGameState Position { get; }
    public UndoHappenedEventArgs(IGameState position)
    {
        Position = position;
    }
}