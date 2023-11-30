using System;
using System.Collections.Generic;

namespace ConsoleClient.Display.Models;

/// <summary>
/// Represents a row of display positions for the <see cref="IOverLay"/> interface.
/// </summary>
public interface IDisplayRow
{
    /// <summary>
    /// A list of <see cref="DisplayPosition"/>s that are printed.
    /// </summary>
    IList<DisplayPosition> DisplayPositions { get; }

    /// <summary>
    /// The count of <see cref="DisplayPosition"/>s stored in <see cref="DisplayPositions"/>.
    /// </summary>
    int ColumnCount { get; }

    /// <summary>
    /// True if any of the display positions in this row is set/printed.
    /// </summary>
    bool IsSet { get; }

    /// <summary>
    /// An indexer for the <see cref="DisplayPositions"/> list.
    /// </summary>
    /// <param name="index">The column number of the position.</param>
    /// <returns></returns>
    DisplayPosition this[int index] { get; set; }

    /// <summary>
    /// Returns self, but padded to the right, filled with <see cref="DisplayPosition"/>s that are set to the specified values.
    /// </summary>
    /// <param name="width">The width to pad to.</param>
    /// <param name="foregroundColor">The foreground color to set the new display positions to.</param>
    /// <param name="backgroundColor">The background color to set the new display positions to.</param>
    /// <param name="value">The display value to set the display positions to.</param>
    /// <param name="isSet">The set value to set the display positions to.</param>
    /// <returns></returns>
    IDisplayRow PadRight(int width, ConsoleColor foregroundColor, ConsoleColor backgroundColor, char value, bool isSet);
}