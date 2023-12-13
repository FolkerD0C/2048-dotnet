using ConsoleClient.Display.Helpers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Display.Models;

/// <summary>
/// A base overlay for the <see cref="DisplayManager"/>.
/// </summary>
internal class BaseOverlay : IOverLay
{
    public IDisplayRow this[int index]
    {
        get { return displayRows[index]; }
        set { displayRows[index] = value; }
    }

    readonly IList<IDisplayRow> displayRows;
    public IList<IDisplayRow> DisplayRows => displayRows;

    public int RowCount => displayRows.Count;

    /// <summary>
    /// Creates a new instance of the <see cref="BaseOverlay"/> class.
    /// </summary>
    public BaseOverlay()
    {
        displayRows = new List<IDisplayRow>();
        for (int i = 0; i < DisplayManager.Height; i++)
        {
            displayRows.Add(new DisplayRow().PadRight(DisplayManager.Width, DisplayManager.DefaultForegroundColor, DisplayManager.DefaultBackgroundColor, DisplayManager.DefaultDisplayPositionValue, true));
        }
    }

    public void Dispose()
    {
        displayRows.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        return true;
    }

    public void SetPreviousOverlaySuppression(bool previousOverlaySuppression)
    { }

    public void RestoreAsPrevious()
    { }
}