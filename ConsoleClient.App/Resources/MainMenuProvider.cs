using ConsoleClient.App.Helpers;
using ConsoleClient.AppUI.Menu;
using ConsoleClient.Display;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.App.Resources;

internal static class MainMenuProvider
{
    static readonly string gameLogicIsNullErrorMessage = "Game logic can not be null";

    internal static IConsoleMenu ProvideMainMenu()
    {
        IList<IMenuItem> mainMenuItems = new List<IMenuItem>()
        {
            ProvideNewGameMenuItem(),
            ProvideLoadGameMenuItem(),
            ProvideHighscoresMenuItem(),
            ProvideGameDescriptionMenuItem(),
            ProvideGameHelpMenuItem(),
            ProvideExitMenuItem()
        };
        IConsoleMenu mainMenu = new ConsoleMenu(mainMenuItems, InputProvider.ProvideMenuInput);
        IMenuDisplay menuDisplay = new MenuDisplay();
        mainMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        mainMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        mainMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        return mainMenu;
    }

    static IMenuItem ProvideNewGameMenuItem()
    {
        string menuItemName = "New Game";
        IMenuActionRequestedArgs menuActionRequestedArgs = new MenuActionRequestedArgs(PlayProvider.ProvideNewGame);
        IMenuItem newGameMenuItem = new MenuItem(menuItemName, menuActionRequestedArgs);
        return newGameMenuItem;
    }

    static IMenuItem ProvideLoadGameMenuItem()
    {
        string menuItemName = "Load Game";
        IMenuActionRequestedArgs menuActionRequestedArgs = new MenuActionRequestedArgs(ProvideLoadGameSubMenuAction);
        IMenuItem loadGameMenuItem = new MenuItem(menuItemName, menuActionRequestedArgs);
        return loadGameMenuItem;
    }

    static void ProvideLoadGameSubMenuAction()
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException(gameLogicIsNullErrorMessage);
        }
        IEnumerable<string> savedGameNames = AppEnvironment.GameLogic.GetSavedGames();
        IList<IMenuItem> loadGameMenuItems = new List<IMenuItem>();
        foreach (string savedGameName in savedGameNames)
        {
            IMenuActionRequestedArgs menuActionRequestedArgs = new MenuActionRequestedArgs(PlayProvider.ProvideLoadedGame, savedGameName);
            loadGameMenuItems.Add(new MenuItem(savedGameName, menuActionRequestedArgs));
        }
        loadGameMenuItems.Add(new MenuItem("Back"));
        IConsoleMenu loadGameMenu = new ConsoleMenu(loadGameMenuItems, InputProvider.ProvideMenuInput);
        IMenuDisplay menuDisplay = new MenuDisplay();
        loadGameMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        loadGameMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        loadGameMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        loadGameMenu.Navigate();
    }

    static IMenuItem ProvideHighscoresMenuItem()
    {
        string menuItemName = "Highscores";
        IMenuActionRequestedArgs menuActionRequestedArgs = new MenuActionRequestedArgs(ProvideHighscoresSubMenuAction);
        IMenuItem highscoresMenuItem = new MenuItem(menuItemName, menuActionRequestedArgs);
        return highscoresMenuItem;
    }

    static void ProvideHighscoresSubMenuAction()
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException(gameLogicIsNullErrorMessage);
        }
        IMenuItem back = new MenuItem("Back");
        IList<IHighscore> highscores = AppEnvironment.GameLogic.GetHighscores();
        IList<string> displayText = new List<string>();
        foreach (var highscore in highscores)
        {
            displayText.Add(string.Format("<{0}>: [{1}]", highscore.PlayerName, highscore.PlayerScore));
        }
        IConsoleMenu highscoresMenu = new ConsoleMenu(new List<IMenuItem>() { back }, InputProvider.ProvideMenuInput, displayText);
        IMenuDisplay menuDisplay = new MenuDisplay();
        highscoresMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        highscoresMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        highscoresMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        highscoresMenu.Navigate();
    }

    static IMenuItem ProvideGameDescriptionMenuItem()
    {
        string menuItemName = "Game Description";
        IMenuActionRequestedArgs menuActionRequestedArgs = new MenuActionRequestedArgs(ProvideGameDescriptionSubMenuAction);
        IMenuItem gameDescriptionMenuItem = new MenuItem(menuItemName, menuActionRequestedArgs);
        return gameDescriptionMenuItem;
    }

    static void ProvideGameDescriptionSubMenuAction()
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException(gameLogicIsNullErrorMessage);
        }
        IMenuItem back = new MenuItem("Back");
        string gameDescription = AppEnvironment.GameLogic.GetGameDescription();
        IList<string> displayText = gameDescription.Slice((DisplayManager.Width * 2) / 3);
        IConsoleMenu gameDescriptionMenu = new ConsoleMenu(new List<IMenuItem>() { back }, InputProvider.ProvideMenuInput, displayText);
        IMenuDisplay menuDisplay = new MenuDisplay();
        gameDescriptionMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        gameDescriptionMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        gameDescriptionMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        gameDescriptionMenu.Navigate();
    }

    static IMenuItem ProvideGameHelpMenuItem()
    {
        string menuItemName = "Game Help";
        IMenuActionRequestedArgs menuActionRequestedArgs = new MenuActionRequestedArgs(ProvideGameHelpSubMenuAction);
        IMenuItem gameHelpMenuItem = new MenuItem(menuItemName, menuActionRequestedArgs);
        return gameHelpMenuItem;
    }

    static void ProvideGameHelpSubMenuAction()
    {
        if (AppEnvironment.GameLogic is null)
        {
            throw new NullReferenceException(gameLogicIsNullErrorMessage);
        }
        IMenuItem back = new MenuItem("Back");
        string gameHelp = AppEnvironment.GameLogic.GetGameHelp();
        IList<string> displayText = gameHelp.Slice((DisplayManager.Width * 2) / 3);
        IConsoleMenu gameHelpMenu = new ConsoleMenu(new List<IMenuItem>() { back }, InputProvider.ProvideMenuInput, displayText);
        IMenuDisplay menuDisplay = new MenuDisplay();
        gameHelpMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        gameHelpMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        gameHelpMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        gameHelpMenu.Navigate();
    }

    static IMenuItem ProvideExitMenuItem()
    {
        string menuItemName = "Guit Game";
        var exitGameSubMenu = ProvideExitGameSubMenu();
        exitGameSubMenu.AddNavigationBreaker(MenuItemResult.Yes);
        exitGameSubMenu.AddNavigationBreaker(MenuItemResult.No);
        IMenuDisplay menuDisplay = new MenuDisplay();
        exitGameSubMenu.MenuNavigationStarted += menuDisplay.OnMenuNavigationStarted;
        exitGameSubMenu.MenuSelectionChanged += menuDisplay.OnMenuSelectionChanged;
        exitGameSubMenu.MenuNavigationEnded += menuDisplay.OnMenuNavigationEnded;
        exitGameSubMenu.MenuItemReturnedYes += (sender, args) =>
        {
            AppEnvironment.MainMenu.EndNavigation();
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
            "Are you sure you want to exit?"
        };
        return new ConsoleMenu(exitGameSubMenuItems, InputProvider.ProvideMenuInput, displayText);
    }
}
