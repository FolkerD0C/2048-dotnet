using System;

namespace ConsoleClient.Menu.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about a change of selected <see cref="IMenuItem"/> during the navigation of aN <see cref="IConsoleMenu"/> object.
/// </summary>
public class MenuSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// The index of the previously selected <see cref="IMenuItem"/> object.
    /// </summary>
    public int PreviousItem { get; }

    /// <summary>
    /// The index of the newly selected <see cref="IMenuItem"/> object.
    /// </summary>
    public int NewItem { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="MenuSelectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="previousItem">The index of the previously selected <see cref="IMenuItem"/> object.</param>
    /// <param name="newItem">The index of the newly selected <see cref="IMenuItem"/> object.</param>
    public MenuSelectionChangedEventArgs(int previousItem, int newItem)
    {
        PreviousItem = previousItem;
        NewItem = newItem;
    }
}