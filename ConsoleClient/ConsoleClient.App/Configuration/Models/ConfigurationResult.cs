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
    internal ConfigurationResultType Result { get; set; }

    /// <summary>
    /// An optional message in case of <see cref="ConfigurationResultType.Failure"/>.
    /// </summary>
    internal string? Message { get; set; }
}
