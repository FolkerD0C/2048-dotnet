using Game2048.Config;
using Game2048.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.Logic;

public class ConfigLogic : IConfigLogic
{
    public IEnumerable<ConfigItem> GetConfigItems()
    {
        var items = ConfigManager.GetConfigItems();
        return items.Select(item => new ConfigItem()
        {
            Name = item.Name,
            Value = item.Value,
            Type = item.Type
        });
    }

    public T GetConfigValue<T>(string configItemName)
    {
        return ConfigManager.GetConfigItem<T>(configItemName);
    }

    public void LoadConfig()
    {
        ConfigManager.Load();
    }

    public void SaveConfig()
    {
        ConfigManager.Save();
    }

    public void SetConfigValue<T>(string configItemName, T newValue)
    {
        ConfigManager.SetConfigItem(configItemName, newValue);
    }
}