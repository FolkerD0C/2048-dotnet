using System;
using System.Collections.Generic;

namespace ConsoleClient.Display.Models;

/// <summary>
/// A default overlay that is needed for the DisplayManager before initialization.
/// </summary>
internal class NullOverlay : IOverLay
{
    public IDisplayRow this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IList<IDisplayRow> DisplayRows => throw new NotImplementedException();

    public int RowCount => throw new NotImplementedException();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        throw new NotImplementedException();
    }

    public void RestoreAsPrevious()
    {
        throw new NotImplementedException();
    }

    public void SetPreviousOverlaySuppression(bool previousOverlaySuppression)
    {
        throw new NotImplementedException();
    }
}
