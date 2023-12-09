using ConsoleClient.AppUI.Menu;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Logic.Enums;
using System;
using System.Collections.Generic;

namespace ConsoleClient.App.Resources;

/// <summary>
/// A static class that provides a pause menu for a play action.
/// </summary>
internal static class PauseMenuProvider
{
    /// <summary>
    /// A class that can store  the result of a pause action.
    /// </summary>
    class PauseMenuActionRequestedResultProvider
    {
        /// <summary>
        /// The result of the  pause action.
        /// </summary>
        internal PauseResult Result { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="PauseMenuActionRequestedResultProvider"/> class.
        /// </summary>
        public PauseMenuActionRequestedResultProvider()
        {
            Result = PauseResult.Unknown;
        }

        /// <summary>
        /// Sets the <see cref="Result"/> to <see cref="PauseResult.Continue"/>.
        /// </summary>
        internal void Continue()
        {
            Result = PauseResult.Continue;
        }

        /// <summary>
        /// Sets the <see cref="Result"/> to <see cref="PauseResult.EndPlay"/>.
        /// </summary>
        internal void EndPlay()
        {
            Result = PauseResult.EndPlay;
        }

        /// <summary>
        /// Sets the <see cref="Result"/> to <see cref="PauseResult.ExitGame"/>.
        /// </summary>
        internal void ExitGame()
        {
            Result = PauseResult.ExitGame;
        }
    }

    /// <summary>
    /// Provides a pause menu action for a play action.
    /// </summary>
    /// <param name="changePlayerNameMethod">A method for changing a player's name.</param>
    /// <param name="saveGameMethod">A method for saving the game.</param>
    /// <returns></returns>
    internal static PauseResult ProvidePauseMenuAction(Action changePlayerNameMethod, Action saveGameMethod)
    {
        PauseMenuActionRequestedResultProvider resultProvider = new();
        IList<IMenuItem> pauseMenuItems = new List<IMenuItem>
        {
            new MenuItem("Continue", MenuItemResult.Back, new MenuItemActionRequestedArgs (resultProvider.Continue)),
            new MenuItem("Change player name", new MenuItemActionRequestedArgs (changePlayerNameMethod)),
            new MenuItem("Save Game", new MenuItemActionRequestedArgs (saveGameMethod)),
            ProvideEndPlayMenuItem(resultProvider),
            ProvideExitGameMenuItem(resultProvider)
        };
        IConsoleMenu pauseMenu = new ConsoleMenu(pauseMenuItems, InputProvider.ProvideMenuInput);
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

    /// <summary>
    /// Provides a menu item for exiting to the main menu.
    /// </summary>
    /// <param name="resultProvider">The result provider for the pause menu.</param>
    /// <returns>A menu item that starts a menu navigation for the exit play prompt menu if selected.</returns>
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
        return new MenuItem(menuItemName, new MenuItemActionRequestedArgs(pauseMenuEndPlayPromptMenu));
    }

    /// <summary>
    /// Provides a menu for the exit play prompt menu.
    /// </summary>
    /// <returns>A menu for the exit play prompt menu.</returns>
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

    /// <summary>
    /// Provides a menu item for the exit game prompt menu.
    /// </summary>
    /// <param name="resultProvider">The result provider for the pause menu.</param>
    /// <returns>A menu item that starts a menu navigation for the exit game prompt menu if selected.</returns>
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
        return new MenuItem(menuItemName, new MenuItemActionRequestedArgs(pauseMenuExitGamePromptMenu));
    }

    /// <summary>
    /// Provides a menu for the exit game prompt menu.
    /// </summary>
    /// <returns>A menu for the exit game prompt menu.</returns>
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
