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
        protected set
        {
            displayName = value;
        }
    }

    MenuResult returnValue;
    protected MenuResult ReturnValue
    {
        get
        {
            return returnValue;
        }
    }

    public ReturnMenu(MenuResult returnValue)
    {
        this.displayName = returnValue.ToString();
        this.returnValue = returnValue;
    }

    public virtual MenuResult MenuAction()
    {
        return returnValue;
    }
}
