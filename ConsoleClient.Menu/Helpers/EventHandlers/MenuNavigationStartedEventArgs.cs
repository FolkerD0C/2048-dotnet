using System;
using System.Collections.Generic;

namespace ConsoleClient.Menu.EventHandlers;

public class MenuNavigationStartedEventArgs : EventArgs
{
    public IList<IMenuItem> MenuItems { get; }
    public int SelectedMenuItem { get; }
    public IList<string>? DisplayText { get; }

    public MenuNavigationStartedEventArgs(IList<IMenuItem> menuItems, int selectedMenuItem, IList<string>? displayText)
    {
        MenuItems = menuItems;
        SelectedMenuItem = selectedMenuItem;
        DisplayText = displayText;
    }
}