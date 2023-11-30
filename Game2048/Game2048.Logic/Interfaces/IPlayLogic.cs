using Game2048.Logic.Enums;
using Game2048.Shared.Enums;

namespace Game2048.Logic;

/// <summary>
/// Represents a set of methods for handling an active play.
/// </summary>
public interface IPlayLogic : IPlayInstance
{
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
}