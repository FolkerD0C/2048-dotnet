using System;
using System.Collections.Generic;
using Game2048.ConsoleFrontend.Helpers;
using Game2048.ConsoleFrontend.Helpers.Enums;
using Game2048.ConsoleFrontend.Helpers.EventHandlers;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Resources.Menus;

public class Menu : IMenu
{
    readonly IList<IMenuItem> menuItems;
    public IList<IMenuItem> MenuItems => menuItems;

    public event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;
    public event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;
    public event EventHandler<MenuNavigationEndedEventArgs>? MenuNavigationEnded;

    public Menu(IList<IMenuItem> menuItems)
    {
        this.menuItems = menuItems;
        MenuNavigationStarted += MenuDisplayProvider.OnMenuNavigationStarted;
    }

    public Menu() : this(new List<IMenuItem>())
    { }

    public MenuItemResult Navigate()
    {
        int selectedMenuItemIndex = 0;
        int selectionHelper(int idx) =>
            idx < 0 ? menuItems.Count - 1 : idx >= menuItems.Count ? 0 : idx;
        MenuItemResult menuResult = MenuItemResult.Unknown;
        MenuNavigationStarted?.Invoke(this, new MenuNavigationStartedEventArgs(menuItems, selectedMenuItemIndex, new List<string>()));
        while (menuResult != MenuItemResult.Back)
        {
            switch (InputProvider.ProvideMenuInput())
            {
                case MenuInput.Up:
                {
                    int previoslySelectedMenuItemIndex = selectedMenuItemIndex;
                    selectedMenuItemIndex = selectionHelper(selectedMenuItemIndex - 1);
                    MenuSelectionChanged?.Invoke(this, new MenuSelectionChangedEventArgs(previoslySelectedMenuItemIndex, selectedMenuItemIndex));
                    break;
                }
                case MenuInput.Down:
                {
                    int previoslySelectedMenuItemIndex = selectedMenuItemIndex;
                    selectedMenuItemIndex = selectionHelper(selectedMenuItemIndex + 1);
                    MenuSelectionChanged?.Invoke(this, new MenuSelectionChangedEventArgs(previoslySelectedMenuItemIndex, selectedMenuItemIndex));
                    break;
                }
                case MenuInput.Select:
                {
                    menuResult = menuItems[selectedMenuItemIndex].Select();
                    break;
                }
                default:
                    break;
            }
        }
        MenuNavigationEnded?.Invoke(this, new MenuNavigationEndedEventArgs());
        return menuResult;
    }
}
