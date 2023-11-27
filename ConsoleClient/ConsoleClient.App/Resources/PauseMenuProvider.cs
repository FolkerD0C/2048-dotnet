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
            new MenuItem("Continue", MenuItemResult.Back, new MenuActionRequestedArgs (resultProvider.Continue)),
            new MenuItem("Change player name", new MenuActionRequestedArgs (changePlayerNameMethod)),
            new MenuItem("Save Game", new MenuActionRequestedArgs (saveGameMethod)),
            ProvideEndPlayMenuItem(resultProvider),
            ProvideExitGameMenuItem(resultProvider)
        };
        IConsoleMenu pauseMenu = new ConsoleMenu(pauseMenuItems, InputProvider.ProvideMenuInput);
        // !!!!########!!!!!!!!!!########### handle
        pauseMenu.AddNavigationBreaker(MenuItemResult.Yes);
        // !!!!########!!!!!!!!!!###########
        IMenuDisplay pauseMenuOverlay = new MenuDisplay();
        pauseMenu.MenuNavigationStarted += pauseMenuOverlay.OnMenuNavigationStarted;
        pauseMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("pauseMenu", pauseMenu);
            AppEnvironment.CurrentOverlays.Add("pauseMenuOverlay", pauseMenuOverlay);
        };
        pauseMenu.MenuSelectionChanged += pauseMenuOverlay.OnMenuSelectionChanged;
        pauseMenu.MenuNavigationEnded += pauseMenuOverlay.OnMenuNavigationEnded;
        pauseMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("pauseMenu");
            AppEnvironment.CurrentOverlays.Remove("pauseMenuOverlay");
        };
        pauseMenu.Navigate();
        return resultProvider.Result;
    }

    static IMenuItem ProvideEndPlayMenuItem(PauseMenuActionRequestedResultProvider resultProvider)
    {
        string menuItemName = "Exit to Main Menu";
        IConsoleMenu pauseMenuEndPlayPromptMenu = ProvideEndPlaySubMenu();
        pauseMenuEndPlayPromptMenu.AddNavigationBreaker(MenuItemResult.Yes);
        pauseMenuEndPlayPromptMenu.AddNavigationBreaker(MenuItemResult.No);
        IMenuDisplay pauseMenuEndPlayPromptMenuOverlay = new MenuDisplay();
        pauseMenuEndPlayPromptMenu.MenuNavigationStarted += pauseMenuEndPlayPromptMenuOverlay.OnMenuNavigationStarted;
        pauseMenuEndPlayPromptMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("pauseMenuEndPlayPromptMenu", pauseMenuEndPlayPromptMenu);
            AppEnvironment.CurrentOverlays.Add("pauseMenuEndPlayPromptMenuOverlay", pauseMenuEndPlayPromptMenuOverlay);
        };
        pauseMenuEndPlayPromptMenu.MenuSelectionChanged += pauseMenuEndPlayPromptMenuOverlay.OnMenuSelectionChanged;
        pauseMenuEndPlayPromptMenu.MenuNavigationEnded += pauseMenuEndPlayPromptMenuOverlay.OnMenuNavigationEnded;
        pauseMenuEndPlayPromptMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("pauseMenuEndPlayPromptMenu");
            AppEnvironment.CurrentOverlays.Remove("pauseMenuEndPlayPromptMenuOverlay");
        };
        pauseMenuEndPlayPromptMenu.MenuItemReturnedYes += (sender, args) =>
        {
            resultProvider.EndPlay();
            AppEnvironment.CurrentOverlays["pauseMenuEndPlayPromptMenuOverlay"].SetPreviousOverlaySuppression(true);
            AppEnvironment.CurrentOverlays["pauseMenuOverlay"].SetPreviousOverlaySuppression(true);
            AppEnvironment.CurrentMenus["pauseMenu"].EndNavigation();
        };
        return new MenuItem(menuItemName, new MenuActionRequestedArgs(pauseMenuEndPlayPromptMenu));
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
        IConsoleMenu pauseMenuExitGamePromptMenu = ProvideExitGameSubMenu();
        pauseMenuExitGamePromptMenu.AddNavigationBreaker(MenuItemResult.Yes);
        pauseMenuExitGamePromptMenu.AddNavigationBreaker(MenuItemResult.No);
        IMenuDisplay pauseMenuExitGamePromptMenuOverlay = new MenuDisplay();
        pauseMenuExitGamePromptMenu.MenuNavigationStarted += pauseMenuExitGamePromptMenuOverlay.OnMenuNavigationStarted;
        pauseMenuExitGamePromptMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("pauseMenuExitGamePromptMenu", pauseMenuExitGamePromptMenu);
            AppEnvironment.CurrentOverlays.Add("pauseMenuExitGamePromptMenuOverlay", pauseMenuExitGamePromptMenuOverlay);
        };
        pauseMenuExitGamePromptMenu.MenuSelectionChanged += pauseMenuExitGamePromptMenuOverlay.OnMenuSelectionChanged;
        pauseMenuExitGamePromptMenu.MenuNavigationEnded += pauseMenuExitGamePromptMenuOverlay.OnMenuNavigationEnded;
        pauseMenuExitGamePromptMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("pauseMenuExitGamePromptMenu");
            AppEnvironment.CurrentOverlays.Remove("pauseMenuExitGamePromptMenuOverlay");
        };
        pauseMenuExitGamePromptMenu.MenuItemReturnedYes += (sender, args) =>
        {
            resultProvider.ExitGame();
            AppEnvironment.CurrentOverlays["pauseMenuExitGamePromptMenuOverlay"].SetPreviousOverlaySuppression(true);
            AppEnvironment.CurrentOverlays["pauseMenuOverlay"].SetPreviousOverlaySuppression(true);
            AppEnvironment.CurrentOverlays["currentPlayInstanceOverlay"].SetPreviousOverlaySuppression(true);
            AppEnvironment.CurrentMenus["pauseMenu"].EndNavigation();
        };
        return new MenuItem(menuItemName, new MenuActionRequestedArgs(pauseMenuExitGamePromptMenu));
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
