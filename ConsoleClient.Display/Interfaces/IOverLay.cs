using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Display;

public interface IOverLay : IDisposable
{
    public IList<IDisplayRow> DisplayRows { get; }
    public int RowCount { get; }
    public IDisplayRow this[int index] { get; set; }
}