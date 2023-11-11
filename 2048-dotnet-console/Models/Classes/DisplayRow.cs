using System;
using System.Collections.Generic;
using System.Linq;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Display;

public class DisplayRow : IDisplayRow
{
    IList<DisplayPosition> displayPositions;
    public IList<DisplayPosition> DisplayPositions => displayPositions;

    public DisplayRow(IList<DisplayPosition> displayPositions)
    {
        this.displayPositions = displayPositions;
    }

    public bool IsSameColor => AllAreTheSameColor();

    public bool IsSet => AreAnySet();

    public int ColumnCount => displayPositions.Count;

    bool AllAreTheSameColor()
    {
        ConsoleColor fgColor = displayPositions[0].ForeGroundColor;
        ConsoleColor bgColor = displayPositions[0].BackGroundColor;
        for (int i = 1; i < displayPositions.Count; i++)
        {
            if (displayPositions[i].ForeGroundColor != fgColor || displayPositions[i].BackGroundColor != bgColor)
            {
                return false;
            }
        }
        return true;
    }

    bool AreAnySet()
    {
        return displayPositions.Any(dp => dp.IsSet);
    }

    public (ConsoleColor FGColor, ConsoleColor BGColor, string RowValue) GetFullRow()
    {
        ConsoleColor FGColor = displayPositions[0].ForeGroundColor;
        ConsoleColor BGColor = displayPositions[0].BackGroundColor;
        string RowValue = new(displayPositions.Select(dp => dp.Value).ToArray());
        return (FGColor, BGColor, RowValue);
    }

    public DisplayPosition this[int index]
    {
        get { return displayPositions[index]; }
        set { displayPositions[index] = value; }
    }
}