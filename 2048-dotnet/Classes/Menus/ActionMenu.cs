using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

class ActionMenu : IMenu
{
    string displayName;
    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    Action<object?> action;

    object? obj;

    public ActionMenu(string displayName, Action<object?> action, object? obj)
    {
        this.displayName = displayName;
        this.action = action;
        this.obj = obj;
    }

    public MenuResult MenuAction()
    {
        action?.Invoke(obj);
        return MenuResult.OK;
    }
}
