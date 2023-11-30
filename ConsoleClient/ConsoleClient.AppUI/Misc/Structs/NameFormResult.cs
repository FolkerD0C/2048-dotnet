using ConsoleClient.AppUI.Enums;

namespace ConsoleClient.AppUI.Misc;

/// <summary>
/// A struct that represents the result of an <see cref="INameForm"/> object action.
/// </summary>
public struct NameFormResult
{
    /// <summary>
    /// The type of the <see cref="INameForm"/> object action's result.
    /// </summary>
    public NameFormResultType ResultType { get; set; }

    /// <summary>
    /// The new name of the player.
    /// </summary>
    public string Name { get; set; }
}
