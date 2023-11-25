using Game2048.Shared.Models;
using System;

namespace Game2048.Shared.EventHandlers;

public class PlayStartedEventArgs : EventArgs
{
    public IGamePosition Position { get; }

    public int RemainingUndos { get; }

    public int RemainingLives { get; }

    public int HighestNumber { get; }

    public int GridHeight { get; }

    public int GridWidth { get; }

    public string PlayerName { get; }

    public PlayStartedEventArgs(IGamePosition position, int remainingUndos, int remainingLives, int highestNumber, int gridHeight, int gridWidth, string playerName)
    {
        Position = position;
        RemainingUndos = remainingUndos;
        RemainingLives = remainingLives;
        HighestNumber = highestNumber;
        GridHeight = gridHeight;
        GridWidth = gridWidth;
        PlayerName = playerName;
    }
}