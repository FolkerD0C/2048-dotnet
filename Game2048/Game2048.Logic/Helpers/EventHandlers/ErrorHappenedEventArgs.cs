using System;

namespace Game2048.Logic.EventHandlers;

/// <summary>
/// A class used for event handling that stores information about an error that happened during a play.
/// </summary>
public class ErrorHappenedEventArgs : EventArgs
{
    /// <summary>
    /// The message that describes the error.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ErrorHappenedEventArgs"/> class.
    /// </summary>
    /// <param name="errorMessage">The message that describes the error.</param>
    public ErrorHappenedEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}