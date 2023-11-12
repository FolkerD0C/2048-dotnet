using System;
using System.Collections.Generic;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Display;

public interface IOverLay : IDisposable
{
    public IList<IDisplayRow> DrawPositions { get; }
    public int RowCount { get; }
    public IDisplayRow this[int index] { get; set; }
}