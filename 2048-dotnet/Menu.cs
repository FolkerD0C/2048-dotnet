namespace Game2048;

internal class Menu
{
    enum InputAction
    {
        Up,
        Down,
        Activate
    }

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

    public event Func<bool> MenuAction;

    public Menu (string displayName, bool stepBackItem = false)
    {
        this.displayName = displayName;
        subMenus = new List<Menu>();
        if (stepBackItem)
        {
            MenuAction += PreviosMenu;
        }
    }

    public void AddSubMenu(Menu menu)
    {
        SubMenus.Add(menu);
    }

    public bool FireAction()
    {
        if (SubMenus.Count > 0)
        {
            MenuAction += Navigate;
        }
        bool result = MenuAction();
        if (SubMenus.Count > 0)
        {
            MenuAction -= Navigate;
        }
        return result;
    }

    public bool Navigate()
    {
        bool navigation = true;
        int cursorPosition = 0;
        Func<int, int> moveCursor = x =>
            x < 0 ? SubMenus.Count - 1 :
            x >= SubMenus.Count ? 0 : x;
        while (navigation)
        {
            switch(HandleKeyboardInput())
            {
                case InputAction.Up:
                    {
                        cursorPosition = moveCursor(cursorPosition - 1);
                        break;
                    }
                case InputAction.Down:
                    {
                        cursorPosition = moveCursor(cursorPosition + 1);
                        break;
                    }
                case InputAction.Activate:
                    {
                        navigation = SubMenus[cursorPosition].FireAction();
                        break;
                    }
            }
        }
        return true;
    }

    void DrawMenu(int cursorPosition)
    {
        Console.Clear();
        for (int i = 0; i < SubMenus.Count; i++)
        {
            if (i == cursorPosition)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(SubMenus[i].DisplayName);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine(SubMenus[i].DisplayName);
            }
        }
    }

    InputAction HandleKeyboardInput()
    {
        while (true)
        {
            ConsoleKey input = Console.ReadKey(false).Key;
            switch (input)
            {
                case ConsoleKey.W: case ConsoleKey.UpArrow:
                    return InputAction.Up;
                case ConsoleKey.S: case ConsoleKey.DownArrow:
                    return InputAction.Down;
                case ConsoleKey.Enter: case ConsoleKey.Spacebar:
                    return InputAction.Activate;
                default:
                    break;
            }
        }
        throw new ArgumentException();
    }

    bool PreviosMenu()
    {
        return false;
    }
}
