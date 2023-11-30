using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Helpers;
using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Display.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.AppUI.Misc;

/// <summary>
/// A class that can be used for displaying messages during the game.
/// </summary>
public class MessageOverlay : IOverLay
{
    /// <summary>
    /// The foreground color of the displayed message.
    /// </summary>
    readonly ConsoleColor messageForegroundColor;
    /// <summary>
    /// The background color of the displayed message.
    /// </summary>
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

    /// <summary>
    /// The rows of the message.
    /// </summary>
    readonly IList<string> messageRows;
    /// <summary>
    /// The vertical offset of the displayed message.
    /// </summary>
    int verticalOffset;
    /// <summary>
    /// The horizontal offset of the displayed message.
    /// </summary>
    int horizontalOffset;

    readonly IList<IDisplayRow> displayRows;
    public IList<IDisplayRow> DisplayRows => displayRows;

    public int RowCount => displayRows.Count;

    /// <summary>
    /// If true then the printing of the overlay under this is suppressed.
    /// </summary>
    bool suppressPrintingPreviosOverlay;

    /// <summary>
    /// Creates a new instance of the <see cref="MessageOverlay"/> class.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="messageType">The type of the message to display.</param>
    public MessageOverlay(string message, MessageType messageType)
    {
        displayRows = new List<IDisplayRow>();
        messageRows = message.Slice(DisplayManager.Width / 2);
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
        suppressPrintingPreviosOverlay = false;
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
        int rowLength = DisplayManager.Width / 2;
        horizontalOffset = (DisplayManager.Width - rowLength) / 2;
        verticalOffset = (DisplayManager.Height - messageRows.Count) / 2;
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
        DisplayManager.RollBackOverLay(suppressPrintingPreviosOverlay);
    }

    /// <summary>
    /// Waits until the correct key is pressed
    /// </summary>
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

    public void SetPreviousOverlaySuppression(bool previousOverlaySuppression)
    {
        suppressPrintingPreviosOverlay = previousOverlaySuppression;
    }
}
