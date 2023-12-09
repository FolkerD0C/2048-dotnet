using Game2048.Base.Enums;

namespace Game2048.Base.Models;

/// <summary>
/// A dataclass that stores information about a configuration item.
/// </summary>
/// <typeparam name="T">The type of the configuration item.</typeparam>
public record ConfigItem<T>
{
    /// <summary>
    /// The name of the configuration item.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The value of the configuration item.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// The status of the configuration item.
    /// </summary>
    public ConfigItemStatus Status { get; set; }
}