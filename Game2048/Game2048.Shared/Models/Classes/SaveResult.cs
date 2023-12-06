using Game2048.Shared.Enums;

namespace Game2048.Shared.Models;

/// <summary>
/// A struct that represents the result of a save action.
/// </summary>
public record SaveResult
{
    /// <summary>
    /// The type of the result.
    /// </summary>
    public SaveResultType ResultType { get; set; }

    /// <summary>
    /// An optional message for the result that is used in case of failure.
    /// </summary>
    public string? Message { get; set; }
}
