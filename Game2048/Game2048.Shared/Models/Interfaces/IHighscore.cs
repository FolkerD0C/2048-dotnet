using System;

namespace Game2048.Shared.Models;

/// <summary>
/// Defines two properties for tracking high scores in the game.
/// </summary>
public interface IHighscore : ISerializable, IComparable
{
    /// <summary>
    /// The name of the player who achieved the high score.
    /// </summary>
    string PlayerName
    {
        get;
    }

    /// <summary>
    /// The score of the player who achieved the high score.
    /// </summary>
    int PlayerScore
    {
        get;
    }
}