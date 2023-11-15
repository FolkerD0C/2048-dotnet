using ConsoleClient.Menu.Enums;
using ConsoleClient.Menu.EventHandlers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Menu;

public class Menu : IMenu
{
    readonly IList<IMenuItem> menuItems;
    public IList<IMenuItem> MenuItems => menuItems;

    readonly IList<string>? displayText;
    public IList<string>? DisplayText => displayText;

    readonly Func<MenuInput> inputFunction;

    public event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;
    public event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;
    public event EventHandler? MenuNavigationEnded;
    public event EventHandler? MenuItemReturnedYes;

    public Menu(IList<IMenuItem> menuItems, Func<MenuInput> inputFunction)
    {
        this.menuItems = menuItems;
        this.inputFunction = inputFunction;
    }

    public Menu(IList<IMenuItem> menuItems, Func<MenuInput> inputFunction, IList<string> displayText) : this(menuItems, inputFunction)
    {
        this.displayText = displayText;
    }

    internal Menu() : this(new List<IMenuItem>(), () => MenuInput.Unknown)
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
            switch (inputFunction())
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
            if (menuItems[selectedMenuItemIndex].ItemType == MenuItemType.YesNo && menuResult == MenuItemResult.Yes)
            {
                MenuItemReturnedYes?.Invoke(this, new EventArgs());
            }
        }
        MenuNavigationEnded?.Invoke(this, new EventArgs());
        return menuResult;
    }
}
