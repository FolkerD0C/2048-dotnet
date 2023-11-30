using System;
using System.Collections.Generic;

namespace ConsoleClient.Menu.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about the starting of the navigation of an <see cref="IConsoleMenu"/> object.
/// </summary>
public class MenuNavigationStartedEventArgs : EventArgs
{
    /// <summary>
    /// The <see cref="IMenuItem"/>s that the <see cref="IConsoleMenu"/> contains.
    /// </summary>
    public IList<IMenuItem> MenuItems { get; }

    /// <summary>
    /// The index of the selected <see cref="IMenuItem"/> at the navigation start.
    /// </summary>
    public int SelectedMenuItem { get; }

    /// <summary>
    /// An optional <see cref="IList{string}"/> that stores text that should be displayed.
    /// </summary>
    public IList<string>? DisplayText { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="MenuNavigationStartedEventArgs"/> class.
    /// </summary>
    /// <param name="menuItems">The <see cref="IMenuItem"/>s that the <see cref="IConsoleMenu"/> contains.</param>
    /// <param name="selectedMenuItem">The index of the selected <see cref="IMenuItem"/> at the navigation start.</param>
    /// <param name="displayText">An optional <see cref="IList{string}"/> that stores text that sholud be displayed.</param>
    public MenuNavigationStartedEventArgs(IList<IMenuItem> menuItems, int selectedMenuItem, IList<string>? displayText)
    {
        MenuItems = menuItems;
        SelectedMenuItem = selectedMenuItem;
        DisplayText = displayText;
    }
}