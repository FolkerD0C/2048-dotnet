using ConsoleClient.App.Resources;
using ConsoleClient.Menu;
using Game2048.Logic;

namespace ConsoleClient.App;

internal static class AppEnvironment
{
    internal readonly static IConfigLogic ConfigLogic = new ConfigLogic();
    internal readonly static IGameLogic GameLogic = new GameLogic();
    internal readonly static IConsoleMenu MainMenu = MainMenuProvider.ProvideMainMenu();
}
