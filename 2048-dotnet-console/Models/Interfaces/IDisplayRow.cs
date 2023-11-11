using System;
using System.Collections.Generic;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Display;

public interface IDisplayRow
{
    public IList<DisplayPosition> DisplayPositions { get; }
    public int ColumnCount { get; }
    public bool IsSameColor { get; }
    public bool IsSet { get; }
    public (ConsoleColor FGColor, ConsoleColor BGColor, string RowValue) GetFullRow();
    public DisplayPosition this[int index] { get; set; }
}