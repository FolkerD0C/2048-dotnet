using System;
using System.Collections.Generic;

namespace ConsoleClient.Display.Models;

/// <summary>
/// Represents a printable layer for the <see cref="DisplayManager"/>.
/// </summary>
public interface IOverLay : IDisposable
{
    /// <summary>
    /// The list of <see cref="IDisplayRow"/> objects that are printed.
    /// </summary>
    IList<IDisplayRow> DisplayRows { get; }

    /// <summary>
    /// The count of <see cref="IDisplayRow"/> objects stored in <see cref="DisplayRows"/>.
    /// </summary>
    int RowCount { get; }

    /// <summary>
    /// Returns a true if the position at <paramref name="relativeVerticalPosition"/> and <paramref name="relativeHorizontalPosition"/> is set.
    /// </summary>
    /// <param name="relativeVerticalPosition">The vertical orientation of the display position.</param>
    /// <param name="relativeHorizontalPosition">The horizontal orientation of the display position.</param>
    /// <returns>True if the display position specified is set/printed.</returns>
    bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition);

    /// <summary>
    /// An indexer for th <see cref="DisplayRows"/> list.
    /// </summary>
    /// <param name="index">The row number.</param>
    /// <returns>The <see cref="IDisplayRow"/> objects at <paramref name="index"/>.</returns>
    IDisplayRow this[int index] { get; set; }

    /// <summary>
    /// Sets the suppression of the printing of the previous overlay to <paramref name="previousOverlaySuppression"/>.
    /// </summary>
    /// <param name="previousOverlaySuppression">A boolean value.</param>
    void SetPreviousOverlaySuppression(bool previousOverlaySuppression);
}