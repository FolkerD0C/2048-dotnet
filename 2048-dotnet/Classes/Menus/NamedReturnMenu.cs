using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class NamedReturnMenu : ReturnMenu
{
    public NamedReturnMenu(string displayName, MenuResult returnValue) :
        base(returnValue)
    {
        DisplayName = displayName;
    }

    public override MenuResult MenuAction()
    {
        return ReturnValue;
    }
}
