using ConsoleClient.Menu.Enums;
using System;

namespace ConsoleClient.Menu;

public class MenuItemActionRequestedArgs : IMenuItemActionRequestedArgs
{
    readonly MenuItemActionType menuActionType;
    public MenuItemActionType ActionType => menuActionType;

    readonly IConsoleMenu subMenu;
    public IConsoleMenu SubMenu => subMenu;

    readonly Action action;
    public Action Action => action;

    readonly Action<string> actionWithStringParam;
    public Action<string> ActionWithStringParam => actionWithStringParam;

    readonly string actionStringParam;
    public string ActionStringParam => actionStringParam;

    /// <summary>
    /// Creates a new instance of the <see cref="MenuItemActionRequestedArgs"/> class.
    /// </summary>
    /// <param name="menuActionType">The type of the action.</param>
    /// <param name="subMenu">An optional submenu. Relates to <see cref="MenuItemActionType.SubMenu"/>.</param>
    /// <param name="action">An optional action. Relates to <see cref="MenuItemActionType.Action"/>.</param>
    /// <param name="actionWithStringParam">An optional action with a string parameter. Relates to <see cref="MenuItemActionType.ActionWithStringParam"/>.</param>
    /// <param name="actionStringParam">The string parameter for the <paramref name="actionWithStringParam"/> action.</param>
    MenuItemActionRequestedArgs(MenuItemActionType menuActionType, IConsoleMenu subMenu, Action action, Action<string> actionWithStringParam, string actionStringParam)
    {
        this.menuActionType = menuActionType;
        this.subMenu = subMenu;
        this.action = action;
        this.actionWithStringParam = actionWithStringParam;
        this.actionStringParam = actionStringParam;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MenuItemActionRequestedArgs"/> class that holds an <see cref="IConsoleMenu"/> as a submenu.
    /// <see cref="ActionType"/> will be <see cref="MenuItemActionType.SubMenu"/>.
    /// </summary>
    /// <param name="subMenu">The submenu for the <see cref="SubMenu"/> property.</param>
    public MenuItemActionRequestedArgs(IConsoleMenu subMenu) : this(MenuItemActionType.SubMenu, subMenu, () => { }, (s) => { }, "")
    { }

    /// <summary>
    /// Creates a new instance of the <see cref="MenuItemActionRequestedArgs"/> class that holds an <see cref="System.Action"/>.
    /// <see cref="ActionType"/> will be <see cref="MenuItemActionType.Action"/>.
    /// </summary>
    /// <param name="action">The action for the <see cref="Action"/> property.</param>
    public MenuItemActionRequestedArgs(Action action) : this(MenuItemActionType.Action, new ConsoleMenu(), action, (s) => { }, "")
    { }

    /// <summary>
    /// Creates a new instance of the <see cref="MenuItemActionRequestedArgs"/> class that holds an <see cref="Action{string}"/>.
    /// <see cref="ActionType"/> will bew <see cref="MenuItemActionType.ActionWithStringParam"/>.
    /// </summary>
    /// <param name="actionWithStringParam">The action for the <see cref="ActionWithStringParam"/> property.</param>
    /// <param name="actionStringParam">The string parameter for the <see cref="ActionStringParam"/> property.</param>
    public MenuItemActionRequestedArgs(Action<string> actionWithStringParam, string actionStringParam) : this(MenuItemActionType.ActionWithStringParam, new ConsoleMenu(), () => { }, actionWithStringParam, actionStringParam)
    { }
}
