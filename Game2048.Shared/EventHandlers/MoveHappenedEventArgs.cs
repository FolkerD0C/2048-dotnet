using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

public class MoveHappenedEventArgs : EventArgs
{
    public IGamePosition Position { get; }

    public MoveDirection Direction { get; }

    public MoveHappenedEventArgs(IGamePosition position, MoveDirection direction)
    {
        Position = position;
        Direction = direction;
    }
}