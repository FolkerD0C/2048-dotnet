using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace ConsoleClient.Display;

public static class DisplayManager
{
    public static readonly ConsoleColor DefaultForegroundColor = ConsoleColor.White;
    public static readonly ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    static readonly int offsetHorizontal;
    static readonly int offsetVertical;

    static readonly int width;
    public static int Width => width;

    static readonly int height;
    public static int Height => height;

    static readonly Stack<IOverLay> OverlayStack;

    static IOverLay CurrentOverlay;
    static bool[,] currentDisplayPositionsSet;


    static DisplayManager()
    {
        Console.CursorVisible = false;
        width = Console.WindowWidth < 70 ? Console.WindowWidth : 70;
        height = Console.WindowHeight < 40 ? Console.WindowHeight : 40;
        offsetHorizontal = (Console.WindowWidth - width) / 2;
        offsetVertical = (Console.WindowHeight - height) / 2;
        OverlayStack = new Stack<IOverLay>();
        CurrentOverlay = new BaseOverlay();
        currentDisplayPositionsSet = new bool[height, width];
    }

    public static void NewOverlay(IOverLay overlay)
    {
        ResetDisplayPositions();
        overlay.PrintAsNew();
        OverlayStack.Push(CurrentOverlay);
        CurrentOverlay = overlay;
    }

    public static void RollBackOverLay()
    {
        CurrentOverlay.Dispose();
        var previous = OverlayStack.Pop();
        previous.PrintAsPrevious();
        CurrentOverlay = previous;
    }

    // TODO
    public static void PrintText(string text, int relativeHorizontalPosition, int relativeVerticalPosition,
            ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)
    {
        for (int i = 0; i < text.Length; i++)
        {
            DisplayPosition displayPosition = new DisplayPosition()
            {
                Value = text[i],
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor
            };
            PrintDisplayPosition(i + relativeHorizontalPosition, relativeVerticalPosition, displayPosition);
            CurrentOverlay[relativeVerticalPosition][relativeHorizontalPosition + i] = displayPosition;
        }
    }

    // TODO write a method that handles rows with same color text
    // (eg. append this method with a bool or an enum that manages this inside this method)
    // new method would be better
    static void PrintDisplayPosition(int relativeVerticalPosition, int relativeHorizontalPosition, DisplayPosition toDraw)
    {
        SetCursorPos(relativeHorizontalPosition, relativeVerticalPosition);
        Console.ForegroundColor = toDraw.ForegroundColor;
        Console.BackgroundColor = toDraw.BackgroundColor;
        Console.Write(toDraw.Value);
        toDraw.IsSet = true;
    }

    public static void SetCursorVisibility(bool visible)
    {
        Console.CursorVisible = visible;
    }

    public static void SetCursorPos(int relativeHorizontalPosition, int relativeVerticalPosition)
    {
        Console.SetCursorPosition(offsetHorizontal + relativeHorizontalPosition, offsetVertical + relativeVerticalPosition);
    }

    static void ResetDisplayPositions()
    {
        currentDisplayPositionsSet = new bool[height, width];
    }

    static void PrintAsNew(this IOverLay overlay)
    {
        for (int i = 0; i < overlay.RowCount; i++)
        {
            if (!overlay[i].IsSet)
            {
                continue;
            }
            for (int j = 0; j < overlay[i].ColumnCount; j++)
            {
                if (overlay[i][j].IsSet)
                {
                    PrintDisplayPosition(i, j, overlay[i][j]);
                    currentDisplayPositionsSet[i, j] = true;
                }
            }
        }
    }

    static void PrintAsPrevious(this IOverLay overlay)
    {
        // TODO
    }
}
