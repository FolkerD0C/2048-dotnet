using ConsoleClient.App.Resources;
using ConsoleClient.Display;
using ConsoleClient.Menu;
using Game2048.Logic;
using System.Collections.Generic;

namespace ConsoleClient.App;

internal static class AppEnvironment
{
    internal readonly static IConfigLogic ConfigLogic = new ConfigLogic();
    internal readonly static IGameLogic GameLogic = new GameLogic();
    internal static readonly Dictionary<string, IOverLay> CurrentOverlays = new();
    internal readonly static Dictionary<string, IConsoleMenu> CurrentMenus = new();
    internal static IPlayInstance? CurrentPlayInstance;
    internal static IConsoleMenu MainMenu => CurrentMenus["mainMenu"];

    internal static void Initialize()
    {
        MainMenuProvider.ProvideMainMenu();
    }
}
