using _2048ish.Base.Models;
using CommandLine;
using ConsoleClient.App.Configuration.Enums;
using ConsoleClient.App.Configuration.Models;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.App.Configuration;

/// <summary>
/// A static class used for setting configuration values that come from outside the app as command line arguments.
/// </summary>
internal static class ConfigOptionParser
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
    };

    static bool ValidateOption(string optionName, int optionValue)
    {
        return optionValue >= ValidRanges[optionName].Min && optionValue <= ValidRanges[optionName].Max;
    }

    static string ConstructOutOfRangeMessage(string optionName, int optionValue)
    {
        return "Value: '" + optionValue + "', is out of range: [" +
            ValidRanges[optionName] + "], in '" +
            optionName + "'.";
    }

    /// <summary>
    /// Sets the configuration values specified in <paramref name="parserResult"/>.
    /// </summary>
    /// <param name="parserResult">The result of the command line parser.</param>
    /// <returns>The result of all the configuration value settings.</returns>
    internal static ConfigurationResult SetConfigOptions(ParserResult<ConfigOptions> parserResult)
    {
        var configResult = new ConfigurationResult()
        {
            ResultType = ConfigurationResultType.Failure,
            Message = ""
        };

        if (parserResult.Tag == ParserResultType.NotParsed)
        {
            configResult.ResultType = ConfigurationResultType.NotParsed;
            return configResult;
        }

        var options = parserResult.Value;
        var newGameConfig = new NewGameConfiguration();

        if (options.AcceptedSpawnables is not null && options.AcceptedSpawnables.Any())
        {
            foreach (int acceptedSpawnable in options.AcceptedSpawnables)
            {
                if (!ValidateOption("AcceptedSpawnables", acceptedSpawnable))
                {
                    configResult.Message = ConstructOutOfRangeMessage("AcceptedSpawnables", acceptedSpawnable);
                    return configResult;
                }
            }
            newGameConfig.AcceptedSpawnables = options.AcceptedSpawnables.ToList();
        }

        if (options.Goal != 0)
        {
            if (!ValidateOption("Goal", options.Goal))
            {
                configResult.Message = ConstructOutOfRangeMessage("Goal", options.Goal);
                return configResult;
            }
            newGameConfig.Goal = options.Goal;
        }

        if (options.MaxLives != 0)
        {
            if (!ValidateOption("MaxLives", options.MaxLives))
            {
                configResult.Message = ConstructOutOfRangeMessage("MaxLives", options.MaxLives);
                return configResult;
            }
            newGameConfig.MaxLives = options.MaxLives;
        }

        if (options.MaxUndos != 0)
        {
            if (!ValidateOption("MaxUndos", options.MaxUndos))
            {
                configResult.Message = ConstructOutOfRangeMessage("MaxUndos", options.MaxUndos);
                return configResult;
            }
            newGameConfig.MaxUndos = options.MaxUndos;
        }

        if (options.GridHeight != 0)
        {
            if (!ValidateOption("GridHeight", options.GridHeight))
            {
                configResult.Message = ConstructOutOfRangeMessage("GridHeight", options.GridHeight);
                return configResult;
            }
            newGameConfig.GridHeight = options.GridHeight;
        }

        if (options.GridWidth != 0)
        {
            if (!ValidateOption("GridWidth", options.GridWidth))
            {
                configResult.Message = ConstructOutOfRangeMessage("GridWidth", options.GridWidth);
                return configResult;
            }
            newGameConfig.GridWidth = options.GridWidth;
        }

        if (options.StarterTiles != 0)
        {
            if (options.StarterTiles < 1 || newGameConfig.StarterTiles > newGameConfig.GridHeight * options.GridWidth - 1)
            {
                configResult.Message = $"Value: {options.StarterTiles}, is out of range: [{new ValidRange(1, newGameConfig.GridHeight * newGameConfig.GridWidth - 1)}], in 'StarterTiles'.";
                return configResult;
            }
            newGameConfig.StarterTiles = options.StarterTiles;
        }

        configResult.ConfiguredValues = newGameConfig;

        configResult.ResultType = ConfigurationResultType.Success;
        return configResult;
    }
}
