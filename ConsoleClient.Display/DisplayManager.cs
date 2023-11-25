using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.Display;

public static class DisplayManager
{
    public static readonly ConsoleColor DefaultForegroundColor = ConsoleColor.White;
    public static readonly ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    public static readonly char DefaultDisplayPositionValue = ' ';
    static readonly int offsetHorizontal;
    static readonly int offsetVertical;

    static readonly int height;
    public static int Height => height;

    static readonly int width;
    public static int Width => width;

    static DisplayPosition defaultDisplayPosition;
    static IOverLay CurrentOverlay;

    static readonly Stack<IOverLay> OverlayStack;

    static DisplayManager()
    {
        Console.CursorVisible = false;
        width = Console.WindowWidth < 70 ? Console.WindowWidth : 70;
        height = Console.WindowHeight < 40 ? Console.WindowHeight : 40;
        offsetHorizontal = (Console.WindowWidth - width) / 2;
        offsetVertical = (Console.WindowHeight - height) / 2;
        OverlayStack = new Stack<IOverLay>();
        CurrentOverlay = new BaseOverlay();

        defaultDisplayPosition = new DisplayPosition()
        {
            BackgroundColor = DefaultBackgroundColor,
            ForegroundColor = DefaultForegroundColor,
            Value = DefaultDisplayPositionValue,
            IsSet = true
        };
    }

    public static void NewOverlay(IOverLay overlay)
    {
        overlay.PrintAsNew();
        OverlayStack.Push(CurrentOverlay);
        CurrentOverlay = overlay;
    }

    public static void RollBackOverLay()
    {
        var previous = OverlayStack.Pop();
        previous.PrintAsPrevious();
        CurrentOverlay.Dispose();
        CurrentOverlay = previous;
    }

    public static void PrintText(string text, int relativeVerticalPosition, int relativeHorizontalPosition,
            ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {
        if (relativeHorizontalPosition + text.Length - 1 > width)
        {
            throw new InvalidOperationException("Text is wider then the display");
        }
        SetCursorPos(relativeHorizontalPosition, relativeVerticalPosition);
        Console.ForegroundColor = foregroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.Write(text);

        for (int i = 0; i < text.Length; i++)
        {
            DisplayPosition displayPosition = new DisplayPosition()
            {
                Value = text[i],
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor,
                IsSet = true
            };
            CurrentOverlay[relativeVerticalPosition][relativeHorizontalPosition + i] = displayPosition;
        }
    }

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
                }
            }
        }
    }

    static void PrintAsPrevious(this IOverLay overlay)
    {
        for (int i = 0; i < CurrentOverlay.RowCount; i++)
        {
            if (!CurrentOverlay[i].IsSet)
            {
                continue;
            }
            for (int j = 0; j < CurrentOverlay[i].ColumnCount; j++)
            {
                if (!CurrentOverlay[i][j].IsSet)
                {
                    continue;
                }
                if (overlay.IsPositionSet(i, j))
                {
                    PrintDisplayPosition(i, j, overlay[i][j]);
                    continue;
                }
                var firstDrawableOverlay = OverlayStack.FirstOrDefault(overlay => overlay.IsPositionSet(i, j));
                var displayPositionToPrint = firstDrawableOverlay is not null ? firstDrawableOverlay[i][j] : defaultDisplayPosition;
                PrintDisplayPosition(i, j, displayPositionToPrint);
            }
        }
    }
}
