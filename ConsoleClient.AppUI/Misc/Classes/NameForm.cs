using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.AppUI.Misc;

public class NameForm : INameForm
{
    const string PromptLabel1 = "Please enter your name below,";
    const string PromptLabel2 = "you can submit with ENTER and cancel with ESCAPE";
    const int NameFormLength = 32;
    readonly static ConsoleColor FormForegroundColor = ConsoleColor.Black;
    readonly static ConsoleColor FormBackgroundColor = ConsoleColor.Cyan;

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

    readonly IList<IDisplayRow> displayRows;
    public IList<IDisplayRow> DisplayRows => displayRows;

    public int RowCount => displayRows.Count;

    readonly char[] nameFormValue;
    readonly Func<NameFormInput> inputMethod;

    public NameForm(Func<NameFormInput> inputMethod)
    {
        displayRows = new List<IDisplayRow>();
        nameFormValue = new char[NameFormLength];
        this.inputMethod = inputMethod;
    }

    public void Dispose()
    {
        displayRows.Dispose();
        GC.SuppressFinalize(this);
    }

    public NameFormResult PromptPlayerName(string name)
    {
        DisplayManager.NewOverlay(this);
        int middleRow = DisplayManager.Height / 2;
        int formVerticalOFfset = middleRow + 1;
        int formHorizontalOFfset = (DisplayManager.Width - NameFormLength) / 2;
        throw new NotImplementedException();
    }

    void PrintPromptLabel()
    {
        int promptLabel1VerticalOffset = DisplayManager.Height / 2 - 2;
        int promptLabel2VerticalOffset = DisplayManager.Height / 2 - 1;
        int promptLabelHorizontalOffset = (DisplayManager.Width - PromptLabel1.Length) / 2;
        DisplayManager.PrintText(
            PromptLabel1,
            promptLabel1VerticalOffset,
            promptLabelHorizontalOffset,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        DisplayManager.PrintText(
            PromptLabel2,
            promptLabel2VerticalOffset,
            promptLabelHorizontalOffset,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        return displayRows.Count > relativeVerticalPosition
            && displayRows[relativeVerticalPosition].ColumnCount > relativeHorizontalPosition
            && displayRows[relativeVerticalPosition][relativeHorizontalPosition].IsSet;
    }
}
