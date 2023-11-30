using System;

namespace ConsoleClient.Display.Models;

/// <summary>
/// Represents a simple display position managed by <see cref="IOverLay"/>s and the <see cref="DisplayManager"/>.
/// </summary>
public struct DisplayPosition
{
    /// <summary>
    /// The foreground color of the display position.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// The background color of the display position.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; }


    /// <summary>
    /// The value of the display position.
    /// </summary>
    public char Value { get; set; }

    /// <summary>
    /// Wether this display position is set/printed.
    /// </summary>
    public bool IsSet { get; set; }
}