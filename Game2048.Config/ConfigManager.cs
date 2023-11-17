    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Game2048.Config;

public static class ConfigManager
{
    public static IEnumerable<(string Name, object? Value, Type Type)> GetConfigItems()
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static) ?? throw new Exception("Config can not be null.");
        var result = new List<(string Name, object? Value, Type Type)>();
        return configItems.Select(configItemAsFieldInfo => 
        (
            configItemAsFieldInfo.Name,
            configItemAsFieldInfo.GetValue(null),
            configItemAsFieldInfo.FieldType
        ));
    }

    static FieldInfo? GetConfigItem(string configItemName)
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static) ?? throw new Exception("Config can not be null.");
        return configItems?.Where(item => item.Name == configItemName).FirstOrDefault();
    }

    public static void SetConfigItem<T>(string configItemName, T newValue)
    {
        var configItemInfo = GetConfigItem(configItemName);
        // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
        configItemInfo?.SetValue(null, newValue);
    }

#pragma warning disable CS8600
    public static T GetConfigItem<T>(string configItemName, T configItemType)
    {
        var configItemInfo = GetConfigItem(configItemName);
        if (configItemInfo?.DeclaringType == configItemType?.GetType())
        {
            // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
            return (T)configItemInfo?.GetValue(null) ?? throw new NullReferenceException("Config item can not be null");
        }
        throw new Exception("Wrong type request");
    }
#pragma warning restore CS8600

    public static void Load()
    {
        string configData = File.ReadAllText(GameData.ConfigFilePath);
        using var jsonDoc = JsonDocument.Parse(configData);
        var jsonRoot = jsonDoc.RootElement;
        try
        {
            IList<int> defaultAcceptedSpawnables = new List<int>();
            var acceptedSpawnablesEnumerable = jsonRoot.GetProperty("defaultAcceptedSpawnables").EnumerateArray();
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
            GameConfiguration.DefaultGoal = jsonRoot.GetProperty("defaultGoal").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.MaxHighscoresListLength = jsonRoot.GetProperty("maxHighscoresListLength").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultMaxLives = jsonRoot.GetProperty("defaultMaxLives").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultMaxUndos = jsonRoot.GetProperty("defaultMaxUndos").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultGridWidth = jsonRoot.GetProperty("defaultGridWidth").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.DefaultGridHeight = jsonRoot.GetProperty("defaultGridHeight").GetInt32();
        }
        catch (KeyNotFoundException)
        { }
        try
        {
            GameConfiguration.GameDataDirectory = jsonRoot.GetProperty("gameDataDirectory").GetString();
        }
        catch (KeyNotFoundException)
        { }
    }

    public static void Save()
    {
        string jsonData = "{";
        jsonData += "\"defaultAcceptedSpawnables\":[" + string.Join(",", GameConfiguration.DefaultAcceptedSpawnables) + "],";
        jsonData += "\"defaultGoal\":" + GameConfiguration.DefaultGoal + ",";
        jsonData += "\"maxHighscoresListLength\":" + GameConfiguration.MaxHighscoresListLength + ",";
        jsonData += "\"defaultMaxLives\":" + GameConfiguration.DefaultMaxLives + ",";
        jsonData += "\"defaultMaxUndos\":" + GameConfiguration.DefaultMaxUndos + ",";
        jsonData += "\"defaultGridWidth\":" + GameConfiguration.DefaultGridWidth + ",";
        jsonData += "\"defaultGridHeight\":" + GameConfiguration.DefaultGridHeight;
        if (GameConfiguration.GameDataDirectory is not null)
        {
            jsonData += ",\"gameDataDirectory\":" + GameConfiguration.GameDataDirectory;
        }
        jsonData += "}";
        File.WriteAllText(jsonData, GameData.ConfigFilePath);
    }
}
