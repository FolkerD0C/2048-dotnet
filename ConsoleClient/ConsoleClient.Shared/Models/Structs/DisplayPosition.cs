using System;

namespace ConsoleClient.Shared.Models;

public struct DisplayPosition
{
    public ConsoleColor ForegroundColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }
    public char Value { get; set; }
    public bool IsSet { get; set; }
}