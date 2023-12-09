using Game2048.Base.Models;
using System.Collections.Generic;

namespace Game2048.Managers;

/// <summary>
/// Represents a high level manager for handling the game configuration.
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// Loads the game configuration.
    /// </summary>
    void LoadConfig();

    /// <summary>
    /// Saves the game configuration.
    /// </summary>
    void SaveConfig();

    /// <summary>
    /// Gets the value of the configuration item named <paramref name="configItemName"/>.
    /// </summary>
    /// <typeparam name="T">The type of the desired configuration item.</typeparam>
    /// <param name="configItemName">The name of the desired configuration item.</param>
    /// <returns>The value of the configuration item named <paramref name="configItemName"/> as a <see cref="ConfigItem{T}"/> object.</returns>
    ConfigItem<T> GetConfigValue<T>(string configItemName);

    /// <summary>
    /// Sets the value of the configuration item specified in <paramref name="newValue"/>.
    /// </summary>
    /// <typeparam name="T">The type of the configuration item.</typeparam>
    /// <param name="newValue">The new value of the configuration item as a <see cref="ConfigItem{T}"/> object.</param>
    ConfigItem<T> SetConfigValue<T>(ConfigItem<T> newValue);

    /// <summary>
    /// Gets all configurable items.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{ConfigItem{object}"/> that contains the name and value of all configurable items.</returns>
    IEnumerable<ConfigItem<object>> GetConfigItems();
}
