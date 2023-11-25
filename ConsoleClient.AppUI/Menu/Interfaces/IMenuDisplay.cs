using ConsoleClient.Display;
using ConsoleClient.Menu.EventHandlers;
using System;

namespace ConsoleClient.AppUI.Menu;

public interface IMenuDisplay : IOverLay
{
    void OnMenuNavigationStarted(object? sender, MenuNavigationStartedEventArgs args);
    void OnMenuSelectionChanged(object? sender, MenuSelectionChangedEventArgs args);
    void OnMenuNavigationEnded(object? sender, EventArgs args);
}