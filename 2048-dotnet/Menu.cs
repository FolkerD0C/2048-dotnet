namespace Game2048;

internal class Menu
{
    List<Menu> subMenus;
    public List<Menu> SubMenus
    {
        get
        {
            return subMenus;
        }
        private set
        {
            subMenus = value;
        }
    }

    string displayName;
    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    public event Action MenuAction;

    public Menu (string displayName)
    {
        this.displayName = displayName;
        subMenus = new List<Menu>();
    }

    public void AddSubMenu(Menu menu)
    {
        SubMenus.Add(menu);
    }

    void Asd()
    {
        Console.WriteLine("It worked");
    }
}
