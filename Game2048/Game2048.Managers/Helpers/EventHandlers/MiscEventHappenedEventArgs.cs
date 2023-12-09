using Game2048.Managers.Enums;
using System;

namespace Game2048.Managers.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about miscellanious events happening during a play.
/// </summary>
public class MiscEventHappenedEventArgs : EventArgs
{
    /// <summary>
    /// The type of the miscellanious event.
    /// </summary>
    public MiscEvent Event { get; }

    /// <summary>
    /// An optional integer that is needed for some miscellanious events.
    /// </summary>
    public int NumberArg { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="MiscEventHappenedEventArgs"/> class.
    /// </summary>
    /// <param name="Event">The type of the miscellanious event.</param>
    /// <param name="numberArg">An optional integer that is needed for some miscellanious events.</param>
    public MiscEventHappenedEventArgs(MiscEvent Event, int numberArg = -1)
    {
        this.Event = Event;
        NumberArg = numberArg;
    }
}