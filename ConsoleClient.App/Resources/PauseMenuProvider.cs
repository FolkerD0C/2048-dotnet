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

        internal void Resume()
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

    internal static PauseResult ProvidePauseMenuAction(Action saveGameMethod)
    {
        PauseMenuActionRequestedResultProvider resultProvider = new();
        IList<IMenuItem> pauseMenuItems = new List<IMenuItem>
        {
            new MenuItem("Continue", MenuItemResult.Back, new MenuActionRequestedArgs(resultProvider.Resume)),
            new MenuItem("Save Game", new MenuActionRequestedArgs(saveGameMethod)),
            new MenuItem("Exit to Main Menu", MenuItemResult.Back, new MenuActionRequestedArgs(resultProvider.EndPlay)),
            new MenuItem("Quit Game", MenuItemResult.Back, new MenuActionRequestedArgs(resultProvider.ExitGame))
        };
        IConsoleMenu pauseMenu = new ConsoleMenu(pauseMenuItems, InputProvider.ProvideMenuInput);
        IMenuDisplay menuDisplay = new MenuDisplay();
        pauseMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        pauseMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        pauseMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        pauseMenu.Navigate();
        return resultProvider.Result;
    }
}
