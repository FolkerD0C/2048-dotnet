using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.Display.Models;

/// <summary>
/// A class that represents a row of display positions for the <see cref="IOverLay"/> interface.
/// </summary>
public class DisplayRow : IDisplayRow
{
    readonly IList<DisplayPosition> displayPositions;
    public IList<DisplayPosition> DisplayPositions => displayPositions;

    /// <summary>
    /// Creates a new instance of the  <see cref="DisplayRow"/> class.
    /// </summary>
    public DisplayRow()
    {
        displayPositions = new List<DisplayPosition>();
    }

    public bool IsSet => AreAnySet();

    public int ColumnCount => displayPositions.Count;

    /// <summary>
    /// Returns true if any of the display positions are set.
    /// </summary>
    /// <returns>True if any of the display positions are set.</returns>
    bool AreAnySet()
    {
        return displayPositions.Any(dp => dp.IsSet);
    }

    public IDisplayRow PadRight(int width, ConsoleColor foregroundColor, ConsoleColor backgroundColor, char value, bool isSet = false)
    {
        while (ColumnCount < width)
        {
            displayPositions.Add(new DisplayPosition()
            {
                BackgroundColor = backgroundColor,
                ForegroundColor = foregroundColor,
                Value = value,
                IsSet = isSet
            });
        }
        return this;
    }

    public DisplayPosition this[int index]
    {
        get
        {
            while (displayPositions.Count <= index)
            {
                displayPositions.Add(new DisplayPosition()
                {
                    IsSet = false
                });
            }
            return displayPositions[index];
        }
        set
        {
            while (displayPositions.Count <= index)
            {
                displayPositions.Add(new DisplayPosition()
                {
                    IsSet = false
                });
            }
            displayPositions[index] = value;
        }
    }
}