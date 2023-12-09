using ConsoleClient.AppUI.Helpers;
using ConsoleClient.AppUI.Menu;
using ConsoleClient.Display;
using ConsoleClient.Menu;
using ConsoleClient.Menu.Enums;
using 2048ish.Base.Models;
using System.Collections.Generic;

namespace ConsoleClient.App.Resources;

/// <summary>
/// A static class that provides a main menu for the game.
/// </summary>
internal static class MainMenuProvider
{
    /// <summary>
    /// Provides a main menu for the game.
    /// </summary>
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

    /// <summary>
    /// Provides a menu item for new games.
    /// </summary>
    /// <returns>A menu item that starts a new game if selected.</returns>
    static IMenuItem ProvideNewGameMenuItem()
    {
        string menuItemName = "New Game";
        IMenuItem newGameMenuItem = new MenuItem(menuItemName, new MenuItemActionRequestedArgs(PlayProvider.ProvideNewGame));
        return newGameMenuItem;
    }

    /// <summary>
    /// Provides a menu item for loaded games.
    /// </summary>
    /// <returns>A menu item that starts a submenu navigation for the load game menu if selected.</returns>
    static IMenuItem ProvideLoadGameMenuItem()
    {
        string menuItemName = "Load Game";
        IMenuItem loadGameMenuItem = new MenuItem(menuItemName, new MenuItemActionRequestedArgs(ProvideLoadGameSubMenuAction));
        return loadGameMenuItem;
    }

    /// <summary>
    /// Provides a submenu to load games.
    /// </summary>
    static void ProvideLoadGameSubMenuAction()
    {
        IEnumerable<string> savedGameNames = AppEnvironment.GameManager.GetSavedGames();
        IList<IMenuItem> loadGameMenuItems = new List<IMenuItem>();
        foreach (string savedGameName in savedGameNames)
        {
            loadGameMenuItems.Add(new MenuItem(savedGameName, MenuItemResult.Back, new MenuItemActionRequestedArgs(PlayProvider.ProvideLoadedGame, savedGameName)));
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

    /// <summary>
    /// Provides a menu item for a high scores submenu.
    /// </summary>
    /// <returns>A menu item that starts a submenu navigation for the high scores menu if selected.</returns>
    static IMenuItem ProvideHighscoresMenuItem()
    {
        string menuItemName = "Highscores";
        IMenuItem highscoresMenuItem = new MenuItem(menuItemName, new MenuItemActionRequestedArgs(ProvideHighscoresSubMenuAction));
        return highscoresMenuItem;
    }

    /// <summary>
    /// Provides a submenu to view high scores.
    /// </summary>
    static void ProvideHighscoresSubMenuAction()
    {
        IMenuItem back = new MenuItem("Back");
        IList<Highscore> highscores = AppEnvironment.GameManager.GetHighscores();
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

    /// <summary>
    /// Provides a menu item for the game description submenu.
    /// </summary>
    /// <returns>A menu item that starts a submenu navigation for the game description submenu if selected.</returns>
    static IMenuItem ProvideGameDescriptionMenuItem()
    {
        string menuItemName = "Game Description";
        IMenuItem gameDescriptionMenuItem = new MenuItem(menuItemName, new MenuItemActionRequestedArgs(ProvideGameDescriptionSubMenuAction));
        return gameDescriptionMenuItem;
    }

    /// <summary>
    /// Provides a submenu to view the game description.
    /// </summary>
    static void ProvideGameDescriptionSubMenuAction()
    {
        IMenuItem back = new MenuItem("Back");
        string gameDescription = AppEnvironment.GameManager.GetGameDescription();
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

    /// <summary>
    /// Provides a menu item for the exit game prompt menu.
    /// </summary>
    /// <returns>A menu item that starts a menu navigation for the exit game prompt menu if selected.</returns>
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
        return new MenuItem(menuItemName, new MenuItemActionRequestedArgs(exitGamePromptMenu));
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
            "Are you sure you want to exit?"
        };
        return new ConsoleMenu(exitGameSubMenuItems, InputProvider.ProvideMenuInput, displayText);
    }
}
