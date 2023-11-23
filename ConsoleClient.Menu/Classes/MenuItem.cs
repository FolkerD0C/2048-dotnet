using ConsoleClient.Menu.Enums;

namespace ConsoleClient.Menu;

public class MenuItem : IMenuItem
{
    readonly string name;
    public string Name => name;

    readonly MenuItemType itemType;
    public MenuItemType ItemType => itemType;

    MenuItemResult menuResult;

    readonly IMenuActionRequestedArgs actionRequestedArgs;
    public IMenuActionRequestedArgs ActionRequestedArgs => actionRequestedArgs;

    MenuItem(string name, MenuItemType itemType, MenuItemResult menuResult, IMenuActionRequestedArgs actionRequestedArgs)
    {
        this.name = name;
        this.itemType = itemType;
        this.menuResult = menuResult;
        this.actionRequestedArgs = actionRequestedArgs;
    }

    public MenuItem(string name) : this(name, MenuItemType.Back, MenuItemResult.Back, new MenuActionRequestedArgs(() => { }))
    { }

    public MenuItem(string name, MenuItemResult menuResult) : this(name, MenuItemType.YesNo, menuResult, new MenuActionRequestedArgs(() => { }))
    { }

    public MenuItem(string name, IMenuActionRequestedArgs actionRequestedArgs) : this(name, MenuItemType.Action, MenuItemResult.Ok, actionRequestedArgs)
    { }

    public MenuItem(string name, MenuItemResult menuResult, IMenuActionRequestedArgs actionRequestedArgs) : this(name, MenuItemType.Action, menuResult, actionRequestedArgs)
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
