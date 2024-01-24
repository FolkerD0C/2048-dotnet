using ConsoleClient.Display.Models;
using Game2048.Managers.EventHandlers;
using System;

namespace ConsoleClient.AppUI.Play;

/// <summary>
/// Defines methods that can be subscribed to a <see cref="Game2048.Managers.IPlayInstanceManager"/> objects's events.
/// </summary>
public interface IGameDisplay : IOverLay
{
    /// <summary>
    /// Handles play started events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnPlayStarted(object? sender, PlayStartedEventArgs args);

    /// <summary>
    /// Handles move happened events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnMoveHappened(object? sender, MoveHappenedEventArgs args);

    /// <summary>
    /// Handles undo happened events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnUndoHappened(object? sender, UndoHappenedEventArgs args);

    /// <summary>
    /// Handles error happened events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnErrorHappened(object? sender, ErrorHappenedEventArgs args);

    /// <summary>
    /// Handles misc events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void MiscEventHappenedDispatcher(object? sender, MiscEventHappenedEventArgs args);

    /// <summary>
    /// Handles player name changed events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnPlayerNameChanged(object? sender, PlayerNameChangedEventArgs args);

    /// <summary>
    /// Handles input processed events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnInputProcessingFinished(object? sender, EventArgs args);

    /// <summary>
    /// Handles play ended events.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Additional information about the event.</param>
    void OnPlayEnded(object? sender, EventArgs args);
}