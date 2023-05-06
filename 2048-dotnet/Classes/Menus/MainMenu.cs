using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class MainMenu : NavigationMenu
{
    public MainMenu(string displayName, List<IMenu> subMenus) : base(displayName, subMenus)
    {
        Navigate();
    }
}
