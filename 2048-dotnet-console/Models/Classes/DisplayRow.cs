using System.Collections.Generic;
using System.Linq;

namespace Game2048.ConsoleFrontend.Models;

public class DisplayRow : IDisplayRow
{
    IList<IDisplayPosition> displayPositions;
    public IList<IDisplayPosition> DisplayPositions => displayPositions;

    public DisplayRow(IList<IDisplayPosition> displayPositions)
    {
        this.displayPositions = displayPositions;
    }

    public bool IsSet => AreAnySet();

    public int ColumnCount => displayPositions.Count;

    bool AreAnySet()
    {
        return displayPositions.Any(dp => dp.IsSet);
    }

    public IDisplayPosition this[int index]
    {
        get { return displayPositions[index]; }
        set { displayPositions[index] = value; }
    }
}