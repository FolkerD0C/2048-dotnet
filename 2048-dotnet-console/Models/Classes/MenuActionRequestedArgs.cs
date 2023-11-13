using Game2048.ConsoleFrontend.Helpers.Enums;
using Game2048.ConsoleFrontend.Resources.Menus;
using System;

namespace Game2048.ConsoleFrontend.Models;

public class MenuActionRequestedArgs : IMenuActionRequestedArgs
{
    readonly MenuActionType menuActionType;
    public MenuActionType ActionType => menuActionType;

    readonly IMenu subMenu;
    public IMenu SubMenu => subMenu;

    readonly Action action;
    public Action Action => action;

    readonly Action<string> actionWithStringArg;
    public Action<string> ActionWithStringArg => actionWithStringArg;

    readonly string actionStringArg;
    public string ActionStringArg => actionStringArg;

    MenuActionRequestedArgs(MenuActionType menuActionType, IMenu subMenu, Action action, Action<string> actionWithStringArg, string actionStringArg)
    {
        this.menuActionType = menuActionType;
        this.subMenu = subMenu;
        this.action = action;
        this.actionWithStringArg = actionWithStringArg;
        this.actionStringArg = actionStringArg;
    }

    public MenuActionRequestedArgs(IMenu subMenu) : this(MenuActionType.SubMenu, subMenu, () => { }, (s) => { }, "")
    { }

    public MenuActionRequestedArgs(Action action) : this(MenuActionType.Action, new Menu(), action, (s) => { }, "")
    { }

    public MenuActionRequestedArgs(Action<string> actionWithStringArg, string actionStringArg) : this(MenuActionType.ActionWithStringArg, new Menu(), () => { }, actionWithStringArg, actionStringArg)
    { }
}
