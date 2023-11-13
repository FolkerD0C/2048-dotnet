using System.Collections.Generic;
using Game2048.ConsoleFrontend.Helpers.EventHandlers;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Display;

public interface IMenuDisplay : IOverLay
{
    IList<IMenuItem> MenuItems { get; }
    int SelectedMenuItem { get; }
    IList<string> DisplayText { get; }
    void InitializeDisplayRows();
    void OnMenuSelectionChanged(object? sender, MenuSelectionChangedEventArgs args);
    void OnMenuNavigationEnded(object? sender, MenuNavigationEndedEventArgs args);
}