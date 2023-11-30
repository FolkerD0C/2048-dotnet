using ConsoleClient.Menu.Enums;
using System;

namespace ConsoleClient.Menu;

/// <summary>
/// Represents a helper for the optional actions that can happen during the selection of an <see cref="IMenuItem"/>.
/// </summary>
public interface IMenuItemActionRequestedArgs
{
    /// <summary>
    /// The type of the action.
    /// </summary>
    MenuItemActionType ActionType { get; }

    /// <summary>
    /// An optional submenu. Relates to <see cref="MenuItemActionType.SubMenu"/>.
    /// </summary>
    IConsoleMenu SubMenu { get; }

    /// <summary>
    /// An optional action. Relates to <see cref="MenuItemActionType.Action"/>.
    /// </summary>
    Action Action { get; }

    /// <summary>
    /// An optional action with a string parameter. Relates to <see cref="MenuItemActionType.ActionWithStringParam"/>.
    /// </summary>
    Action<string> ActionWithStringParam { get; }

    /// <summary>
    /// The string parameter for the <see cref="ActionWithStringParam"/> action.
    /// </summary>
    string ActionStringParam { get; }
}