using ConsoleClient.AppUI.Enums;
using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Display.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.AppUI.Misc;

/// <summary>
/// A class that defines and handles displaying a method for prompting the player for new a name.
/// </summary>
public class NameForm : INameForm
{
    /// <summary>
    /// A list of characters which can not be stored in player names.
    /// </summary>
    const string BannedCharacters = "<>:\"/\\|?*";
    /// <summary>
    /// The first row of the prompt label.
    /// </summary>
    const string PromptLabel1 = "Please enter your name below,";
    /// <summary>
    /// The second row of the prompt label.
    /// </summary>
    const string PromptLabel2 = "you can submit with ENTER and cancel with ESCAPE";
    /// <summary>
    /// The maximum length for a name form.
    /// </summary>
    const int NameFormLength = 32;
    /// <summary>
    /// The foreground color of a name form.
    /// </summary>
    readonly static ConsoleColor FormForegroundColor = ConsoleColor.Black;
    /// <summary>
    /// The background color of a name form.
    /// </summary>
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

    /// <summary>
    /// The vertical offset of the frame.
    /// </summary>
    int formVerticalOffset;
    /// <summary>
    /// The horizontal offset of the frame.
    /// </summary>
    int formHorizontalOffset;

    /// <summary>
    /// Stores the current value of the player's name.
    /// </summary>
    readonly char[] nameFormValue;
    /// <summary>
    /// The method that supplies the form action with an input.
    /// </summary>
    readonly Func<NameFormInput> inputMethod;

    /// <summary>
    /// If true then the printing of the overlay under this is suppressed.
    /// </summary>
    bool suppressPrintingPreviosOverlay;

    /// <summary>
    /// Creates a new instance of the <see cref="NameForm"/> class.
    /// </summary>
    /// <param name="inputMethod">The method that supplies the form action with an input.</param>
    public NameForm(Func<NameFormInput> inputMethod)
    {
        displayRows = new List<IDisplayRow>();
        nameFormValue = new char[NameFormLength];
        this.inputMethod = inputMethod;
        suppressPrintingPreviosOverlay = false;
    }

    public void Dispose()
    {
        displayRows.Dispose();
        GC.SuppressFinalize(this);
    }

    public NameFormResult PromptPlayerName(string name)
    {
        var formResult = new NameFormResult()
        {
            ResultType = NameFormResultType.Unknown,
            Name = name
        };
        int cursorPos = 0;

        static int moveCursorLeft(int cp) => Math.Max(cp - 1, 0);
        static int moveCursorRight(int cp) => Math.Min(cp + 1, NameFormLength - 1);

        DisplayManager.NewOverlay(this);
        PrintPromptLabel();
        InitializeFormValue(name);
        UpdateCursor(cursorPos);
        DisplayManager.SetCursorVisibility(true);
        bool inPrompt = true;
        while (inPrompt)
        {
            var input = inputMethod();
            switch (input.InputType)
            {
                case NameFormInputType.Character:
                    {
                        if (InsertAt(input.InputValue, cursorPos))
                        {
                            PrintFormValue(cursorPos);
                            cursorPos = moveCursorRight(cursorPos);
                            UpdateCursor(cursorPos);
                        }
                        break;
                    }
                case NameFormInputType.MoveLeft:
                    {
                        cursorPos = moveCursorLeft(cursorPos);
                        UpdateCursor(cursorPos);
                        break;
                    }
                case NameFormInputType.MoveRight:
                    {
                        cursorPos = moveCursorRight(cursorPos);
                        UpdateCursor(cursorPos);
                        break;
                    }
                case NameFormInputType.RemoveBefore:
                    {
                        if (cursorPos > 0 && RemoveAt(cursorPos - 1))
                        {
                            cursorPos = moveCursorLeft(cursorPos);
                            PrintFormValue(cursorPos);
                            UpdateCursor(cursorPos);
                        }
                        break;
                    }
                case NameFormInputType.RemoveAfter:
                    {
                        if (RemoveAt(cursorPos))
                        {
                            PrintFormValue(cursorPos);
                            UpdateCursor(cursorPos);
                        }
                        break;
                    }
                case NameFormInputType.Return:
                    {
                        formResult.ResultType = NameFormResultType.Success;
                        formResult.Name = new string(nameFormValue).Trim();
                        inPrompt = false;
                        break;
                    }
                case NameFormInputType.Cancel:
                    {
                        DisplayManager.SetCursorVisibility(false);
                        var errorMessageOverlay = new MessageOverlay("You have cancelled the name form action...", MessageType.Error);
                        errorMessageOverlay.SetPreviousOverlaySuppression(true);
                        errorMessageOverlay.PrintMessage();
                        formResult.ResultType = NameFormResultType.Cancelled;
                        inPrompt = false;
                        break;
                    }
                default:
                    break;
            }
        }
        DisplayManager.SetCursorVisibility(false);
        DisplayManager.RollBackOverLay(suppressPrintingPreviosOverlay);
        return formResult;
    }

