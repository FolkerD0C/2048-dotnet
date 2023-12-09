using System.Collections.Generic;

namespace Game2048.Config;

public class ConfigSaveHelper
{
    /// <summary>
    /// The maximum length of the high score list.
    /// </summary>
    public int? MaxHighscoresListLength { get; set; }

    /// <summary>
    /// The path of the directory that contains <see cref="GameData.HighscoresFilePath"/>
    /// and <see cref="GameData.SaveGameDirectoryPath"/>.
    /// </summary>
    public string? GameDataDirectory { get; set; }

    /// <summary>
    /// The list that contains the accepted spawnable tiles/numbers.
    /// </summary>
    public List<int>? DefaultAcceptedSpawnables { get; set; }

    /// <summary>
    /// The goal of the game. If a player reaches it, they win.
    /// </summary>
    public int? DefaultGoal { get; set; }

    /// <summary>
    /// Maximum lives a player can get when starting a new game.
    /// </summary>
    public int? DefaultMaxLives { get; set; }

    /// <summary>
    /// Maximum length of the undo chain a player can reach in a play.
    /// </summary>
    public int? DefaultMaxUndos { get; set; }

    /// <summary>
    /// Width of the playing grid that sets a new games playing grid width.
    /// </summary>
    public int? DefaultGridHeight { get; set; }

    /// <summary>
    /// Height of the playing grid that sets a new games playing grid height.
    /// </summary>
    public int? DefaultGridWidth { get; set; }

    /// <summary>
    /// The number of starter tiles/numbers a player can get when starting a new game.
    /// </summary>
    public int? DefaultStarterTiles { get; set; }

    public ConfigSaveHelper()
    { }

    public void GetConfig()
    {
        MaxHighscoresListLength = GameConfiguration.MaxHighscoresListLength;
        GameDataDirectory = GameConfiguration.GameDataDirectory;
        DefaultAcceptedSpawnables = GameConfiguration.DefaultAcceptedSpawnables;
        DefaultGoal = GameConfiguration.DefaultGoal;
        DefaultMaxLives = GameConfiguration.DefaultMaxLives;
        DefaultMaxUndos = GameConfiguration.DefaultMaxUndos;
        DefaultGridHeight = GameConfiguration.DefaultGridHeight;
        DefaultGridWidth = GameConfiguration.DefaultGridWidth;
        DefaultStarterTiles = GameConfiguration.DefaultStarterTiles;
    }

    public void SetConfig()
    {
        if (MaxHighscoresListLength is not null)
        {
            GameConfiguration.MaxHighscoresListLength = (int)MaxHighscoresListLength;
        }
        if (GameDataDirectory is not null)
        {
            GameConfiguration.GameDataDirectory = GameDataDirectory;
        }
        if (DefaultAcceptedSpawnables is not null)
        {
            GameConfiguration.DefaultAcceptedSpawnables = DefaultAcceptedSpawnables;
        }
        if (DefaultGoal is not null)
        {
            GameConfiguration.DefaultGoal = (int)DefaultGoal;
        }
        if (DefaultMaxLives is not null)
        {
            GameConfiguration.DefaultMaxLives = (int)DefaultMaxLives;
        }
        if (DefaultMaxUndos is not null)
        {
            GameConfiguration.DefaultMaxUndos = (int)DefaultMaxUndos;
        }
        if (DefaultGridHeight is not null)
        {
            GameConfiguration.DefaultGridHeight = (int)DefaultGridHeight;
        }
        if (DefaultGridWidth is not null)
        {
            GameConfiguration.DefaultGridWidth = (int)DefaultGridWidth;
        }
        if (DefaultStarterTiles is not null)
        {
            GameConfiguration.DefaultStarterTiles = (int)DefaultStarterTiles;
        }
    }
}
