namespace ConsoleClient.App.Configuration.Models;

/// <summary>
/// A class used for validating that an integer is inside a specified range.
/// </summary>
/// <param name="Min">The minimum value of the integer, inclusive.</param>
/// <param name="Max">The maximum value of the integer, inclusive.</param>
internal record ValidRange(int Min, int Max);
