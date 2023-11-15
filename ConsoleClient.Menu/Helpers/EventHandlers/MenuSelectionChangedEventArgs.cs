using System;

namespace ConsoleClient.Menu.EventHandlers;

public class MenuSelectionChangedEventArgs : EventArgs
{
    public int PreviousItem { get; }
    public int NewItem { get; }

    public MenuSelectionChangedEventArgs(int previousItem, int newItem)
    {
        PreviousItem = previousItem;
        NewItem = newItem;
    }
}