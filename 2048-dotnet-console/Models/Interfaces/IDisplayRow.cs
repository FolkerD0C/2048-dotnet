using System;
using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Models;

public interface IDisplayRow
{
    public IList<IDisplayPosition> DisplayPositions { get; }
    public int ColumnCount { get; }
    public bool IsSet { get; }
    public IDisplayPosition this[int index] { get; set; }
}