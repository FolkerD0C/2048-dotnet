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
        { "DefaultAcceptedSpawnables", new ValidRange(1, 100000) },
        { "DefaultGoal", new ValidRange(100, 2000000000) },
        { "DefaultMaxLives", new ValidRange(1, 100) },
        { "DefaultMaxUndos", new ValidRange(1, 100) },
        { "DefaultGridHeight", new ValidRange(3, 10) },
        { "DefaultGridWidth", new ValidRange(3, 7) },
        { "DefaultStarterTiles", new ValidRange(1, int.MaxValue) },
    };
}
