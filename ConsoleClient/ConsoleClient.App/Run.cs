using CommandLine;
using ConsoleClient.App.Configuration;
using ConsoleClient.App.Configuration.Enums;
using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Misc;
using System;

namespace ConsoleClient.App;

/// <summary>
/// The entry class of the app.
/// </summary>
class Run
{
    /// <summary>
    /// The main entry point of the game.
    /// </summary>
    /// <param name="args">The command line arguments form outside of the application.</param>
    static void Main(string[] args)
    {
        AppEnvironment.Initialize();
        AppEnvironment.Configuration.LoadConfig();
        var optionsParser = Parser.Default.ParseArguments<ConfigOptions>(args);
        var setConfigResult = ConfigOptionParser.SetConfigOptions(optionsParser);
        switch (setConfigResult.Result)
        {
            case ConfigurationResultType.Failure:
                {
                    new MessageOverlay(setConfigResult.Message ?? "", MessageType.Error).PrintMessage();
                    break;
                }
            case ConfigurationResultType.Success:
                {
                    try
                    {
                        AppEnvironment.MainMenu.Navigate();
                        if (optionsParser.Value.SaveConfig)
                        {
                            AppEnvironment.Configuration.SaveConfig();
                        }
                    }
                    catch (Exception exc)
                    {
                        new MessageOverlay(exc.Message, MessageType.Error).PrintMessage();
                    }
                    finally
                    {
                        AppEnvironment.Shutdown();
                    }
                    break;
                }
            default:
                break;
        }
    }
}