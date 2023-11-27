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

    internal static void ProvideMainMenu()
    {
        IList<IMenuItem> mainMenuItems = new List<IMenuItem>()
        {
            ProvideNewGameMenuItem(),
            ProvideLoadGameMenuItem(),
            ProvideHighscoresMenuItem(),
            ProvideGameDescriptionMenuItem(),
            ProvideExitMenuItem()
        };
        IConsoleMenu mainMenu = new ConsoleMenu(mainMenuItems, InputProvider.ProvideMenuInput);
        IMenuDisplay mainMenuOverlay = new MenuDisplay();
        mainMenu.MenuNavigationStarted += mainMenuOverlay.OnMenuNavigationStarted;
        mainMenu.MenuSelectionChanged += mainMenuOverlay.OnMenuSelectionChanged;
        mainMenu.MenuNavigationEnded += mainMenuOverlay.OnMenuNavigationEnded;
        mainMenuOverlay.SetPreviousOverlaySuppression(true);
        AppEnvironment.CurrentMenus.Add("mainMenu", mainMenu);
        AppEnvironment.CurrentOverlays.Add("mainMenuOverlay", mainMenuOverlay);
    }

    static IMenuItem ProvideNewGameMenuItem()
    {
        string menuItemName = "New Game";
        IMenuItem newGameMenuItem = new MenuItem(menuItemName, new MenuActionRequestedArgs(PlayProvider.ProvideNewGame));
        return newGameMenuItem;
    }

    static IMenuItem ProvideLoadGameMenuItem()
    {
        string menuItemName = "Load Game";
        IMenuItem loadGameMenuItem = new MenuItem(menuItemName, new MenuActionRequestedArgs(ProvideLoadGameSubMenuAction));
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
            loadGameMenuItems.Add(new MenuItem(savedGameName, MenuItemResult.Back, new MenuActionRequestedArgs(PlayProvider.ProvideLoadedGame, savedGameName)));
        }
        loadGameMenuItems.Add(new MenuItem("Back"));
        IConsoleMenu loadGameMenu = new ConsoleMenu(loadGameMenuItems, InputProvider.ProvideMenuInput);
        IMenuDisplay loadGameMenuOverlay = new MenuDisplay();
        loadGameMenu.MenuNavigationStarted += loadGameMenuOverlay.OnMenuNavigationStarted;
        loadGameMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("loadGameMenu", loadGameMenu);
            AppEnvironment.CurrentOverlays.Add("loadGameMenuOverlay", loadGameMenuOverlay);
        };
        loadGameMenu.MenuSelectionChanged += loadGameMenuOverlay.OnMenuSelectionChanged;
        loadGameMenu.MenuNavigationEnded += loadGameMenuOverlay.OnMenuNavigationEnded;
        loadGameMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("loadGameMenu");
            AppEnvironment.CurrentOverlays.Remove("loadGameMenuOverlay");
        };
        loadGameMenu.Navigate();
    }

    static IMenuItem ProvideHighscoresMenuItem()
    {
        string menuItemName = "Highscores";
        IMenuItem highscoresMenuItem = new MenuItem(menuItemName, new MenuActionRequestedArgs(ProvideHighscoresSubMenuAction));
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
        IMenuDisplay highscoresMenuOverlay = new MenuDisplay();
        highscoresMenu.MenuNavigationStarted += highscoresMenuOverlay.OnMenuNavigationStarted;
        highscoresMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("highscoresMenu", highscoresMenu);
            AppEnvironment.CurrentOverlays.Add("highscoresMenuOverlay", highscoresMenuOverlay);
        };
        highscoresMenu.MenuSelectionChanged += highscoresMenuOverlay.OnMenuSelectionChanged;
        highscoresMenu.MenuNavigationEnded += highscoresMenuOverlay.OnMenuNavigationEnded;
        highscoresMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("highscoresMenu");
            AppEnvironment.CurrentOverlays.Remove("highscoresMenuOverlay");
        };
        highscoresMenu.Navigate();
    }

    static IMenuItem ProvideGameDescriptionMenuItem()
    {
        string menuItemName = "Game Description";
        IMenuItem gameDescriptionMenuItem = new MenuItem(menuItemName, new MenuActionRequestedArgs(ProvideGameDescriptionSubMenuAction));
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
        IMenuDisplay gameDescriptionMenuOverlay = new MenuDisplay();
        gameDescriptionMenu.MenuNavigationStarted += gameDescriptionMenuOverlay.OnMenuNavigationStarted;
        gameDescriptionMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("gameDescriptionMenu", gameDescriptionMenu);
            AppEnvironment.CurrentOverlays.Add("gameDescriptionMenuOverlay", gameDescriptionMenuOverlay);
        };
        gameDescriptionMenu.MenuSelectionChanged += gameDescriptionMenuOverlay.OnMenuSelectionChanged;
        gameDescriptionMenu.MenuNavigationEnded += gameDescriptionMenuOverlay.OnMenuNavigationEnded;
        gameDescriptionMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("gameDescriptionMenu");
            AppEnvironment.CurrentOverlays.Remove("gameDescriptionMenuOverlay");
        };
        gameDescriptionMenu.Navigate();
    }

    static IMenuItem ProvideExitMenuItem()
    {
        string menuItemName = "Guit Game";
        var exitGamePromptMenu = ProvideExitGameSubMenu();
        exitGamePromptMenu.AddNavigationBreaker(MenuItemResult.Yes);
        exitGamePromptMenu.AddNavigationBreaker(MenuItemResult.No);
        IMenuDisplay exitGamePromptMenuOverlay = new MenuDisplay();
        exitGamePromptMenu.MenuNavigationStarted += exitGamePromptMenuOverlay.OnMenuNavigationStarted;
        exitGamePromptMenu.MenuNavigationStarted += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Add("mainMenuExitPromptMenu", exitGamePromptMenu);
            AppEnvironment.CurrentOverlays.Add("mainMenuExitPromptMenuOverlay", exitGamePromptMenuOverlay);
        };
        exitGamePromptMenu.MenuSelectionChanged += exitGamePromptMenuOverlay.OnMenuSelectionChanged;
        exitGamePromptMenu.MenuNavigationEnded += exitGamePromptMenuOverlay.OnMenuNavigationEnded;
        exitGamePromptMenu.MenuNavigationEnded += (sender, args) =>
        {
            AppEnvironment.CurrentMenus.Remove("mainMenuExitPromptMenu");
            AppEnvironment.CurrentOverlays.Remove("mainMenuExitPromptMenuOverlay");
        };
        exitGamePromptMenu.MenuItemReturnedYes += (sender, args) =>
        {
            AppEnvironment.CurrentMenus["mainMenu"].EndNavigation();
            AppEnvironment.CurrentOverlays["mainMenuExitPromptMenuOverlay"].SetPreviousOverlaySuppression(true);
        };
        return new MenuItem(menuItemName, new MenuActionRequestedArgs(exitGamePromptMenu));
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
