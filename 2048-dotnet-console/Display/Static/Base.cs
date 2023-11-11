using System;
using System.Collections.Generic;
using Game2048.ConsoleFrontend.Models;

namespace Game2048.ConsoleFrontend.Display;

static class Base
{
    public static readonly ConsoleColor DefaultForegroundColor = ConsoleColor.White;
    public static readonly ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    static readonly int offsetHorizontal;
    static readonly int offsetVertical;

    static readonly int width;
    public static int Width => width;

    static readonly int height;
    public static int Height = height;

    static readonly Stack<IOverLay> DisplayStack;

    #pragma warning disable CS8618
    static IOverLay CurrentDisplay;

    static Base()
    {
        Console.CursorVisible = false;
        width = Console.WindowWidth < 70 ? Console.WindowWidth : 70;
        height = Console.WindowHeight < 40 ? Console.WindowHeight : 40;
        offsetHorizontal = (Console.WindowWidth - width) / 2;
        offsetVertical = (Console.WindowHeight - height) / 2;
        DisplayStack = new Stack<IOverLay>();
    }

    static void DrawEntireOverlay()
    {
        for (int i = 0; i < CurrentDisplay.RowCount; i++)
        {
            if (!CurrentDisplay[i].IsSet)
            {
                continue;
            }
            if (CurrentDisplay[i].IsSameColor)
            {
                PrintRow(CurrentDisplay[i].GetFullRow());
            }
            else
            {
                for (int j = 0; j < CurrentDisplay[i].ColumnCount; j++)
                {
                    if (CurrentDisplay[i][j].IsSet)
                    {
                        Print(i, j, CurrentDisplay[i][j]);
                    }
                }
            }
        }
    }

    static void DrawEntireDisplay()
    {
        for (int i = 0; i < CurrentDisplay.RowCount; i++)
        {
            for (int j = 0; j < CurrentDisplay[i].ColumnCount; j++)
            {
                Print(i, j, CurrentDisplay[i][j]);
            }
        }
    }

    public static void NewOverlay(IOverLay overlay)
    {
        DisplayStack.Push(CurrentDisplay);
        CurrentDisplay = overlay;
        DrawEntireOverlay();
    }

    public static void RollBackOverLay()
    {
        CurrentDisplay = DisplayStack.Pop();
        DrawEntireDisplay();
    }

    public static void PrintText(string text, int relativeHorizontalPosition, int relativeVerticalPosition,
            ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)
    {
        for (int i = 0; i < text.Length; i++)
        {
            DisplayPosition val = new DisplayPosition()
            {
                Value = text[i],
                ForeGroundColor = ForegroundColor,
                BackGroundColor = BackgroundColor
            };
            Print(i + relativeHorizontalPosition, relativeVerticalPosition, val);
            CurrentDisplay.DrawPositions[relativeVerticalPosition][relativeHorizontalPosition + i] = val;
        }
    }

    static void Print(int relativHorizontalPosition, int relativeVerticalPosition, DisplayPosition toDraw)
    {
        SetCursorPos(relativHorizontalPosition, relativeVerticalPosition);
        Console.ForegroundColor = toDraw.ForeGroundColor;
        Console.BackgroundColor = toDraw.BackGroundColor;
        Console.Write(toDraw.Value);
    }

    static void PrintRow((ConsoleColor fgColor, ConsoleColor bgColor, string rowValue) args)
    {
        SetCursorPos(0, 0);
        Console.ForegroundColor = args.fgColor;
        Console.BackgroundColor = args.bgColor;
        Console.Write(args.rowValue);
    }

    public static void SetCursorVisibility(bool visible)
    {
        Console.CursorVisible = visible;
    }

    public static void SetCursorPos(int relativeHorizontalPosition, int relativeVerticalPosition)
    {
        Console.SetCursorPosition(offsetHorizontal + relativeHorizontalPosition, offsetVertical + relativeVerticalPosition);
    }
}
