using Game2048.ConsoleFrontend.Models;
using System;
using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Display;

static class DisplayManager
{
    public static readonly ConsoleColor DefaultForegroundColor = ConsoleColor.White;
    public static readonly ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    static readonly int offsetHorizontal;
    static readonly int offsetVertical;

    static readonly int width;
    public static int Width => width;

    static readonly int height;
    public static int Height = height;

    static readonly Stack<IOverLay> OverlayStack;

    static IOverLay CurrentOverlay;
    static readonly IDisplayPosition[,] currentDisplayPositions;
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
        currentDisplayPositions = new IDisplayPosition[height, width];
        for (int i = 0; i < CurrentOverlay.RowCount; i++)
        {
            for (int j = 0; j < CurrentOverlay[i].ColumnCount; j++)
            {
                currentDisplayPositions[i, j] = CurrentOverlay[i][j];
            }
        }
        currentDisplayPositionsSet = new bool[height, width];
    }

    public static void NewOverlay(IOverLay overlay)
    {
        ResetDisplayPositions();
        overlay.Print();
        OverlayStack.Push(CurrentOverlay);
        CurrentOverlay = overlay;
    }

    public static void RollBackOverLay()
    {
        CurrentOverlay.PrintUnderlay();
        CurrentOverlay.Dispose();
        CurrentOverlay = OverlayStack.Pop();
    }

    // TODO
    public static void PrintText(string text, int relativeHorizontalPosition, int relativeVerticalPosition,
            ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)
    {
        for (int i = 0; i < text.Length; i++)
        {
            IDisplayPosition displayPosition = new DisplayPosition()
            {
                Value = text[i],
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor
            };
            Print(i + relativeHorizontalPosition, relativeVerticalPosition, displayPosition);
            CurrentOverlay[relativeVerticalPosition][relativeHorizontalPosition + i] = displayPosition;
        }
    }

    static void Print(int relativeVerticalPosition, int relativeHorizontalPosition, IDisplayPosition toDraw)
    {
        SetCursorPos(relativeHorizontalPosition, relativeVerticalPosition);
        Console.ForegroundColor = toDraw.ForegroundColor;
        Console.BackgroundColor = toDraw.BackgroundColor;
        Console.Write(toDraw.Value);
        toDraw.IsSet = true;
        currentDisplayPositionsSet[relativeVerticalPosition, relativeHorizontalPosition] = true;
        currentDisplayPositions[relativeVerticalPosition, relativeHorizontalPosition] = toDraw;
    }

    // TODO
    static void PrintRow(int relativeVerticalPosition, ISameColorDisplayRow displayRow)
    {
        SetCursorPos(0, relativeVerticalPosition);
        Console.ForegroundColor = displayRow.ForegroundColor;
        Console.BackgroundColor = displayRow.BackgroundColor;
        Console.Write(displayRow.RowText);
        displayRow.SetAll();
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

    static void Print(this IOverLay overlay)
    {
        for (int i = 0; i < overlay.RowCount; i++)
        {
            if (!overlay[i].IsSet)
            {
                continue;
            }
            if (overlay[i] is ISameColorDisplayRow sameColorDisplayRow)
            {
                PrintRow(i, sameColorDisplayRow);
                continue;
            }
            for (int j = 0; j < overlay[i].ColumnCount; j++)
            {
                if (overlay[i][j].IsSet)
                {
                    Print(i, j, overlay[i][j]);
                }
            }
        }
    }

    // TODO
    static void PrintUnderlay(this IOverLay overlay)
    {
        for (int i = 0; i < overlay.RowCount; i++)
        {
            if (!overlay[i].IsSet)
            {
                continue;
            }
            for (int j = 0; j < overlay[i].ColumnCount; j++)
            {
                if (!overlay[i][j].IsSet)
                {
                    continue;
                }
                Print(i, j, overlay[i][j].UnderLay ?? throw new ArgumentNullException("toDraw", "UnderLay can not be null"));
            }
        }
    }
}
