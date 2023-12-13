using ConsoleClient.Display.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.Display;

/// <summary>
/// A static class that wraps the <see cref="Console"/> class.
/// </summary>
public static class DisplayManager
{
    /// <summary>
    /// The default foreground color for the display.
    /// </summary>
    public static readonly ConsoleColor DefaultForegroundColor = ConsoleColor.White;
    /// <summary>
    /// The default background color for the display.
    /// </summary>
    public static readonly ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    /// <summary>
    /// The default value for a display position.
    /// </summary>
    public static readonly char DefaultDisplayPositionValue = ' ';
    /// <summary>
    /// The horizontal offset of the display from the left edge of the terminal window.
    /// </summary>
    static int offsetHorizontal;
    /// <summary>
    /// The vertical offset of the display from the upper edge of the terminal window.
    /// </summary>
    static int offsetVertical;

    static int height;
    /// <summary>
    /// The height of the display.
    /// </summary>
    public static int Height => height;

    static int width;
    /// <summary>
    /// The width of the display.
    /// </summary>
    public static int Width => width;

    /// <summary>
    /// A default value for display positions.
    /// </summary>
    static DisplayPosition defaultDisplayPosition = new()
    {
        BackgroundColor = DefaultBackgroundColor,
        ForegroundColor = DefaultForegroundColor,
        Value = DefaultDisplayPositionValue,
        IsSet = true
    };

    /// <summary>
    /// The current overlay printed on the display.
    /// </summary>
    static IOverLay CurrentOverlay = new NullOverlay();

    /// <summary>
    /// A stack of overlays printed on top of each other.
    /// </summary>
    static readonly Stack<IOverLay> OverlayStack = new();

    /// <summary>
    /// Initializes some default values.
    /// </summary>
    public static void Initialize(int terminalHeight, int terminalWidth)
    {
        Console.CursorVisible = false;
        height = Console.WindowHeight < terminalHeight ? Console.WindowHeight : terminalHeight;
        width = Console.WindowWidth < terminalWidth ? Console.WindowWidth : terminalWidth;
        offsetHorizontal = (Console.WindowWidth - width) / 2;
        offsetVertical = (Console.WindowHeight - height) / 2;
        CurrentOverlay = new BaseOverlay();
        Console.Clear();
    }

    public static void EndDsiplay()
    {
        Console.SetCursorPosition(width - 1, height - 1);
        Console.CursorVisible = true;
    }

    /// <summary>
    /// Prints a new overlay on the display.
    /// </summary>
    /// <param name="overlay">The <see cref="IOverLay"/> to print.</param>
    public static void NewOverlay(IOverLay overlay)
    {
        overlay.PrintAsNew();
        OverlayStack.Push(CurrentOverlay);
        CurrentOverlay = overlay;
    }

    /// <summary>
    /// Rolls back to the previous overlay (as well as pops the overlay stack).
    /// </summary>
    /// <param name="suppressPrintingPrevious">Wether the printing of the previous overlay should be suppressed.</param>
    public static void RollBackOverLay(bool suppressPrintingPrevious)
    {
        if (suppressPrintingPrevious)
        {
            var overlayToDispose = OverlayStack.Pop();
            CurrentOverlay.MergeWithPrevious(overlayToDispose);
            overlayToDispose.Dispose();
        }
        else
        {
            var previous = OverlayStack.Pop();
            previous.PrintAsPrevious();
            CurrentOverlay.Dispose();
            CurrentOverlay = previous;
            CurrentOverlay.RestoreAsPrevious();
        }
    }

    /// <summary>
    /// Prints the specified text at the specified position with the specified colors.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="relativeVerticalPosition">The vertical position to print <paramref name="text"/>.</param>
    /// <param name="relativeHorizontalPosition">The horizontal position to print <paramref name="text"/>.</param>
    /// <param name="foregroundColor">The foreground color to use when printing <paramref name="text"/>.</param>
    /// <param name="backgroundColor">The background color to use when printing <paramref name="text"/>.</param>
    /// <exception cref="InvalidOperationException"></exception>
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
            DisplayPosition displayPosition = new()
            {
                Value = text[i],
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor,
                IsSet = true
            };
            CurrentOverlay[relativeVerticalPosition][relativeHorizontalPosition + i] = displayPosition;
        }
    }

    /// <summary>
    /// Prints a single <see cref="DisplayPosition"/>.
    /// </summary>
    /// <param name="relativeVerticalPosition">The vertical position to print <paramref name="toPrint"/> to.</param>
    /// <param name="relativeHorizontalPosition">The horizontal position to print <paramref name="toPrint"/> to.</param>
    /// <param name="toPrint">The value to print to.</param>
    static void PrintDisplayPosition(int relativeVerticalPosition, int relativeHorizontalPosition, DisplayPosition toPrint)
    {
        SetCursorPos(relativeHorizontalPosition, relativeVerticalPosition);
        Console.ForegroundColor = toPrint.ForegroundColor;
        Console.BackgroundColor = toPrint.BackgroundColor;
        Console.Write(toPrint.Value);
        toPrint.IsSet = true;
    }

    /// <summary>
    /// Sets the console's cursor visibility to <paramref name="visible"/>.
    /// </summary>
    /// <param name="visible">A boolean value.</param>
    public static void SetCursorVisibility(bool visible)
    {
        Console.CursorVisible = visible;
    }

    /// <summary>
    /// Sets the console's cursor position.
    /// </summary>
    /// <param name="relativeHorizontalPosition">The horizontal position to set the cursor to.</param>
    /// <param name="relativeVerticalPosition">The vertical position to set the cursor to.</param>
    public static void SetCursorPos(int relativeHorizontalPosition, int relativeVerticalPosition)
    {
        Console.SetCursorPosition(offsetHorizontal + relativeHorizontalPosition, offsetVertical + relativeVerticalPosition);
    }

    /// <summary>
    /// Prints <paramref name="overlay"/> as a new overlay.
    /// </summary>
    /// <param name="overlay">The <see cref="IOverLay"/> value to print to.</param>
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

    /// <summary>
    /// Prints <paramref name="overlay"/> as a previous overlay.
    /// </summary>
    /// <param name="overlay">The <see cref="IOverLay"/> value to print to.</param>
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

    /// <summary>
    /// Merges two overlays together. <paramref name="currentOverlay"/> is the upper.
    /// </summary>
    /// <param name="currentOverlay">The upper overlay value.</param>
    /// <param name="otherOverlay">The lower overlay value.</param>
    static void MergeWithPrevious(this IOverLay currentOverlay, IOverLay otherOverlay)
    {
        for (int i = 0; i < currentOverlay.RowCount || i < otherOverlay.RowCount; i++)
        {
            if (!currentOverlay[i].IsSet && !otherOverlay[i].IsSet)
            {
                continue;
            }
            for (int j = 0; j < currentOverlay[i].ColumnCount || j < otherOverlay[i].ColumnCount; j++)
            {
                if (!currentOverlay[i][j].IsSet && otherOverlay[i][j].IsSet)
                {
                    currentOverlay[i][j] = otherOverlay[i][j];
                }
            }
        }
    }
}
