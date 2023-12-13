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
        //AppEnvironment.Initialize();
        var optionsParser = Parser.Default.ParseArguments<ConfigOptions>(args);
        var configurationResult = ConfigOptionParser.SetConfigOptions(optionsParser);

        if (configurationResult.ResultType == ConfigurationResultType.NotParsed)
        {
            Environment.Exit(1);
        }

        if (configurationResult.ResultType == ConfigurationResultType.Failure)
        {
            Console.WriteLine(configurationResult.Message);
            Environment.Exit(2);
        }

        AppEnvironment.Initialize(configurationResult.ConfiguredValues);
        try
        {
            AppEnvironment.MainMenu.Navigate();
        }
        catch (Exception exc)
        {
            new MessageOverlay(exc.Message, MessageType.Error).PrintMessage();
        }
        finally
        {
            AppEnvironment.Shutdown();
        }
    }
}