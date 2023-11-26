using Game2048.Shared.Enums;

namespace Game2048.Shared.Models;

public record ConfigItem<T>
{
    public string? Name { get; init; }
    public T? Value { get; set; }
    public ConfigItemStatus Status { get; set; }
}