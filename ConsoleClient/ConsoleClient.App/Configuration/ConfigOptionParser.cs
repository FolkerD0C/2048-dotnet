using CommandLine;
using ConsoleClient.App.Configuration.Enums;
using ConsoleClient.App.Configuration.Models;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient.App.Configuration;

internal static class ConfigOptionParser
{
    internal static ConfigurationResult SetConfigOptions(ParserResult<ConfigOptions> parserResult)
    {
        var configResult = new ConfigurationResult()
        {
            Result = ConfigurationResultType.Unknown,
            Message = ""
        };
        if (parserResult.Tag == ParserResultType.NotParsed)
        {
            configResult.Result = ConfigurationResultType.NotParsed;
            return configResult;
        }

        var options = parserResult.Value;
        foreach (var option in options.GetType().GetProperties())
        {
            if (option.GetValue(options) is IEnumerable<int> intEnumerableValue && intEnumerableValue is not null && intEnumerableValue.Count() > 0)
            {
                foreach (var item in intEnumerableValue)
                {
                    if (item < ConfigOptionValidator.ValidRanges[option.Name].Min
                        || item > ConfigOptionValidator.ValidRanges[option.Name].Max)
                    {
                        configResult.Result = ConfigurationResultType.Failure;
                        configResult.Message = "Value: '" + item + "' is out of range: '[" +
                            ConfigOptionValidator.ValidRanges[option.Name].Min + "-" +
                            ConfigOptionValidator.ValidRanges[option.Name].Max + "]' in '" + option.Name + "'";
                        return configResult;
                    }
                }
                ConfigItem<IList<int>> newListValue = new()
                {
                    Name = "Default" + option.Name,
                    Value = intEnumerableValue.ToList(),
                    Status = ConfigItemStatus.Unknown
                };
                newListValue = AppEnvironment.ConfigLogic.SetConfigValue(newListValue);
                if (newListValue.Status == ConfigItemStatus.SettingFailed)
                {
                    configResult.Result = ConfigurationResultType.Failure;
                    configResult.Message = "Setting config item '" + newListValue.Name + "' failed.";
                    return configResult;
                }
            }
            else if (option.GetValue(options) is int intValue && intValue != 0)
            {
                if (intValue < ConfigOptionValidator.ValidRanges[option.Name].Min
                    || intValue > ConfigOptionValidator.ValidRanges[option.Name].Max)
                {
                    configResult.Result = ConfigurationResultType.Failure;
                    configResult.Message = "Value: '" + intValue + "' is out of range: '[" +
                        ConfigOptionValidator.ValidRanges[option.Name].Min + "-" +
                        ConfigOptionValidator.ValidRanges[option.Name].Max + "]' in '" + option.Name + "'";
                    return configResult;
                }
                ConfigItem<int> newIntValue = new()
                {
                    Name = "Default" + option.Name,
                    Value = intValue,
                    Status = ConfigItemStatus.Unknown
                };
                newIntValue = AppEnvironment.ConfigLogic.SetConfigValue(newIntValue);
                if (newIntValue.Status == ConfigItemStatus.SettingFailed)
                {
                    configResult.Result = ConfigurationResultType.Failure;
                    configResult.Message = "Setting config item '" + newIntValue.Name + "' failed.";
                    return configResult;
                }
            }
        }
        configResult.Result = ConfigurationResultType.Success;
        return configResult;
    }
}
