using System;
using Game2048.Shared.Enums;

namespace Game2048.Shared.EventHandlers;

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