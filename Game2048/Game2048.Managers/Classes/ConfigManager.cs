using Game2048.Base.Enums;
using Game2048.Base.Models;
using Game2048.Config;
using System;
using System.Collections.Generic;

namespace Game2048.Managers;

/// <summary>
/// A class that represents a high level manager for handling the game configuration.
/// </summary>
public class ConfigManager : IConfigManager
{
    public IEnumerable<ConfigItem<object>> GetConfigItems()
    {
        return ConfigRepository.GetConfigItems();
    }

    public ConfigItem<T> GetConfigValue<T>(string configItemName)
    {
        T configItemValue = ConfigRepository.GetConfigItemValue<T>(configItemName);
        if (configItemValue is null)
        {
            return new ConfigItem<T>()
            {
                Name = configItemName,
                Value = default,
                Status = ConfigItemStatus.NotFound
            };
        }
        return new ConfigItem<T>()
        {
            Name = configItemName,
            Value = configItemValue,
            Status = ConfigItemStatus.Found
        };
    }

    public void LoadConfig()
    {
        ConfigRepository.Load();
    }

    public void SaveConfig()
    {
        ConfigRepository.Save();
    }

    public ConfigItem<T> SetConfigValue<T>(ConfigItem<T> configItem)
    {
        try
        {
            ConfigRepository.SetConfigItemValue(configItem.Name ?? "", configItem.Value);
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
}