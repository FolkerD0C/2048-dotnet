namespace Game2048.Config;

public static class GameData
{
    public static readonly string GameDescription = "";

    public static readonly string GameHelp = "";

    public static readonly string SaveGameDirectoryPath = (GameConfiguration.GameDataDirectory ?? "") + "saves";

    public static readonly string HighscoresFilePath = (GameConfiguration.GameDataDirectory ?? "") + "highscores.json";

    public static readonly string ConfigFilePath = "gameconfig.json";
}