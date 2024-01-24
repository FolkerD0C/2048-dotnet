using _2048ish.Base.Models;
using ConsoleClient.App.Resources;
using ConsoleClient.Display;
using ConsoleClient.Display.Models;
using ConsoleClient.Menu;
using Game2048.Managers;
using System.Collections.Generic;

namespace ConsoleClient.App;

/// <summary>
/// A static class that stores all objects needed for the app and its components.
/// </summary>
internal static class AppEnvironment
{
    /// <summary>
    /// The high level manager of the backend.
    /// </summary>
    internal readonly static IGameManager GameManager = new GameManager();
    /// <summary>
    /// The currently active overlays.
    /// </summary>
    internal static readonly Dictionary<string, IOverLay> CurrentOverlays = new();
    /// <summary>
    /// The currently active menus.
    /// </summary>
    internal readonly static Dictionary<string, IConsoleMenu> CurrentMenus = new();
    /// <summary>
    /// The currently active play.
    /// </summary>
    internal static IPlayInstanceManager? CurrentPlayInstanceManager;
    /// <summary>
    /// The main menu of the game.
    /// </summary>
    internal static IConsoleMenu MainMenu => CurrentMenus["mainMenu"];
    /// <summary>
    /// The configuration passed on to new games.
    /// </summary>
    internal static NewGameConfiguration? GameConfiguration;

    /// <summary>
    /// Initializes the game.
    /// </summary>
    internal static void Initialize(NewGameConfiguration gameConfig)
    {
        DisplayManager.Initialize(40, 100);
        MainMenuProvider.ProvideMainMenu();
        GameConfiguration = gameConfig;
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    internal static void Shutdown()
    {
        DisplayManager.EndDsiplay();
    }
}
