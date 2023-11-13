using System;
using Game2048.Backend.Helpers.Enums;

namespace Game2048.Backend.Helpers.EventHandlers;

public class MiscEventHappenedEventArgs : EventArgs
{
    public MiscEvent Event { get; }
    public int NumberArg { get; }
    public MiscEventHappenedEventArgs(MiscEvent Event, int numberArg = -1)
    {
        this.Event = Event;
        NumberArg = numberArg;
    }
}