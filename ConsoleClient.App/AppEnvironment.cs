using Game2048.Logic;

namespace ConsoleClient.App;

internal static class AppEnvironment
{
    internal static IGameLogic? GameLogic;

    internal static void Initialize()
    {
        GameLogic = new GameLogic();
    }
}
