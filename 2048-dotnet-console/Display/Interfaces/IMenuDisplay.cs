using Game2048.ConsoleFrontend.Helpers.EventHandlers;
using Game2048.ConsoleFrontend.Models;
using System.Collections.Generic;

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