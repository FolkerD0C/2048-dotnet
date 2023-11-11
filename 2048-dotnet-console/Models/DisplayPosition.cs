using System;

namespace Game2048.ConsoleFrontend.Models;

public struct DisplayPosition
{
    public ConsoleColor ForeGroundColor;
    public ConsoleColor BackGroundColor;
    public char Value;
    public bool IsSet;
}