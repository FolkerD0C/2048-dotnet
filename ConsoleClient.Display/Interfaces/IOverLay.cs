using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Display;

public interface IOverLay : IDisposable
{
    IList<IDisplayRow> DisplayRows { get; }
    int RowCount { get; }
    bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition);
    IDisplayRow this[int index] { get; set; }
}