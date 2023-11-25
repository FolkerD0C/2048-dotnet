using ConsoleClient.AppUI.Menu;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Shared.Enums;
using System;
using System.Collections.Generic;

namespace ConsoleClient.App.Resources;

internal static class PauseMenuProvider
{
    class PauseMenuActionRequestedResultProvider
    {
        internal PauseResult Result { get; private set; }

        public PauseMenuActionRequestedResultProvider()
        {
            Result = PauseResult.Unknown;
        }

        internal void Continue()
        {
            Result = PauseResult.Continue;
        }

        internal void EndPlay()
        {
            Result = PauseResult.EndPlay;
        }

        internal void ExitGame()
        {
            Result = PauseResult.ExitGame;
        }
    }

    internal static PauseResult ProvidePauseMenuAction(Action changePlayerNameMethod, Action saveGameMethod)
    {
        PauseMenuActionRequestedResultProvider resultProvider = new();
        IList<IMenuItem> pauseMenuItems = new List<IMenuItem>
        {
            new MenuItem("Continue", MenuItemResult.Back, new MenuActionRequestedArgs(resultProvider.Continue)),
            new MenuItem("Change player name", new MenuActionRequestedArgs(changePlayerNameMethod)),
            new MenuItem("Save Game", new MenuActionRequestedArgs(saveGameMethod)),
            ProvideEndPlayMenuItem(resultProvider),
            ProvideExitGameMenuItem(resultProvider)
        };
        IConsoleMenu pauseMenu = new ConsoleMenu(pauseMenuItems, InputProvider.ProvideMenuInput);
        pauseMenu.AddNavigationBreaker(MenuItemResult.Yes);
        IMenuDisplay menuDisplay = new MenuDisplay();
        pauseMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        pauseMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        pauseMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        pauseMenu.Navigate();
        return resultProvider.Result;
    }

    static IMenuItem ProvideEndPlayMenuItem(PauseMenuActionRequestedResultProvider resultProvider)
    {
        string menuItemName = "Exit to Main Menu";
        var endPlaySubMenu = ProvideEndPlaySubMenu();
        endPlaySubMenu.AddNavigationBreaker(MenuItemResult.Yes);
        endPlaySubMenu.AddNavigationBreaker(MenuItemResult.No);
        var menuDisplay = new MenuDisplay();
        endPlaySubMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        endPlaySubMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        endPlaySubMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        endPlaySubMenu.MenuItemReturnedYes += (sender, args) =>
        {
            resultProvider.EndPlay();
        };
        return new MenuItem(menuItemName, new MenuActionRequestedArgs(endPlaySubMenu));
    }

    static IConsoleMenu ProvideEndPlaySubMenu()
    {
        IList<IMenuItem> endPlaySubMenuItems = new List<IMenuItem>()
        {
            new MenuItem("Yes", MenuItemResult.Yes),
            new MenuItem("No", MenuItemResult.No)
        };
        IList<string> displayText = new List<string>()
        {
            "Are you sure you want to exit to Main Menu?"
        };
        return new ConsoleMenu(endPlaySubMenuItems, InputProvider.ProvideMenuInput, displayText);
    }

    static IMenuItem ProvideExitGameMenuItem(PauseMenuActionRequestedResultProvider resultProvider)
    {
        string menuItemName = "Guit Game";
        var exitGameSubMenu = ProvideExitGameSubMenu();
        exitGameSubMenu.AddNavigationBreaker(MenuItemResult.Yes);
        exitGameSubMenu.AddNavigationBreaker(MenuItemResult.No);
        var menuDisplay = new MenuDisplay();
        exitGameSubMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        exitGameSubMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        exitGameSubMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        exitGameSubMenu.MenuItemReturnedYes += (sender, args) =>
        {
            resultProvider.ExitGame();
        };
        return new MenuItem(menuItemName, new MenuActionRequestedArgs(exitGameSubMenu));
    }

    static IConsoleMenu ProvideExitGameSubMenu()
    {
        IList<IMenuItem> exitGameSubMenuItems = new List<IMenuItem>()
        {
            new MenuItem("Yes", MenuItemResult.Yes),
            new MenuItem("No", MenuItemResult.No)
        };
        IList<string> displayText = new List<string>()
        {
            "Are you sure you want to quit the Game?"
        };
        return new ConsoleMenu(exitGameSubMenuItems, InputProvider.ProvideMenuInput, displayText);
    }
}
