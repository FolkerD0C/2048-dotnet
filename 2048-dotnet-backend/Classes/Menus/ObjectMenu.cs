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

    MenuResult result;

    public ObjectMenu(string displayName, Action<object[]> action, params object[] args)
    {
        this.displayName = displayName;
        this.action = action;
        this.args = args;
        this.result = MenuResult.Obj;
    }

    public void SetResult(MenuResult result)
    {
        this.result = result;
    }

    public void AddArgs(object[] args)
    {
        this.args = this.args.Concat(args).ToArray();
    }

    public MenuResult MenuAction()
    {
        Display.NewLayout();
        action?.Invoke(args);
        Display.PreviousLayout();
        return this.result;
    }
}
