using System;
using System.IO;

namespace Game2048.Config;

/// <summary>
/// A static class that contains data mostly for the manager layer.
/// </summary>
public static class GameData
{
    /// <summary>
    /// Description of the game.
    /// </summary>
    public static readonly string GameDescription =
        "2048 is a single-player sliding block puzzle game designed by Italian web developer Gabriele Cirulli. " +
        "The game's objective is to slide numbered tiles on a grid to combine them to create a tile with the number 2048. " +
        "However, one can continue to play the game after reaching the goal, creating tiles with larger numbers. " +
        "In this version of the game you can undo, have remaining lives and you can customize all sorts of things: " +
        "grid width, grid height, max undos, max lives even the tiles to spawn and the number of tiles spawning in a new game." +
        "You can also save and load games and view high scores (Note: high score will only be added upon Game Over).";

    /// <summary>
    /// The path for the directory that contains game save files.
    /// </summary>
    public static string SaveGameDirectoryPath => GameConfiguration.GameDataDirectory + "saves";

    /// <summary>
    /// The path of the file that contains high scores.
    /// </summary>
    public static string HighscoresFilePath => GameConfiguration.GameDataDirectory + "highscores.json";

    /// <summary>
    /// The path of the game configuration file. It should be next to the executable.
    /// </summary>
    public static string ConfigFilePath => Path.Join(AppDomain.CurrentDomain.BaseDirectory, "gameconfig.json");
}