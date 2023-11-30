using ConsoleClient.App.Resources;
using ConsoleClient.Display.Models;
using ConsoleClient.Menu;
using Game2048.Logic;
using System.Collections.Generic;

namespace ConsoleClient.App;

/// <summary>
/// A static class that stores all objects needed for the app and its components.
/// </summary>
internal static class AppEnvironment
{
    /// <summary>
    /// The high level manager for the config.
    /// </summary>
    internal readonly static IConfigLogic ConfigLogic = new ConfigLogic();
    /// <summary>
    /// The high level manager of the backend.
    /// </summary>
    internal readonly static IGameLogic GameLogic = new GameLogic();
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
    internal static IPlayInstance? CurrentPlayInstance;
    /// <summary>
    /// The main menu of the game.
    /// </summary>
    internal static IConsoleMenu MainMenu => CurrentMenus["mainMenu"];

    /// <summary>
    /// Initializes the game.
    /// </summary>
    internal static void Initialize()
    {
        MainMenuProvider.ProvideMainMenu();
    }
}
