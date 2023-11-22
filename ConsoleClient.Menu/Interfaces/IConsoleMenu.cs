using ConsoleClient.Menu.Enums;
using ConsoleClient.Menu.EventHandlers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Menu;

public interface IConsoleMenu
{
    IList<IMenuItem> MenuItems { get; }
    IList<string>? DisplayText { get; }
    MenuItemResult Navigate();
    void EndNavigation();
    event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;
    event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;
    event EventHandler? MenuNavigationEnded;
    event EventHandler? MenuItemReturnedYes;
}