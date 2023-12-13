using _2048ish.Base.Models;
using ConsoleClient.App.Configuration.Enums;

namespace ConsoleClient.App.Configuration.Models;

/// <summary>
/// A class that represents the result of a game configuration action.
/// </summary>
internal record ConfigurationResult
{
    /// <summary>
    /// The type of the result.
    /// </summary>
    internal ConfigurationResultType ResultType { get; set; }

    /// <summary>
    /// An optional message in case of <see cref="ConfigurationResultType.Failure"/>.
    /// </summary>
    internal string? Message { get; set; }

    /// <summary>
    /// The configuration passed on to new games.
    /// </summary>
    internal NewGameConfiguration ConfiguredValues { get; set; } = new NewGameConfiguration();
}
