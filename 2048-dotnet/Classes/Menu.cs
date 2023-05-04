namespace Game2048.Classes;

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

    int menuPosition = 0;

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

    public void SetDisplayPosition(int position)
    {
        menuPosition =
            position < 0 ? 0 :
            position > Console.BufferHeight - 1 - SubMenus.Count ?
                Console.BufferHeight - 1 - SubMenus.Count :
            position;
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
        ClearDisplay();
        for (int i = 0; i < SubMenus.Count; i++)
        {
            if (i == cursorPosition)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(SubMenus[i].DisplayName);
                Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine(SubMenus[i].DisplayName);
            }
        }
    }

    void ClearDisplay()
    {
        Console.SetCursorPosition(0, menuPosition);
        for (int i = menuPosition; i < Console.BufferHeight; i++)
        {
            Console.WriteLine(new string(' ', Console.BufferWidth));
        }
        Console.SetCursorPosition(0, menuPosition);
    }

    InputAction HandleKeyboardInput()
    {
        while (true)
        {
            ConsoleKey input = Console.ReadKey(true).Key;
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
        Console.ReadKey(true);
        return false;
    }
}
