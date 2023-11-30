using ConsoleClient.Menu.Enums;
using ConsoleClient.Menu.EventHandlers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Menu;

/// <summary>
/// A class that represents an interactive menu with <see cref="IMenuItem"/>s and optional display text.
/// </summary>
public class ConsoleMenu : IConsoleMenu
{
    /// <summary>
    /// If true then the navigation is active.
    /// </summary>
    bool inNavigation;

    /// <summary>
    /// The <see cref="IMenuItem"/>s that the <see cref="IConsoleMenu"/> contains.
    /// </summary>
    readonly IList<IMenuItem> menuItems;

    /// <summary>
    /// An optional <see cref="IList{string}"/> that stores text that should be displayed.
    /// </summary>
    readonly IList<string>? displayText;

    /// <summary>
    /// An input function for the menu navigation.
    /// </summary>
    readonly Func<MenuInput> inputFunction;

    public event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;
    public event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;
    public event EventHandler? MenuNavigationEnded;
    public event EventHandler? MenuItemReturnedYes;

    /// <summary>
    /// A list of navigation breaker results. If an <see cref="IMenuItem"/> returns a
    /// <see cref="MenuItemResult"/> contained in that list, then the navigation ends.
    /// </summary>
    readonly IList<MenuItemResult> navigationBreakers;

    /// <summary>
    /// Creates a new instance of the <see cref="ConsoleMenu"/> class.
    /// </summary>
    /// <param name="menuItems">The <see cref="IMenuItem"/>s that the <see cref="IConsoleMenu"/> contains.</param>
    /// <param name="inputFunction">An input function for the menu navigation.</param>
    public ConsoleMenu(IList<IMenuItem> menuItems, Func<MenuInput> inputFunction)
    {
        this.menuItems = menuItems;
        this.inputFunction = inputFunction;
        inNavigation = false;
        navigationBreakers = new List<MenuItemResult>()
        {
            MenuItemResult.Back
        };
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ConsoleMenu"/> class with display text.
    /// </summary>
    /// <param name="menuItems">The <see cref="IMenuItem"/>s that the <see cref="IConsoleMenu"/> contains.</param>
    /// <param name="inputFunction">An input function for the menu navigation.</param>
    /// <param name="displayText">A <see cref="IList{string}"/> that stores text that should be displayed.</param>
    public ConsoleMenu(IList<IMenuItem> menuItems, Func<MenuInput> inputFunction, IList<string> displayText) : this(menuItems, inputFunction)
    {
        this.displayText = displayText;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ConsoleMenu"/> class with dummy parameters. Intended for internal use only.
    /// </summary>
    internal ConsoleMenu() : this(new List<IMenuItem>(), () => MenuInput.Unknown)
    { }

    public void AddNavigationBreaker(MenuItemResult navigationBreaker)
    {
        navigationBreakers.Add(navigationBreaker);
    }

    public MenuItemResult Navigate()
    {
        int selectedMenuItemIndex = 0;
        int selectionHelper(int idx) =>
            idx < 0 ? menuItems.Count - 1 : idx >= menuItems.Count ? 0 : idx;
        MenuItemResult menuResult = MenuItemResult.Unknown;
        MenuNavigationStarted?.Invoke(this, new MenuNavigationStartedEventArgs(menuItems, selectedMenuItemIndex, displayText));
        inNavigation = true;
        while (inNavigation)
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
            if (navigationBreakers.Contains(menuResult))
            {
                inNavigation = false;
            }
        }
        MenuNavigationEnded?.Invoke(this, new EventArgs());
        return menuResult;
    }

    public void EndNavigation()
    {
        if (!inNavigation)
        {
            throw new InvalidOperationException("Can not end navigation in menu where there is no navigation.");
        }
        inNavigation = false;
    }
}
