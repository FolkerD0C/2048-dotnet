using Game2048.Interfaces;
using Game2048.Static;

namespace Game2048.Classes;

class ObjectMenu : IMenu
{
    string displayName;
    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    Action<object[]> action;

    object[] args;

    public ObjectMenu(string displayName, Action<object[]> action, params object[] args)
    {
        this.displayName = displayName;
        this.action = action;
        this.args = args;
    }

    public MenuResult MenuAction()
    {
        Display.NewLayout();
        action?.Invoke(args);
        Display.PreviousLayout();
        return MenuResult.Obj;
    }
}
