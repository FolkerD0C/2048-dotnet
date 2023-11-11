using Game2048.Backend.Helpers.Config;

namespace Game2048.Backend.Helpers.Static;

public static class GameData
{
    public static readonly string GameDescription = "";

    public static readonly string GameHelp = "";

    public static readonly string SaveGameDirectoryPath = (GameConfiguration.GameDataDirectory ?? "") + "saves";

    public static readonly string HighscoresFilePath = (GameConfiguration.GameDataDirectory ?? "") + "highscores.json";

    public static readonly string ConfigFilePath = (GameConfiguration.GameDataDirectory ?? "") + "config.json";
}