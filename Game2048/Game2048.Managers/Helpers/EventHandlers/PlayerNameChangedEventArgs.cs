using System;

namespace Game2048.Managers.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a player name change.
/// </summary>
public class PlayerNameChangedEventArgs : EventArgs
{
    /// <summary>
    /// The old name of the player.
    /// </summary>
    public string OldName { get; }

    /// <summary>
    /// The new name of the player.
    /// </summary>
    public string NewName { get; }

    /// <summary>
    /// Creates a new instance for the <see cref="PlayerNameChangedEventArgs"/> class.
    /// </summary>
    /// <param name="newName">The new name of the player.</param>
    public PlayerNameChangedEventArgs(string oldName, string newName)
    {
        OldName = oldName;
        NewName = newName;
    }
}
