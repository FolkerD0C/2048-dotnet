using Game2048.ConsoleFrontend.Display;
using Game2048.ConsoleFrontend.Helpers.EventHandlers;
using Game2048.ConsoleFrontend.Models;
using Game2048.ConsoleFrontend.Resources.Menus;
using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Helpers;

public static class MenuDisplayProvider
{
    static IMenuDisplay ProvideMenuDisplay(IList<IMenuItem> menuItems, int selectedMenuItem, IList<string> displayText)
    {
        return new MenuDisplay(menuItems, selectedMenuItem, displayText);
    }

    public static void OnMenuNavigationStarted(object? sender, MenuNavigationStartedEventArgs args)
    {
        if (sender is IMenu menu)
        {
            IMenuDisplay display = ProvideMenuDisplay(args.MenuItems, args.SelectedMenuItem, args.DisplayText);
            menu.MenuSelectionChanged += display.OnMenuSelectionChanged;
            menu.MenuNavigationEnded += display.OnMenuNavigationEnded;
            display.InitializeDisplayRows();
            DisplayManager.NewOverlay(display);
        }
    }
}