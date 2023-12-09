using Game2048.Processors.Enums;
using System;

namespace Game2048.Processors.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a processor level event.
/// </summary>
public class PlayProcessorEventHappenedEventArgs : EventArgs
{
    /// <summary>
    /// The type of the processor level event.
    /// </summary>
    public PlayProcessorEvent ProcessorEvent { get; }

    /// <summary>
    /// A value needed for each processor level event type.
    /// </summary>
    public int NumberArg { get; }

    /// <summary>
    /// Creates a new instance for the <see cref="PlayProcessorEventHappenedEventArgs"/> class.
    /// </summary>
    /// <param name="processorEvent">The type of the processor level event.</param>
    /// <param name="numberArg">A value needed for each processor level event type.</param>
    public PlayProcessorEventHappenedEventArgs(PlayProcessorEvent processorEvent, int numberArg)
    {
        ProcessorEvent = processorEvent;
        NumberArg = numberArg;
    }
}