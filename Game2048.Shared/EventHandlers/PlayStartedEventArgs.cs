using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

public class PlayStartedEventArgs : EventArgs
{
    IGamePosition Position { get; }

    int RemainingUndos { get; }

    int RemainingLives { get; }

    public PlayStartedEventArgs(IGamePosition position, int remainingUndos, int remainingLives)
    {
        Position = position;
        RemainingUndos = remainingUndos;
        RemainingLives = remainingLives;
    }
}