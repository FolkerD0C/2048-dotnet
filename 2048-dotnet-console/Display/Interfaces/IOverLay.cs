using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Display;

public interface IOverLay
{
    public IList<IDisplayRow> DrawPositions { get; }
    public int RowCount { get; }
    public void PrintOverLay();
    public void DeleteOverlay();
    public IDisplayRow this[int index] { get; set; }
}