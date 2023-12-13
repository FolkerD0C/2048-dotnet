using CommandLine;
using System.Collections.Generic;

namespace ConsoleClient.App.Configuration;

/// <summary>
/// A class that holds information for the command line argument parser.
/// </summary>
internal class ConfigOptions
{
    [Option('a', "accepted-spawnables", HelpText = "A comma separated list of integers that could spawn randomly while playing, after a valid move happens, all of them must be between 1 and 100000", MetaValue = "number,number,...", Separator = ',')]
    public IEnumerable<int>? AcceptedSpawnables { get; set; }

    [Option('g', "goal", HelpText = "The goal of the game for a new game, must be between 100 and 2000000000", MetaValue = "number")]
    public int Goal { get; set; }

    [Option('l', "max-lives", HelpText = "Max lives for a new game, must be between 1 and 100", MetaValue = "number")]
    public int MaxLives { get; set; }

    [Option('u', "max-undos", HelpText = "Maximum length of the undo chain for a new game, must be between 1 and 100", MetaValue = "number")]
    public int MaxUndos { get; set; }

    [Option('h', "grid-height", HelpText = "Grid height for a new game, must be between 3 and 10", MetaValue = "number")]
    public int GridHeight { get; set; }

    [Option('w', "grid-width", HelpText = "Grid width for a new game, must be between 3 and 7", MetaValue = "number")]
    public int GridWidth { get; set; }

    [Option('t', "starter-tiles", HelpText = "When you start a new game this many tiles will spawn (randomly)", MetaValue = "number")]
    public int StarterTiles { get; set; }

    // TODO add back saving option
}
