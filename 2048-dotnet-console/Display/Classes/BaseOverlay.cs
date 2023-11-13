using Game2048.ConsoleFrontend.Models;
using System;
using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Display;

public class BaseOverlay : IOverLay
{
    public IDisplayRow this[int index]
    {
        get { return drawPositions[index]; }
        set { drawPositions[index] = value; }
    }

    readonly IList<IDisplayRow> drawPositions;
    public IList<IDisplayRow> DrawPositions => drawPositions;

    public int RowCount => drawPositions.Count;

    public BaseOverlay()
    {
        drawPositions = new List<IDisplayRow>();
        for (int i = 0; i < DisplayManager.Height; i++)
        {
            drawPositions.Add(new SameColorDisplayRow());
            for (int j = 0; j < DisplayManager.Width; j++)
            {
                drawPositions[i].DisplayPositions.Add(new DisplayPosition()
                {
                    BackgroundColor = DisplayManager.DefaultBackgroundColor,
                    ForegroundColor = DisplayManager.DefaultForegroundColor,
                    Value = ' ',
                    IsSet = false
                });
            }
        }
    }

#pragma warning disable CA1816
    public void Dispose()
    {
        throw new InvalidOperationException("Base overlay can not be disposed.");
    }
#pragma warning restore CA1816
}