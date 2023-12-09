using CommandLine;
using System.Collections.Generic;

namespace ConsoleClient.App.Configuration;

/// <summary>
/// A class that holds information for the command line argument parser.
/// </summary>
internal class ConfigOptions
{
    [Option('a', "accepted-spawnables", HelpText = "A comma separated list of integers that could spawn randomly while playing, after a valid move happens, all of them must be between 1 and 100000", MetaValue = "number,number,...", Separator = ',')]
    public IEnumerable<int>? DefaultAcceptedSpawnables { get; set; }

    [Option('g', "goal", HelpText = "The goal of the game for a new game, must be between 100 and 2000000000", MetaValue = "number")]
    public int DefaultGoal { get; set; }

    [Option('l', "max-lives", HelpText = "Max lives for a new game, must be between 1 and 100", MetaValue = "number")]
    public int DefaultMaxLives { get; set; }

    [Option('u', "max-undos", HelpText = "Maximum length of the undo chain for a new game, must be between 1 and 100", MetaValue = "number")]
    public int DefaultMaxUndos { get; set; }

    [Option('h', "grid-height", HelpText = "Grid height for a new game, must be between 3 and 10", MetaValue = "number")]
    public int DefaultGridHeight { get; set; }

    [Option('w', "grid-width", HelpText = "Grid width for a new game, must be between 3 and 7", MetaValue = "number")]
    public int DefaultGridWidth { get; set; }

    [Option('t', "starter-tiles", HelpText = "When you start a new game this many tiles will spawn (randomly)", MetaValue = "number")]
    public int DefaultStarterTiles { get; set; }

    [Option('s', "save-config", HelpText = "If this switch is present then the configuration will be saved")]
    public bool SaveConfig { get; set; }
}
