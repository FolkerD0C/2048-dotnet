using System;
using System.Collections.Generic;
using Game2048.ConsoleFrontend.Helpers.Enums;
using Game2048.ConsoleFrontend.Helpers.EventHandlers;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Resources.Menus;

public interface IMenu
{
    IList<IMenuItem> MenuItems { get; }
    MenuItemResult Navigate();
    event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;
    event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;
    event EventHandler<MenuNavigationEndedEventArgs>? MenuNavigationEnded;
}