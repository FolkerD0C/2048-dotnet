namespace ConsoleClient.Menu.Enums;

/// <summary>
/// An enum that represents the type of an <see cref="IMenuItem"/> action.
/// </summary>
public enum MenuItemActionType
{
    SubMenu,
    Action,
    ActionWithStringParam,
    Unknown
}