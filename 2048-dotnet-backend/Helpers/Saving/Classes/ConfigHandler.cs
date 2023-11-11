using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game2048.Classes;

namespace Game2048.Backend.Helpers.Saving;

public class ConfigHandler : FileHandler
{
    public ConfigHandler() : this(GameData.ConfigFilePath)
    { }

    public ConfigHandler(string saveFilePath) : base(saveFilePath)
    { }

    public void Load()
    {
        string configData = Read();
        using var jsonDoc = JsonDocument.Parse(configData);
        var jsonRoot = jsonDoc.RootElement;
        IList<int> defaultAcceptedSpawnables = new List<int>();
        var acceptedSpawnablesEnumerable = jsonRoot.GetProperty("defaultAcceptedSpawnables").EnumerateArray();
        foreach (var accepted in acceptedSpawnablesEnumerable)
        {
            defaultAcceptedSpawnables.Add(accepted.GetInt32());
        }
        GameConfiguration.DefaultAcceptedSpawnables = defaultAcceptedSpawnables;
        GameConfiguration.DefaultGoal = jsonRoot.GetProperty("defaultGoal").GetInt32();
        GameConfiguration.MaxHighscoresListLength = jsonRoot.GetProperty("maxHighscoresListLength").GetInt32();
        GameConfiguration.DefaultMaxLives = jsonRoot.GetProperty("defaultMaxLives").GetInt32();
        GameConfiguration.DefaultMaxUndos = jsonRoot.GetProperty("defaultMaxUndos").GetInt32();
        GameConfiguration.DefaultGridWidth = jsonRoot.GetProperty("defaultGridWidth").GetInt32();
        GameConfiguration.DefaultGridHeight = jsonRoot.GetProperty("defaultGridHeight").GetInt32();
    }

    protected override string Read()
    {
        return File.ReadAllText(saveFilePath);
    }

    protected override void Write(string fileContent)
    {
        throw new InvalidOperationException("ConfigFile should not be written from inside the game.");
    }
}