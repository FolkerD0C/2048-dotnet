using System;

namespace Game2048.Shared.Models;
public struct ConfigItem
{
    public string Name { get; set; }
    public object? Value { get; set; }
    public Type Type { get; set; }
}