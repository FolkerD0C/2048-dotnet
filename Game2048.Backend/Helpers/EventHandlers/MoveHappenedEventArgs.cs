using System;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;

namespace Game2048.Backend.Helpers.EventHandlers;

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