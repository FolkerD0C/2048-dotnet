using Game2048.Logic.EventHandlers;
using System;

namespace Game2048.Logic;

/// <summary>
/// Represents a set of events and properties that contain information about an active play.
/// </summary>
public interface IPlayInstance
{
    /// <summary>
    /// The ID of the play instance.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The current score of the play.
    /// </summary>
    int PlayerScore { get; }

    /// <summary>
    /// The player name of the current play.
    /// </summary>
    string PlayerName { get; set; }

    /// <summary>
    /// An event that triggers when a play is started.
    /// </summary>
    event EventHandler<PlayStartedEventArgs>? PlayStarted;

    /// <summary>
    /// An event that triggers when a move happens.
    /// </summary>
    event EventHandler<MoveHappenedEventArgs>? MoveHappened;

    /// <summary>
    /// An event that triggers when an undo happens.
    /// </summary>
    event EventHandler<UndoHappenedEventArgs>? UndoHappened;

    /// <summary>
    /// An event that triggers when an error happens.
    /// </summary>
    event EventHandler<ErrorHappenedEventArgs>? ErrorHappened;

    /// <summary>
    /// An event that triggers when a misc event happens. For misc event types, see <see cref="Shared.Enums.MiscEvent"/>
    /// </summary>
    event EventHandler<MiscEventHappenedEventArgs>? MiscEventHappened;

    /// <summary>
    /// An event that triggers when the player changes their name.
    /// </summary>
    event EventHandler<PlayerNameChangedEventArgs>? PlayerNameChanged;

    /// <summary>
    /// An event that triggers when a play ends.
    /// </summary>
    event EventHandler? PlayEnded;
}
