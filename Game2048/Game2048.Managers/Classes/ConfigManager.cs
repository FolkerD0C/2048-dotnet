using _2048ish.Base.Enums;
using _2048ish.Base.Models;
using Game2048.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Game2048.Managers;

/// <summary>
/// A class that represents a high level manager for handling the game configuration.
/// </summary>
public class ConfigManager : IConfigManager
{
    public IEnumerable<ConfigItem<object>> GetConfigItems()
    {

        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static);
        return configItems.Select(configItemAsFieldInfo => new ConfigItem<object>()
        {
            Name = configItemAsFieldInfo.Name,
            Value = configItemAsFieldInfo.GetValue(null),
            Status = ConfigItemStatus.Found
        });
    }

    public ConfigItem<T> GetConfigValue<T>(string configItemName)
    {
        var configItemInfo = GetConfigItemInfo(configItemName);
        var failedResult = new ConfigItem<T>()
        {
            Name = configItemName,
            Value = default,
            Status = ConfigItemStatus.NotFound
        };
        if (configItemInfo is null)
        {
            return failedResult;
        }
        // https://stackoverflow.com/questions/2330026/is-it-possible-to-set-this-static-private-member-of-a-static-class-with-reflecti
        if (configItemInfo.GetValue(null) is T configItemValue)
        {
            return new ConfigItem<T>()
            {
                Name = configItemName,
                Value = configItemValue,
                Status = ConfigItemStatus.Found
            };
        }
        return failedResult;
    }

    public ConfigItem<T> SetConfigValue<T>(ConfigItem<T> configItem)
    {
        var configItemInfo = GetConfigItemInfo(configItem.Name);
        if (configItemInfo is null)
        {
            return new ConfigItem<T>()
            {
                Name = configItem.Name,
                Value = default,
                Status = ConfigItemStatus.NotFound
            };
        }
        try
        {
            configItemInfo.SetValue(null, configItem.Value);
        }
        catch (ArgumentException)
        {
            return new ConfigItem<T>()
            {
                Name = configItem.Name,
                Value = default,
                Status = ConfigItemStatus.SettingFailed
            };
        }
        return new ConfigItem<T>()
        {
            Name = configItem.Name,
            Value = configItem.Value,
            Status = ConfigItemStatus.SuccessfullySet
        };
    }

    public void LoadConfig()
    {
        if (!File.Exists(GameData.ConfigFilePath))
        {
            return;
        }
        string configData = File.ReadAllText(GameData.ConfigFilePath);
        JsonSerializerOptions serializerOptions = new()
        {
            WriteIndented = true
        };
        var saveHelper = JsonSerializer.Deserialize<ConfigSaveHelper>(configData, serializerOptions);
        saveHelper?.SetConfig();
    }

    public void SaveConfig()
    {
        ConfigSaveHelper saveHelper = new();
        saveHelper.GetConfig();
        JsonSerializerOptions serializerOptions = new()
        {
            WriteIndented = true
        };
        var configData = JsonSerializer.Serialize(saveHelper, serializerOptions);
        File.WriteAllText(GameData.ConfigFilePath, configData);
    }



    /// <summary>
    /// Gets the field from <see cref="GameConfiguration"/> that matches <paramref name="configItemName"/>.
    /// </summary>
    /// <param name="configItemName">The name of the desired configuration field.</param>
    /// <returns>A <see cref="FieldInfo"/> object that contains the desired configuration field.</returns>
    /// <exception cref="ArgumentException"></exception>
    static FieldInfo? GetConfigItemInfo(string configItemName)
    {
        var configItems = typeof(GameConfiguration).GetFields(BindingFlags.Static);
        var matchingConfigItems = configItems.Where(item => item.Name == configItemName);
        if (!matchingConfigItems.Any())
        {
            return null;
        }
        return matchingConfigItems.First();
    }
}