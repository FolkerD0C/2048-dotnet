using System;

namespace Game2048.ConsoleFrontend.Models;

public interface ISameColorDisplayRow : IDisplayRow
{
    string RowText { get; }
    ConsoleColor ForegroundColor { get; }
    ConsoleColor BackgroundColor { get; }
    void SetAll();
}