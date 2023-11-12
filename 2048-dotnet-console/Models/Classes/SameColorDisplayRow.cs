using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.ConsoleFrontend.Models;

public class SameColorDisplayRow : ISameColorDisplayRow
{
    readonly IList<IDisplayPosition> displayPositions;
    public IList<IDisplayPosition> DisplayPositions => displayPositions;

    public IDisplayPosition this[int index]
    {
        get { return displayPositions[index]; }
        set { displayPositions[index] = value; }
    }

    public SameColorDisplayRow()
    {
        displayPositions = new List<IDisplayPosition>();
    }

    public string RowText => new(displayPositions.Select(dp => dp.Value).ToArray());

    readonly ConsoleColor foregroundColor;
    public ConsoleColor ForegroundColor => foregroundColor;

    readonly ConsoleColor backgroundColor;
    public ConsoleColor BackgroundColor => backgroundColor;

    public int ColumnCount => displayPositions.Count;

    public bool IsSet => displayPositions.Any(dp => dp.IsSet);
    
    public void SetAll()
    {
        foreach (var dp in displayPositions)
        {
            dp.IsSet = true;
        }
    }
}