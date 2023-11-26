using ConsoleClient.AppUI.Enums;
using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.AppUI.Misc;

public class MessageOverlay : IOverLay
{
    readonly ConsoleColor messageForegroundColor;
    readonly ConsoleColor messageBackgroundColor;

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

    readonly IList<string> messageRows;
    int verticalOffset;
    int horizontalOffset;
    int rowLength;

    readonly IList<IDisplayRow> displayRows;
    public IList<IDisplayRow> DisplayRows => displayRows;

    public int RowCount => displayRows.Count;

    public MessageOverlay(string message, MessageType messageType)
    {
        displayRows = new List<IDisplayRow>();
        messageRows = SplitMessage(message);
        switch (messageType)
        {
            case MessageType.Error:
                {
                    messageForegroundColor = ConsoleColor.White;
                    messageBackgroundColor = ConsoleColor.Red;
                    break;
                }
            case MessageType.Success:
                {
                    messageForegroundColor = ConsoleColor.DarkMagenta;
                    messageBackgroundColor = ConsoleColor.Green;
                    break;
                }
            default:
                break;
        }
    }

    // TODO Split on words
    IList<string> SplitMessage(string message)
    {
        rowLength = DisplayManager.Width / 2;
        horizontalOffset = (DisplayManager.Width - rowLength) / 2;
        IList<string> result = new List<string>();
        while (message.Length >= rowLength)
        {
            result.Add(message[..rowLength]);
            message = message[rowLength..];
        }
        result.Add(message);
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

    public void PrintMessage()
    {
        DisplayManager.NewOverlay(this);

        //Clear the area first
        for (int i = 0; i < messageRows.Count + 4; i++)
        {
            DisplayManager.PrintText(
                new string(' ', DisplayManager.Width),
                verticalOffset - 1 + i,
                0,
                DisplayManager.DefaultBackgroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }

        for (int i = 0; i < messageRows.Count; i++)
        {
            DisplayManager.PrintText(
                messageRows[i].PadRight(rowLength),
                verticalOffset + i,
                horizontalOffset,
                messageForegroundColor,
                messageBackgroundColor
            );
        }
        ProvideCorrectInput();
        DisplayManager.RollBackOverLay();
    }

    void ProvideCorrectInput()
    {
        string continueMessage = "Press space to continue...";
        int continueMessageHorizontalOffset = (DisplayManager.Width - continueMessage.Length) / 2;
        do
        {
            DisplayManager.PrintText(
                continueMessage,
                verticalOffset + messageRows.Count + 1,
                continueMessageHorizontalOffset,
                messageForegroundColor,
                messageBackgroundColor
            );
        }
        while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);
    }
}
