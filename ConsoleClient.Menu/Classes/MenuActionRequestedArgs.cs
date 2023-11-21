using ConsoleClient.Menu.Enums;
using System;

namespace ConsoleClient.Menu;

public class MenuActionRequestedArgs : IMenuActionRequestedArgs
{
    readonly MenuActionType menuActionType;
    public MenuActionType ActionType => menuActionType;

    readonly IConsoleMenu subMenu;
    public IConsoleMenu SubMenu => subMenu;

    readonly Action action;
    public Action Action => action;

    readonly Action<string> actionWithStringArg;
    public Action<string> ActionWithStringArg => actionWithStringArg;

    readonly string actionStringArg;
    public string ActionStringArg => actionStringArg;

    MenuActionRequestedArgs(MenuActionType menuActionType, IConsoleMenu subMenu, Action action, Action<string> actionWithStringArg, string actionStringArg)
    {
        this.menuActionType = menuActionType;
        this.subMenu = subMenu;
        this.action = action;
        this.actionWithStringArg = actionWithStringArg;
        this.actionStringArg = actionStringArg;
    }

    public MenuActionRequestedArgs(IConsoleMenu subMenu) : this(MenuActionType.SubMenu, subMenu, () => { }, (s) => { }, "")
    { }

    public MenuActionRequestedArgs(Action action) : this(MenuActionType.Action, new ConsoleMenu(), action, (s) => { }, "")
    { }

    public MenuActionRequestedArgs(Action<string> actionWithStringArg, string actionStringArg) : this(MenuActionType.ActionWithStringArg, new ConsoleMenu(), () => { }, actionWithStringArg, actionStringArg)
    { }
}
