using Game2048.Shared.Models;
using System.Collections.Generic;

namespace Game2048.Logic;

public interface IConfigLogic
{
    void LoadConfig();
    void SaveConfig();
    ConfigItem<T> GetConfigValue<T>(string configItemName);
    ConfigItem<T> SetConfigValue<T>(ConfigItem<T> newValue);
    IEnumerable<ConfigItem<object>> GetConfigItems();
}
