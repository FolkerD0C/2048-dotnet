using System;

namespace Game2048.ConsoleFrontend.Models;

public interface IDisplayPosition
{
    public ConsoleColor ForegroundColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }
    public char Value { get; set; }
    public bool IsSet { get; set; }
    public IDisplayPosition? UnderLay { get; set; }
}