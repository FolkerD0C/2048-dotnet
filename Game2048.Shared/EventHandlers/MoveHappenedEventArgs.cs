using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

public class MoveHappenedEventArgs : EventArgs
{
    public IGameState Position { get; }

    public MoveDirection Direction { get; }

    public MoveHappenedEventArgs(IGameState position, MoveDirection direction)
    {
        Position = position;
        Direction = direction;
    }
}