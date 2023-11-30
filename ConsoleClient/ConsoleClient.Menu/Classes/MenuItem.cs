using ConsoleClient.Menu.Enums;

namespace ConsoleClient.Menu;

/// <summary>
/// A class that represents a manager for the actions that can happen
/// during a selection at the navigation of an <see cref="IConsoleMenu"/> object.
/// </summary>
public class MenuItem : IMenuItem
{
    readonly string name;
    public string Name => name;

    readonly MenuItemType itemType;
    public MenuItemType ItemType => itemType;

    /// <summary>
    /// The result of the selection.
    /// </summary>
    MenuItemResult menuItemResult;

    /// <summary>
    /// An optional object that holds information about the action that can happen during the selection of this <see cref="MenuItem"/> object.
    /// </summary>
    readonly IMenuItemActionRequestedArgs? actionRequestedArgs;

    /// <summary>
    /// Creates a nwe instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="name">The display name of the <see cref="IMenuItem"/> object.</param>
    /// <param name="itemType">The type of the <see cref="IMenuItem"/> object.</param>
    /// <param name="menuItemResult">The result of the selection.</param>
    /// <param name="actionRequestedArgs">An optional object that holds information about the action that can happen during the selection of this <see cref="MenuItem"/> object.</param>
    MenuItem(string name, MenuItemType itemType, MenuItemResult menuItemResult, IMenuItemActionRequestedArgs? actionRequestedArgs)
    {
        this.name = name;
        this.itemType = itemType;
        this.menuItemResult = menuItemResult;
        this.actionRequestedArgs = actionRequestedArgs;
    }

    /// <summary>
    /// Creates a nwe instance of the <see cref="MenuItem"/> class that will be a
    /// <see cref="MenuItemType.Back"/> type and returns <see cref="MenuItemResult.Back"/> as a result upon selection.
    /// </summary>
    /// <param name="name">The display name of the <see cref="IMenuItem"/> object.</param>
    public MenuItem(string name) : this(name, MenuItemType.Back, MenuItemResult.Back, default)
    { }

    /// <summary>
    /// Creates a nwe instance of the <see cref="MenuItem"/> class that will be a
    /// <see cref="MenuItemType.YesNo"/> type.
    /// </summary>
    /// <param name="name">The display name of the <see cref="IMenuItem"/> object.</param>
    /// <param name="menuItemResult">The result of the selection.
    /// Should be <see cref="MenuItemResult.Yes"/> or <see cref="MenuItemResult.No"/>.</param>
    public MenuItem(string name, MenuItemResult menuItemResult) : this(name, MenuItemType.YesNo, menuItemResult, default)
    { }

    /// <summary>
    /// Creates a nwe instance of the <see cref="MenuItem"/> class that will be a
    /// <see cref="MenuItemType.Action"/> type.
    /// </summary>
    /// <param name="name">The display name of the <see cref="IMenuItem"/> object.</param>
    /// <param name="actionRequestedArgs">Holds information about the action that can
    /// happen during the selection of this <see cref="MenuItem"/> object.</param>
    public MenuItem(string name, IMenuItemActionRequestedArgs actionRequestedArgs) : this(name, MenuItemType.Action, MenuItemResult.Ok, actionRequestedArgs)
    { }

    /// <summary>
    /// Creates a nwe instance of the <see cref="MenuItem"/> class that will be a
    /// <see cref="MenuItemType.Action"/> type.
    /// </summary>
    /// <param name="name">The display name of the <see cref="IMenuItem"/> object.</param>
    /// <param name="menuResult">The result of the selection.</param>
    /// <param name="actionRequestedArgs">Holds information about the action that can
    /// happen during the selection of this <see cref="MenuItem"/> object.</param>
    public MenuItem(string name, MenuItemResult menuResult, IMenuItemActionRequestedArgs actionRequestedArgs) : this(name, MenuItemType.Action, menuResult, actionRequestedArgs)
    { }

    public MenuItemResult Select()
    {
        if (itemType == MenuItemType.Action)
        {
            switch (actionRequestedArgs?.ActionType)
            {
                case MenuItemActionType.SubMenu:
                    {
                        menuItemResult = actionRequestedArgs.SubMenu.Navigate();
                        break;
                    }
                case MenuItemActionType.Action:
                    {
                        actionRequestedArgs.Action?.Invoke();
                        break;
                    }
                case MenuItemActionType.ActionWithStringParam:
                    {
                        actionRequestedArgs.ActionWithStringParam?.Invoke(actionRequestedArgs.ActionStringParam);
                        break;
                    }
                default:
                    break;
            }
        }
        return menuItemResult;
    }
}
