using System.Collections.Generic;

namespace Game2048.Config;

internal static class GameConfiguration
{
    internal static int MaxHighscoresListLength = 10;

    internal static string? GameDataDirectory = null;

    internal static IList<int> DefaultAcceptedSpawnables = new List<int>() { 2, 4 };

    internal static int DefaultGoal = 2048;

    internal static int DefaultMaxLives = 5;

    internal static int DefaultMaxUndos = 6;

    internal static int DefaultGridWidth = 4;

    internal static int DefaultGridHeight = 4;
}