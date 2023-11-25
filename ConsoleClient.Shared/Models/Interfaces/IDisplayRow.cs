using System;
using System.Collections.Generic;

namespace ConsoleClient.Shared.Models;

public interface IDisplayRow
{
    IList<DisplayPosition> DisplayPositions { get; }
    int ColumnCount { get; }
    bool IsSet { get; }
    DisplayPosition this[int index] { get; set; }
    IDisplayRow PadRight(int width, ConsoleColor foregroundColor, ConsoleColor backgroundColor, char value, bool isSet);
}