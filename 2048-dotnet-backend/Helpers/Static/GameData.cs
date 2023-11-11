namespace Game2048.Backend;

internal static class GameData
{
    internal static readonly string GameDescription = "";

    internal static readonly string GameHelp = "";

    internal static readonly string SaveGameDirectoryPath = (GameConfiguration.GameDataDirectory ?? "") + "saves";

    internal static readonly string HighscoresFilePath = (GameConfiguration.GameDataDirectory ?? "") + "highscores.json";

    internal static readonly string ConfigFilePath = (GameConfiguration.GameDataDirectory ?? "") + "config.json";
}