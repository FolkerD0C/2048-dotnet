using ConsoleClient.Display.Models;
using ConsoleClient.Menu.EventHandlers;
using System;

namespace ConsoleClient.AppUI.Menu;

/// <summary>
/// Defines methods that can be subscribed to a <see cref="ConsoleClient.Menu.IConsoleMenu"/> object's events.
/// </summary>
public interface IMenuDisplay : IOverLay
{
    /// <summary>
    /// Handles menu navigation started events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnMenuNavigationStarted(object? sender, MenuNavigationStartedEventArgs args);

    /// <summary>
    /// Handles menu selectection changed events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnMenuSelectionChanged(object? sender, MenuSelectionChangedEventArgs args);

    /// <summary>
    /// Handles menu navigation ended events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnMenuNavigationEnded(object? sender, EventArgs args);
}