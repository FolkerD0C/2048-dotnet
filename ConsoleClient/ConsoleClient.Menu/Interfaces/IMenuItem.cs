using ConsoleClient.Menu.Enums;

namespace ConsoleClient.Menu;

/// <summary>
/// Represents a manager for the actions that can happen during a selection at the navigation of an <see cref="IConsoleMenu"/> object.
/// </summary>
public interface IMenuItem
{
    /// <summary>
    /// The display name of the <see cref="IMenuItem"/> object.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The type of the <see cref="IMenuItem"/> object.
    /// </summary>
    MenuItemType ItemType { get; }

    /// <summary>
    /// Performs an optional action, then returns the result of that action as a <see cref="MenuItemResult"/>.
    /// </summary>
    /// <returns>The result of the selection as a <see cref="MenuItemResult"/></returns>.
    MenuItemResult Select();
}