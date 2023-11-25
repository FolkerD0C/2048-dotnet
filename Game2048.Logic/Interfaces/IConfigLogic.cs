using Game2048.Shared.Models;
using System.Collections.Generic;

namespace Game2048.Logic;

public interface IConfigLogic
{
    void LoadConfig();
    void SaveConfig();
    T GetConfigValue<T>(string configItemName);
    void SetConfigValue<T>(string configItemName, T newValue);
    IEnumerable<ConfigItem> GetConfigItems();
}
