using ConsoleClient.Menu.Enums;
using ConsoleClient.Menu.EventHandlers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Menu;

public interface IMenu
{
    IList<IMenuItem> MenuItems { get; }
    IList<string>? DisplayText { get; }
    MenuItemResult Navigate();
    event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;
    event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;
    event EventHandler? MenuNavigationEnded;
    event EventHandler? MenuItemReturnedYes;
}