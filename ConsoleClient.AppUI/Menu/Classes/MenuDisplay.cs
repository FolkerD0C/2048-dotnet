using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Menu;
using ConsoleClient.Menu.EventHandlers;
using ConsoleClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.AppUI.Menu;

public class MenuDisplay : IMenuDisplay
{
    static ConsoleColor selectedMenuItemForeground;
    static ConsoleColor selectedMenuItemBackground;

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

    IList<IMenuItem> menuItems;

    int displayTextRowCount;
    int longestRowLength;

    int verticalOffset;
    int horizontalOffset;

    public MenuDisplay()
    {
        selectedMenuItemForeground = ConsoleColor.White;
        selectedMenuItemBackground = ConsoleColor.Red;
        displayRows = new List<IDisplayRow>();
        menuItems = new List<IMenuItem>();
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
        displayTextRowCount = args.DisplayText is not null ? args.DisplayText.Count : 0;
        if (displayTextRowCount >= DisplayManager.Height)
        {
            throw new InvalidOperationException("Display text can not be longer then the display.");
        }
        menuItems = args.MenuItems;

        int fullLength = menuItems.Count + displayTextRowCount;
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
        horizontalOffset = longestRowLength < DisplayManager.Width
            ? (DisplayManager.Width - longestRowLength) / 2 : 0;

        DisplayManager.NewOverlay(this);

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
                foregroundColor = selectedMenuItemForeground;
                backgroundColor = selectedMenuItemBackground;
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
            selectedMenuItemForeground,
            selectedMenuItemBackground
        );
    }

    public void OnMenuNavigationEnded(object? sender, EventArgs args)
    {
        DisplayManager.RollBackOverLay();
    }
}
