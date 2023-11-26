using System;
using System.IO;

namespace Game2048.Config;

public static class GameData
{
    public static readonly string GameDescription =
        "2048 is a single-player sliding block puzzle game designed by Italian web developer Gabriele Cirulli. " +
        "The game's objective is to slide numbered tiles on a grid to combine them to create a tile with the number 2048. " +
        "However, one can continue to play the game after reaching the goal, creating tiles with larger numbers. " +
        "In this version of the game you can undo, have remaining lives and you can customize all sorts of things: " +
        "grid width, grid height, max undos, max lives even the tiles to spawn and the number of tiles spawning in a new game." +
        "You can also save and load games and view high scores (Note: high score will only be added upon Game Over).";

    public static readonly string SaveGameDirectoryPath = GameConfiguration.GameDataDirectory + "saves";

    public static readonly string HighscoresFilePath = GameConfiguration.GameDataDirectory + "highscores.json";

    public static readonly string ConfigFilePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "gameconfig.json");
}