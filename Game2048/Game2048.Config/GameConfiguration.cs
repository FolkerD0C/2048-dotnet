using System;
using System.Collections.Generic;

namespace Game2048.Config;

/// <summary>
/// A static class that contains configuration values. Use <see cref="ConfigRepository"/> to get and set values.
/// </summary>
internal static class GameConfiguration
{
    /// <summary>
    /// The maximum length of the high score list.
    /// </summary>
    internal static int MaxHighscoresListLength = 10;

    /// <summary>
    /// The path of the directory that contains <see cref="GameData.HighscoresFilePath"/>
    /// and <see cref="GameData.SaveGameDirectoryPath"/>.
    /// </summary>
    internal static string GameDataDirectory = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The list that contains the accepted spawnable tiles/numbers.
    /// </summary>
    internal static IList<int> DefaultAcceptedSpawnables = new List<int>() { 2, 4 };

    /// <summary>
    /// The goal of the game. If a player reaches it, they win.
    /// </summary>
    internal static int DefaultGoal = 2048;

    /// <summary>
    /// Maximum lives a player can get when starting a new game.
    /// </summary>
    internal static int DefaultMaxLives = 5;

    /// <summary>
    /// Maximum length of the undo chain a player can reach in a play.
    /// </summary>
    internal static int DefaultMaxUndos = 6;

    /// <summary>
    /// Height of the playing grid that sets a new games playing grid height.
    /// </summary>
    internal static int DefaultGridWidth = 4;

    /// <summary>
    /// Width of the playing grid that sets a new games playing grid width.
    /// </summary>
    internal static int DefaultGridHeight = 4;

    /// <summary>
    /// The number of starter tiles/numbers a player can get when starting a new game.
    /// </summary>
    internal static int DefaultStarterTiles = 2;
}