    /// <summary>
    /// Prints the prompt label above the name form.
    /// </summary>
    static void PrintPromptLabel()
    {
        int promptLabel1VerticalOffset = DisplayManager.Height / 2 - 2;
        int promptLabel2VerticalOffset = DisplayManager.Height / 2 - 1;
        int promptLabel1HorizontalOffset = (DisplayManager.Width - PromptLabel1.Length) / 2;
        int promptLabel2HorizontalOffset = (DisplayManager.Width - PromptLabel2.Length) / 2;

        //Clear area first
        for (int i = 0; i < 4; i++)
        {
            DisplayManager.PrintText(
                new string(' ', DisplayManager.Width),
                promptLabel1VerticalOffset - 1 + i,
                0,
                DisplayManager.DefaultBackgroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }

        DisplayManager.PrintText(
            PromptLabel1,
            promptLabel1VerticalOffset,
            promptLabel1HorizontalOffset,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        DisplayManager.PrintText(
            PromptLabel2,
            promptLabel2VerticalOffset,
            promptLabel2HorizontalOffset,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
    }

    /// <summary>
    /// Initializes the name form under the prompt label.
    /// </summary>
    /// <param name="name">The previous name of the player.</param>
    void InitializeFormValue(string name)
    {
        formVerticalOffset = DisplayManager.Height / 2 + 1;
        formHorizontalOffset = (DisplayManager.Width - NameFormLength) / 2;
        for (int i = 0; i < name.Length && i < NameFormLength; i++)
        {
            nameFormValue[i] = name[i];
        }
        for (int i = name.Length; i < NameFormLength; i++)
        {
            nameFormValue[i] = ' ';
        }

        //Clear the area first
        for (int i = 0; i < 3; i++)
        {
            DisplayManager.PrintText(
                new string(' ', DisplayManager.Width),
                formVerticalOffset - 1 + i,
                0,
                DisplayManager.DefaultBackgroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }

        DisplayManager.PrintText(
            new string(nameFormValue),
            formVerticalOffset,
            formHorizontalOffset,
            FormForegroundColor,
            FormBackgroundColor
        );
    }

    /// <summary>
    /// Prints the name form from the specified position.
    /// </summary>
    /// <param name="from">The start index to print the name form from.</param>
    void PrintFormValue(int from)
    {
        DisplayManager.SetCursorVisibility(false);
        string toPrint = string.Concat(nameFormValue.Skip(from));
        DisplayManager.PrintText(
            toPrint,
            formVerticalOffset,
            formHorizontalOffset + from,
            FormForegroundColor,
            FormBackgroundColor
        );
    }

    /// <summary>
    /// Inserts a value into <see cref="nameFormValue"/> at the specified index.
    /// </summary>
    /// <param name="value">The value to insert at <paramref name="index"/>.</param>
    /// <param name="index">The index to insert <paramref name="value"/> to.</param>
    /// <returns>True if the insertion was successful.</returns>
    bool InsertAt(char value, int index)
    {
        if (nameFormValue[NameFormLength - 1] != ' ' || BannedCharacters.Contains(value))
        {
            return false;
        }
        for (int i = NameFormLength - 1; i > index; i--)
        {
            nameFormValue[i] = nameFormValue[i - 1];
        }
        nameFormValue[index] = value;
        return true;
    }

    /// <summary>
    /// Removes a value from <see cref="nameFormValue"/> at the specified index.
    /// </summary>
    /// <param name="index">The index for the value to remove.</param>
    /// <returns>True if the removal was successful.</returns>
    bool RemoveAt(int index)
    {
        if (nameFormValue.All(c => c == ' '))
        {
            return false;
        }
        for (int i = index; i < NameFormLength - 1; i++)
        {
            nameFormValue[i] = nameFormValue[i + 1];
        }
        nameFormValue[NameFormLength - 1] = ' ';
        return true;
    }

    /// <summary>
    /// Updates the cursor position on the display and makes it visible.
    /// </summary>
    /// <param name="index">The index in the name form.</param>
    void UpdateCursor(int index)
    {
        DisplayManager.SetCursorPos(formHorizontalOffset + index, formVerticalOffset);
        DisplayManager.SetCursorVisibility(true);
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        return displayRows.Count > relativeVerticalPosition
            && displayRows[relativeVerticalPosition].ColumnCount > relativeHorizontalPosition
            && displayRows[relativeVerticalPosition][relativeHorizontalPosition].IsSet;
    }

    public void SetPreviousOverlaySuppression(bool previousOverlaySuppression)
    {
        suppressPrintingPreviosOverlay = previousOverlaySuppression;
    }

    public void RestoreAsPrevious()
    { }
}
