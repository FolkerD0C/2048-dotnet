using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Game2048.Config;

public static class ConfigManager
{
    public static IEnumerable<(string Name, object? Value)> GetConfigItems()
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static | BindingFlags.NonPublic) ?? throw new NullReferenceException("Config can not be null.");
        var result = new List<(string Name, object? Value, Type Type)>();
        return configItems.Select(configItemAsFieldInfo =>
        (
            configItemAsFieldInfo.Name,
            configItemAsFieldInfo.GetValue(null)
        ));
    }

    static FieldInfo GetConfigItem(string configItemName)
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static | BindingFlags.NonPublic) ?? throw new ArgumentException("Config can not be null.");
        var matchingConfigItems = configItems.Where(item => item.Name == configItemName);
        if (!matchingConfigItems.Any())
        {
            throw new ArgumentException($"Config item '{configItemName}' not found");
        }
        return matchingConfigItems.First();
    }

    public static void SetConfigItem<T>(string configItemName, T newValue)
    {
        var configItemInfo = GetConfigItem(configItemName);
        // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
        configItemInfo.SetValue(null, newValue);
    }

    public static T GetConfigItem<T>(string configItemName)
    {
        var configItemInfo = GetConfigItem(configItemName);
        // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
        if (configItemInfo.GetValue(null) is T configItemValue)
        {
            return configItemValue;
        }
        throw new ArgumentException("Wrong config item type request");
    }

    public static void Load()
    {
        string configData = File.ReadAllText(GameData.ConfigFilePath);
        using var jsonDoc = JsonDocument.Parse(configData);
        var jsonRoot = jsonDoc.RootElement;
        try
        {
            IList<int> defaultAcceptedSpawnables = new List<int>();
            var acceptedSpawnablesEnumerable = jsonRoot.GetProperty("DefaultAcceptedSpawnables").EnumerateArray();
            foreach (var accepted in acceptedSpawnablesEnumerable)
            {
                defaultAcceptedSpawnables.Add(accepted.GetInt32());
            }
            GameConfiguration.DefaultAcceptedSpawnables = defaultAcceptedSpawnables;
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultGoal = jsonRoot.GetProperty("DefaultGoal").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.MaxHighscoresListLength = jsonRoot.GetProperty("MaxHighscoresListLength").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultMaxLives = jsonRoot.GetProperty("DefaultMaxLives").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultMaxUndos = jsonRoot.GetProperty("DefaultMaxUndos").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultGridWidth = jsonRoot.GetProperty("DefaultGridWidth").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultGridHeight = jsonRoot.GetProperty("DefaultGridHeight").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.GameDataDirectory = jsonRoot.GetProperty("GameDataDirectory").GetString() ?? AppDomain.CurrentDomain.BaseDirectory;
        }
        catch (KeyNotFoundException)
        { }
    }

    public static void Save()
    {
        string jsonData = "{";
        jsonData += "\"DefaultAcceptedSpawnables\":[" + string.Join(",", GameConfiguration.DefaultAcceptedSpawnables) + "],";
        jsonData += "\"DefaultGoal\":" + GameConfiguration.DefaultGoal + ",";
        jsonData += "\"MaxHighscoresListLength\":" + GameConfiguration.MaxHighscoresListLength + ",";
        jsonData += "\"DefaultMaxLives\":" + GameConfiguration.DefaultMaxLives + ",";
        jsonData += "\"DefaultMaxUndos\":" + GameConfiguration.DefaultMaxUndos + ",";
        jsonData += "\"DefaultGridWidth\":" + GameConfiguration.DefaultGridWidth + ",";
        jsonData += "\"DefaultGridHeight\":" + GameConfiguration.DefaultGridHeight + ",";
        jsonData += "\"DefaultStarterTiles\":" + GameConfiguration.DefaultStarterTiles + ",";
        jsonData += "\"GameDataDirectory\":\"" + GameConfiguration.GameDataDirectory.Replace(@"\", @"\\") + "\"";
        jsonData += "}";
        File.WriteAllText(GameData.ConfigFilePath, jsonData);
    }
}
