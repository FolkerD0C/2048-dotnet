using ConsoleClient.App.Configuration.Models;
using System.Collections.Generic;

namespace ConsoleClient.App.Configuration;

/// <summary>
/// Stores a validator for integer values that come from outside of the application as command line arguments.
/// </summary>
internal static class ConfigOptionValidator
{
    /// <summary>
    /// The dictionary that stores the validators for the individual configuration items.
    /// </summary>
    internal static Dictionary<string, ValidRange> ValidRanges = new()
    {
        { "AcceptedSpawnables", new ValidRange(1, 100000) },
        { "Goal", new ValidRange(100, 2000000000) },
        { "MaxLives", new ValidRange(1, 100) },
        { "MaxUndos", new ValidRange(1, 100) },
        { "GridHeight", new ValidRange(3, 10) },
        { "GridWidth", new ValidRange(3, 7) },
        { "StarterTiles", new ValidRange(1, int.MaxValue) },
    };
}
