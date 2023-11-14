using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Display;

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

    public BaseOverlay()
    {
        displayRows = new List<IDisplayRow>();
        for (int i = 0; i < DisplayManager.Height; i++)
        {
            displayRows.Add(new DisplayRow().PadRight(DisplayManager.Width, DisplayManager.DefaultForegroundColor, DisplayManager.DefaultBackgroundColor));
        }
    }

#pragma warning disable CA1816
    public void Dispose()
    {
        throw new InvalidOperationException("Base overlay can not be disposed.");
    }
#pragma warning restore CA1816
}