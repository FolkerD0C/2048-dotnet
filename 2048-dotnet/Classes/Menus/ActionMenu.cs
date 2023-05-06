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

    Action action;

    public ActionMenu(string displayName, Action action)
    {
        this.displayName = displayName;
        this.action = action;
    }

    public MenuResult MenuAction()
    {
        action?.Invoke();
        return MenuResult.OK;
    }
}
