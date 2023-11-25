using System;

namespace Game2048.Shared.Models;

// TODO make separate interface and class from this
public struct ConfigItem
{
    public string Name { get; set; }
    public object? Value { get; set; }
    public Type Type { get; set; }
}