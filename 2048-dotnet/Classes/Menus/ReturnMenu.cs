using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class ReturnMenu : IMenu
{
    string displayName;
    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    MenuResult returnValue;

    public ReturnMenu(MenuResult returnValue)
    {
        this.displayName = returnValue.ToString();
        this.returnValue = returnValue;
    }

    public MenuResult MenuAction()
    {
        return returnValue;
    }
}
