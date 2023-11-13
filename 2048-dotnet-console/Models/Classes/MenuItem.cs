using Game2048.ConsoleFrontend.Helpers.Enums;

namespace Game2048.ConsoleFrontend.Models;

public class MenuItem : IMenuItem
{
    readonly MenuItemType itemType;
    public MenuItemType ItemType => itemType;

    MenuItemResult menuResult;

    readonly IMenuActionRequestedArgs actionRequestedArgs;
    public IMenuActionRequestedArgs ActionRequestedArgs => actionRequestedArgs;

    MenuItem(MenuItemType itemType, MenuItemResult menuResult, IMenuActionRequestedArgs actionRequestedArgs)
    {
        this.itemType = itemType;
        this.menuResult = menuResult;
        this.actionRequestedArgs = actionRequestedArgs;
    }

    public MenuItem() : this(MenuItemType.Back, MenuItemResult.Back, new MenuActionRequestedArgs(() => { }))
    { }

    public MenuItem(MenuItemResult menuResult) : this(MenuItemType.YesNo, menuResult, new MenuActionRequestedArgs(() => { }))
    { }

    public MenuItem(IMenuActionRequestedArgs actionRequestedArgs) : this(MenuItemType.Action, MenuItemResult.Ok, actionRequestedArgs)
    { }

    public MenuItemResult Select()
    {
        if (itemType == MenuItemType.Action)
        {
            switch (actionRequestedArgs.ActionType)
            {
                case MenuActionType.SubMenu:
                    {
                        menuResult = actionRequestedArgs.SubMenu.Navigate();
                        break;
                    }
                case MenuActionType.Action:
                    {
                        actionRequestedArgs.Action?.Invoke();
                        break;
                    }
                case MenuActionType.ActionWithStringArg:
                    {
                        actionRequestedArgs.ActionWithStringArg?.Invoke(actionRequestedArgs.ActionStringArg);
                        break;
                    }
                default:
                    break;
            }
        }
        return menuResult;
    }
}
