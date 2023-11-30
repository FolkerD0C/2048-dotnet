using Game2048.Config;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Logic;

/// <summary>
/// A class that represents a high level manager for handling the game configuration.
/// </summary>
public class ConfigLogic : IConfigLogic
{
    public IEnumerable<ConfigItem<object>> GetConfigItems()
    {
        var items = ConfigManager.GetConfigItems();
        return items.Select
        (item =>
            item.Value is null ?
                new ConfigItem<object>()
                {
                    Name = item.Name,
                    Value = null,
                    Status = ConfigItemStatus.NotFound
                } :
                new ConfigItem<object>()
                {
                    Name = item.Name,
                    Value = item.Value,
                    Status = ConfigItemStatus.Found
                }
        );
    }

    public ConfigItem<T> GetConfigValue<T>(string configItemName)
    {
        T configItemValue = ConfigManager.GetConfigItemValue<T>(configItemName);
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
        ConfigManager.Load();
    }

    public void SaveConfig()
    {
        ConfigManager.Save();
    }

    public ConfigItem<T> SetConfigValue<T>(ConfigItem<T> configItem)
    {
        try
        {
            ConfigManager.SetConfigItemValue(configItem.Name ?? "", configItem.Value);
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