using ConsoleClient.Menu.Enums;
using ConsoleClient.Menu.EventHandlers;
using System;

namespace ConsoleClient.Menu;

/// <summary>
/// Represents an interactive menu with <see cref="IMenuItem"/>s and optional display text.
/// </summary>
public interface IConsoleMenu
{
    /// <summary>
    /// Adds a <see cref="MenuItemResult"/> to the navigation breaker list.
    /// If an <see cref="IMenuItem"/> returns a <see cref="MenuItemResult"/> contained in that list, then the navigation ends.
    /// </summary>
    /// <param name="result"></param>
    void AddNavigationBreaker(MenuItemResult result);

    /// <summary>
    /// Starts an interactive navigation section for the <see cref="IConsoleMenu"/>.
    /// </summary>
    /// <returns></returns>
    MenuItemResult Navigate();

    /// <summary>
    /// Ends the interactive navigation section for the <see cref="IConsoleMenu"/>.
    /// </summary>
    void EndNavigation();

    /// <summary>
    /// An event that triggers when  the navigation starts.
    /// </summary>
    event EventHandler<MenuNavigationStartedEventArgs>? MenuNavigationStarted;

    /// <summary>
    /// An event that triggers when the selection changes.
    /// </summary>
    event EventHandler<MenuSelectionChangedEventArgs>? MenuSelectionChanged;

    /// <summary>
    /// An event that triggers when the navigation ends.
    /// </summary>
    event EventHandler? MenuNavigationEnded;

    /// <summary>
    /// An event that triggers when an <see cref="IMenuItem.Select"/> action returns <see cref="MenuItemResult.Yes"/>.
    /// </summary>
    event EventHandler? MenuItemReturnedYes;
}