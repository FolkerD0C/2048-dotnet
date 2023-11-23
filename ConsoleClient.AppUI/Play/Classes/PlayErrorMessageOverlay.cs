using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.AppUI.Play;

public class PlayErrorMessageOverlay : IOverLay
{
    const ConsoleColor defaultForegroundColor = ConsoleColor.White;
    const ConsoleColor defaltBackgroundColor = ConsoleColor.Red;

    public IDisplayRow this[int index]
    {
        get
        {
            while (displayRows.Count <= index)
            {
                displayRows.Add(new DisplayRow());
            }
            return displayRows[index];
        }
        set { displayRows[index] = value; }
    }

    readonly IList<string> errorMessage;
    int verticalOffset;
    int horizontalOffset;
    int rowLength;

    readonly IList<IDisplayRow> displayRows;
    public IList<IDisplayRow> DisplayRows => displayRows;

    public int RowCount => displayRows.Count;

    public PlayErrorMessageOverlay(string error)
    {
        displayRows = new List<IDisplayRow>();
        errorMessage = SplitErrorMessage(error);
    }

    // TODO Split on words
    IList<string> SplitErrorMessage(string error)
    {
        rowLength = DisplayManager.Width / 2;
        horizontalOffset = (DisplayManager.Width - rowLength) / 2;
        IList<string> result = new List<string>();
        while (error.Length >= rowLength)
        {
            result.Add(error[..rowLength]);
            error = error[rowLength..];
        }
        result.Add(error);
        verticalOffset = (DisplayManager.Height - result.Count) / 2;
        return result;
    }

    public void Dispose()
    {
        displayRows.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        return displayRows.Count >= relativeVerticalPosition
            && displayRows[relativeVerticalPosition].ColumnCount >= relativeHorizontalPosition
            && displayRows[relativeVerticalPosition][relativeHorizontalPosition].IsSet;
    }

    public void PrintErrorMessage()
    {
        DisplayManager.NewOverlay(this);
        for (int i = 0; i < errorMessage.Count; i++)
        {
            DisplayManager.PrintText(
                errorMessage[i].PadRight(rowLength),
                verticalOffset + i,
                horizontalOffset,
                defaultForegroundColor,
                defaltBackgroundColor
            );
        }
        ProvideCorrectInput();
        DisplayManager.RollBackOverLay();
    }

    void ProvideCorrectInput()
    {
        do
        {
            DisplayManager.PrintText(
                "Press space to continue...",
                verticalOffset + errorMessage.Count + 1,
                horizontalOffset,
                defaultForegroundColor,
                defaltBackgroundColor
            );
        }
        while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);
    }
}
