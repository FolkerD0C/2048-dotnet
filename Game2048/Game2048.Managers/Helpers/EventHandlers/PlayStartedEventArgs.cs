using 2048ish.Base.Models;
using System;

namespace Game2048.Managers.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a new play starting.
/// </summary>
public class PlayStartedEventArgs : EventArgs
{
    /// <summary>
    /// The starting state of the playing grid.
    /// </summary>
    public GameState State { get; }

    /// <summary>
    /// The number of remaining undos.
    /// </summary>
    public int RemainingUndos { get; }

    /// <summary>
    /// The number of remaining lives.
    /// </summary>
    public int RemainingLives { get; }

    /// <summary>
    /// The highest number on the playing grid.
    /// </summary>
    public int HighestNumber { get; }

    /// <summary>
    /// The height of the playing grid.
    /// </summary>
    public int GridHeight { get; }

    /// <summary>
    /// The width of the playing grid.
    /// </summary>
    public int GridWidth { get; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string PlayerName { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="PlayStartedEventArgs"/> class.
    /// </summary>
    /// <param name="state">The starting state of the playing grid.</param>
    /// <param name="remainingUndos">The number of remaining undos.</param>
    /// <param name="remainingLives">The number of remaining lives.</param>
    /// <param name="highestNumber">The highest number on the playing grid.</param>
    /// <param name="gridHeight">The height of the playing grid.</param>
    /// <param name="gridWidth">The width of the playing grid.</param>
    /// <param name="playerName">The name of the player.</param>
    public PlayStartedEventArgs(GameState state, int remainingUndos, int remainingLives, int highestNumber, int gridHeight, int gridWidth, string playerName)
    {
        State = state;
        RemainingUndos = remainingUndos;
        RemainingLives = remainingLives;
        HighestNumber = highestNumber;
        GridHeight = gridHeight;
        GridWidth = gridWidth;
        PlayerName = playerName;
    }
}