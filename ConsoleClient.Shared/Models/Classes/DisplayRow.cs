using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.Shared.Models;

public class DisplayRow : IDisplayRow
{
    readonly IList<DisplayPosition> displayPositions;
    public IList<DisplayPosition> DisplayPositions => displayPositions;

    public DisplayRow()
    {
        displayPositions = new List<DisplayPosition>();
    }

    public DisplayRow(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor, int HorizontalOffset, ConsoleColor defaultForegroundColor, ConsoleColor defaultBackgroundColor) : this()
    {
        for (int i = 0; i < HorizontalOffset; i++)
        {
            displayPositions.Add(new DisplayPosition()
            {
                BackgroundColor = defaultBackgroundColor,
                ForegroundColor = defaultForegroundColor,
                Value = ' ',
                IsSet = false
            });
        }
        foreach (char letter in text)
        {
            displayPositions.Add(new DisplayPosition()
            {
                BackgroundColor = backgroundColor,
                ForegroundColor = foregroundColor,
                Value = letter,
                IsSet = true
            });
        }
    }

    public DisplayRow(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) : this(text, foregroundColor, backgroundColor, 0, default, default)
    { }

    public bool IsSet => AreAnySet();

    public int ColumnCount => displayPositions.Count;

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