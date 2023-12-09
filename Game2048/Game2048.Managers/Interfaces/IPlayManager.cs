using Game2048.Managers.Enums;
using Game2048.Managers.EventHandlers;
using Game2048.Processors;
using System;

namespace Game2048.Managers;

/// <summary>
/// Represents a set of methods for handling an active play.
/// </summary>
public interface IPlayManager : IPlayInstance
{
    /// <summary>
    /// The play processor object that stores all playdata
    /// </summary>
    IPlayProcessor Processor { get; }

    /// <summary>
    /// If true then the play is saved.
    /// </summary>
    bool IsSaved { get; set; }

    /// <summary>
    /// Triggers events before a play starts.
    /// </summary>
    void Start();

    /// <summary>
    /// Triggers events after a pley ends.
    /// </summary>
    void End();

    /// <summary>
    /// Trigger events pre-input.
    /// </summary>
    void PreInput();

    /// <summary>
    /// Handles a game input.
    /// </summary>
    /// <param name="input">The input as a <see cref="GameInput"/> enum.</param>
    /// <returns>The result of an input as an <see cref="InputResult"/>.</returns>
    InputResult HandleInput(GameInput input);

    /// <summary>
    /// An event that triggers when the player changes their name.
    /// </summary>
    event EventHandler<PlayerNameChangedEventArgs>? PlayerNameChangedManagerEvent;
}