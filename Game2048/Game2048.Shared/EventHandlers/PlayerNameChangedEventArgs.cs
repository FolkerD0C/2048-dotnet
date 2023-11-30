using System;

namespace Game2048.Shared.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a player name change.
/// </summary>
public class PlayerNameChangedEventArgs : EventArgs
{
    /// <summary>
    /// The new name of the player.
    /// </summary>
    public string PlayerName { get; }

    /// <summary>
    /// Creates a new instance for the <see cref="PlayerNameChangedEventArgs"/> class.
    /// </summary>
    /// <param name="playerName">The new name of the player.</param>
    public PlayerNameChangedEventArgs(string playerName)
    {
        PlayerName = playerName;
    }
}
