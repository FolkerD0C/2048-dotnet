using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Display.Models;
using ConsoleClient.Menu;
using ConsoleClient.Menu.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.AppUI.Menu;

/// <summary>
/// A class used for displaying a menu and holds methods that can be subscribed to an <see cref="IConsoleMenu"/> object's events.
/// </summary>
public class MenuDisplay : IMenuDisplay
{
    /// <summary>
    /// The foreground color of the selected <see cref="IMenuItem"/>.
    /// </summary>
    const ConsoleColor SelectedMenuItemForeground = ConsoleColor.White;
    /// <summary>
    /// The background color of the selected <see cref="IMenuItem"/>.
    /// </summary>
    const ConsoleColor SelectedMenuItemBackground = ConsoleColor.Red;
    /// <summary>
    /// The value that gets printed all over the frame of the menu.
    /// </summary>
    const char FrameDisplayValue = '#';
    /// <summary>
    /// The foreground color of the frame.
    /// </summary>
    const ConsoleColor FrameDisplayForeground = ConsoleColor.DarkMagenta;
    /// <summary>
    /// The background color of the frame.
    /// </summary>
    const ConsoleColor FrameDisplayBackground = ConsoleColor.Yellow;

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
    /// The <see cref="IMenuItem"/>s that the <see cref="IConsoleMenu"/> contains.
    /// </summary>
    IList<IMenuItem> menuItems;

    /// <summary>
    /// The vertical length of the optional display text.
    /// </summary>
    int displayTextRowCount;
    /// <summary>
    /// The length of the longest row to display.
    /// </summary>
    int longestRowLength;

    /// <summary>
    /// The vertical offset of the menu display.
    /// </summary>
    int verticalOffset;
    /// <summary>
    /// The horizontal offset of the menu display.
    /// </summary>
    int horizontalOffset;

    /// <summary>
    /// If true then the printing of the overlay under this is suppressed.
    /// </summary>
    bool suppressPrintingPreviosOverlay;

    /// <summary>
    /// Creates a new instance of the <see cref="MenuDisplay"/> class.
    /// </summary>
    public MenuDisplay()
    {
        displayRows = new List<IDisplayRow>();
        menuItems = new List<IMenuItem>();
        suppressPrintingPreviosOverlay = false;
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        return displayRows.Count > relativeVerticalPosition
            && displayRows[relativeVerticalPosition].ColumnCount > relativeHorizontalPosition
            && displayRows[relativeVerticalPosition][relativeHorizontalPosition].IsSet;
    }

    public void Dispose()
    {
        displayRows.Dispose();
        GC.SuppressFinalize(this);
    }

