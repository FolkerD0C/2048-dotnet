using System;
using System.Collections.Generic;

namespace Game2048.Config;

/// <summary>
/// A static class that contains configuration values.
/// </summary>
public static class GameConfiguration
{
#pragma warning disable CA2211
    /// <summary>
    /// The maximum length of the high score list.
    /// </summary>
    public static int MaxHighscoresListLength = 10;

    /// <summary>
    /// The path of the directory that contains <see cref="GameData.HighscoresFilePath"/>
    /// and <see cref="GameData.SaveGameDirectoryPath"/>.
    /// </summary>
    public static string GameDataDirectory = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The list that contains the accepted spawnable tiles/numbers.
    /// </summary>
    public static List<int> DefaultAcceptedSpawnables = new() { 2, 4 };

    /// <summary>
    /// The goal of the game. If a player reaches it, they win.
    /// </summary>
    public static int DefaultGoal = 2048;

    /// <summary>
    /// Maximum lives a player can get when starting a new game.
    /// </summary>
    public static int DefaultMaxLives = 5;

    /// <summary>
    /// Maximum length of the undo chain a player can reach in a play.
    /// </summary>
    public static int DefaultMaxUndos = 6;

    /// <summary>
    /// Width of the playing grid that sets a new games playing grid width.
    /// </summary>
    public static int DefaultGridHeight = 4;

    /// <summary>
    /// Height of the playing grid that sets a new games playing grid height.
    /// </summary>
    public static int DefaultGridWidth = 4;

    /// <summary>
    /// The number of starter tiles/numbers a player can get when starting a new game.
    /// </summary>
    public static int DefaultStarterTiles = 2;
#pragma warning restore CA2211
}