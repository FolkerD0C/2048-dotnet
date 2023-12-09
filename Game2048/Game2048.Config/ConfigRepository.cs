using Game2048.Base.Enums;
using Game2048.Base.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Game2048.Config;

/// <summary>
/// A static class that is used for getting and setting configuration values stored in <see cref="GameConfiguration"/>.
/// </summary>
public static class ConfigRepository
{
    /// <summary>
    /// Returns all configurable items.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{(string, object?)}"/> that contains the name and value of all configurable items.</returns>
    public static IEnumerable<ConfigItem<object>> GetConfigItems()
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
        return configItems.Select(configItemAsFieldInfo => new ConfigItem<object>()
        {
            Name = configItemAsFieldInfo.Name,
            Value = configItemAsFieldInfo.GetValue(null),
            Status = ConfigItemStatus.Found
        });
    }

    /// <summary>
    /// Gets the field from <see cref="GameConfiguration"/> that matches <paramref name="configItemName"/>.
    /// </summary>
    /// <param name="configItemName">The name of the desired configuration field.</param>
    /// <returns>A <see cref="FieldInfo"/> object that contains the desired configuration field.</returns>
    /// <exception cref="ArgumentException"></exception>
    static FieldInfo GetConfigItemInfo(string configItemName)
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
        var matchingConfigItems = configItems.Where(item => item.Name == configItemName);
        if (!matchingConfigItems.Any())
        {
            throw new ArgumentException($"Config item '{configItemName}' not found");
        }
        return matchingConfigItems.First();
    }

    /// <summary>
    /// Sets the value of the configuration item named <paramref name="configItemName"/> to <paramref name="newValue"/>.
    /// </summary>
    /// <typeparam name="T">The type of the configuration item that is set by <see cref="SetConfigItemValue{T}(string, T)"/>.</typeparam>
    /// <param name="configItemName">The name of the configuration item that is set by <see cref="SetConfigItemValue{T}(string, T)"/>.</param>
    /// <param name="newValue">The new value of the configuration item named <paramref name="configItemName"/>.</param>
    public static void SetConfigItemValue<T>(string configItemName, T newValue)
    {
        var configItemInfo = GetConfigItemInfo(configItemName);
        // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
        configItemInfo.SetValue(null, newValue);
    }

    /// <summary>
    /// Gets the value of the configuration item named <paramref name="configItemName"/>.
    /// </summary>
    /// <typeparam name="T">The type of the desired configuration item.</typeparam>
    /// <param name="configItemName">The name of the desired configuration item.</param>
    /// <returns>The <typeparamref name="T"/> value of the configuration item named <paramref name="configItemName"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static T GetConfigItemValue<T>(string configItemName)
    {
        var configItemInfo = GetConfigItemInfo(configItemName);
        // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
        if (configItemInfo.GetValue(null) is T configItemValue)
        {
            return configItemValue;
        }
        throw new ArgumentException("Wrong config item type request");
    }

    /// <summary>
    /// Loads the configuration stored in the file at <see cref="GameData.ConfigFilePath"/>. Does nothing if the file is not found.
    /// </summary>
    public static void Load()
    {
        if (!File.Exists(GameData.ConfigFilePath))
        {
            return;
        }
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

    /// <summary>
    /// Saves the configuration to <see cref="GameData.ConfigFilePath"/>.
    /// </summary>
    public static void Save()
    {
        StringBuilder jsonBuilder = new();
        jsonBuilder.Append('{');

        jsonBuilder.Append("\"DefaultAcceptedSpawnables\":[");
        jsonBuilder.AppendJoin(",", GameConfiguration.DefaultAcceptedSpawnables);
        jsonBuilder.Append("],");

        jsonBuilder.Append("\"DefaultGoal\":");
        jsonBuilder.Append(GameConfiguration.DefaultGoal);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"MaxHighscoresListLength\":");
        jsonBuilder.Append(GameConfiguration.MaxHighscoresListLength);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"DefaultMaxLives\":");
        jsonBuilder.Append(GameConfiguration.DefaultMaxLives);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"DefaultMaxUndos\":");
        jsonBuilder.Append(GameConfiguration.DefaultMaxUndos);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"DefaultGridWidth\":");
        jsonBuilder.Append(GameConfiguration.DefaultGridWidth);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"DefaultGridHeight\":");
        jsonBuilder.Append(GameConfiguration.DefaultGridHeight);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"DefaultStarterTiles\":");
        jsonBuilder.Append(GameConfiguration.DefaultStarterTiles);
        jsonBuilder.Append(',');

        jsonBuilder.Append("\"GameDataDirectory\":\"");
        jsonBuilder.Append(GameConfiguration.GameDataDirectory.Replace(@"\", @"\\"));
        jsonBuilder.Append('"');

        jsonBuilder.Append('}');

        File.WriteAllText(GameData.ConfigFilePath, jsonBuilder.ToString());
    }
}