    public void OnMenuNavigationStarted(object? sender, MenuNavigationStartedEventArgs args)
    {
        bool horizontalFramePrintable = false;
        bool verticalFramePrintable = false;

        displayTextRowCount = args.DisplayText is not null ? args.DisplayText.Count : 0;
        if (displayTextRowCount >= DisplayManager.Height)
        {
            throw new InvalidOperationException("Display text can not be longer then the display.");
        }
        menuItems = args.MenuItems;

        int fullLength = menuItems.Count + displayTextRowCount;
        if (fullLength + 2 <= DisplayManager.Height)
        {
            horizontalFramePrintable = true;
        }
        verticalOffset = fullLength >= DisplayManager.Height
            ? 0 : (DisplayManager.Height - fullLength) / 2;

        int longestDisplayTextRowLength = args.DisplayText is not null ?
            args.DisplayText.Max(displayTextRow => displayTextRow.Length) : 0;
        int longestMenuItemRowLength = menuItems.Select(menuItem => menuItem.Name)
            .Max(menuItemName => menuItemName.Length);
        longestRowLength = Math.Max(longestMenuItemRowLength, longestDisplayTextRowLength);
        if (longestRowLength > DisplayManager.Width)
        {
            throw new InvalidOperationException("Display text can not be longer then the display.");
        }
        if (longestRowLength + 2 <= DisplayManager.Width)
        {
            verticalFramePrintable = true;
        }
        horizontalOffset = longestRowLength < DisplayManager.Width
            ? (DisplayManager.Width - longestRowLength) / 2 : 0;

        DisplayManager.NewOverlay(this);

        if (horizontalFramePrintable)
        {
            DisplayManager.PrintText(
                new string(FrameDisplayValue, longestRowLength),
                verticalOffset - 1,
                horizontalOffset,
                FrameDisplayForeground,
                FrameDisplayBackground
            );
            DisplayManager.PrintText(
                new string(FrameDisplayValue, longestRowLength),
                verticalOffset + fullLength,
                horizontalOffset,
                FrameDisplayForeground,
                FrameDisplayBackground
            );
        }

        if (verticalFramePrintable)
        {
            for (int i = 0; i < fullLength; i++)
            {
                DisplayManager.PrintText(
                    "" + FrameDisplayValue,
                    verticalOffset + i,
                    horizontalOffset - 1,
                    FrameDisplayForeground,
                    FrameDisplayBackground
                );
                DisplayManager.PrintText(
                    "" + FrameDisplayValue,
                    verticalOffset + i,
                    horizontalOffset + longestRowLength,
                    FrameDisplayForeground,
                    FrameDisplayBackground
                );
            }
        }

        if (horizontalFramePrintable && verticalFramePrintable)
        {
            DisplayManager.PrintText(
                "" + FrameDisplayValue,
                verticalOffset - 1,
                horizontalOffset - 1,
                FrameDisplayForeground,
                FrameDisplayBackground
            );
            DisplayManager.PrintText(
                "" + FrameDisplayValue,
                verticalOffset - 1,
                horizontalOffset + longestRowLength,
                FrameDisplayForeground,
                FrameDisplayBackground
            );
            DisplayManager.PrintText(
                "" + FrameDisplayValue,
                verticalOffset + fullLength,
                horizontalOffset - 1,
                FrameDisplayForeground,
                FrameDisplayBackground
            );
            DisplayManager.PrintText(
                "" + FrameDisplayValue,
                verticalOffset + fullLength,
                horizontalOffset + longestRowLength,
                FrameDisplayForeground,
                FrameDisplayBackground
            );
        }

        for (int i = 0; i < displayTextRowCount; i++)
        {
            DisplayManager.PrintText(
#pragma warning disable CS8604
                args.DisplayText?[i].PadRight(longestRowLength),
#pragma warning restore CS8604
                verticalOffset + i,
                horizontalOffset,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }

        // TODO: Handle rolling menus, where full length is longer than DisplayManager.Height
        for (int i = 0; i < menuItems.Count; i++)
        {
            ConsoleColor foregroundColor = DisplayManager.DefaultForegroundColor;
            ConsoleColor backgroundColor = DisplayManager.DefaultBackgroundColor;
            if (i == args.SelectedMenuItem)
            {
                foregroundColor = SelectedMenuItemForeground;
                backgroundColor = SelectedMenuItemBackground;
            }
            DisplayManager.PrintText(
                menuItems[i].Name.PadRight(longestRowLength),
                verticalOffset + displayTextRowCount + i,
                horizontalOffset,
                foregroundColor,
                backgroundColor
            );
        }
    }

    // TODO: Handle rolling menus, where full length is longer than DisplayManager.Height
    public void OnMenuSelectionChanged(object? sender, MenuSelectionChangedEventArgs args)
    {
        if (args.PreviousItem == args.NewItem)
        {
            return;
        }
        DisplayManager.PrintText(
            menuItems[args.PreviousItem].Name.PadRight(longestRowLength),
            verticalOffset + displayTextRowCount + args.PreviousItem,
            horizontalOffset,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        DisplayManager.PrintText(
            menuItems[args.NewItem].Name.PadRight(longestRowLength),
            verticalOffset + displayTextRowCount + args.NewItem,
            horizontalOffset,
            SelectedMenuItemForeground,
            SelectedMenuItemBackground
        );
    }

    public void OnMenuNavigationEnded(object? sender, EventArgs args)
    {
        DisplayManager.RollBackOverLay(suppressPrintingPreviosOverlay);
    }

    public void SetPreviousOverlaySuppression(bool previousOverlaySuppression)
    {
        suppressPrintingPreviosOverlay = previousOverlaySuppression;
    }
}
