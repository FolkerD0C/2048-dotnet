using ConsoleClient.AppUI.Enums;
using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.AppUI.Misc;

public class NameForm : INameForm
{
    const string BannedCharacters = "<>:\"/\\|?*";
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

    int formVerticalOffset;
    int formHorizontalOffset;

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
                        new MessageOverlay("You have cancelled the name form action...", MessageType.Error).PrintMessage();
                        formResult.ResultType = NameFormResultType.Cancelled;
                        inPrompt = false;
                        break;
                    }
                default:
                    break;
            }
        }
        DisplayManager.SetCursorVisibility(false);
        DisplayManager.RollBackOverLay();
        return formResult;
    }

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
}
