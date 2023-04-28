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

    public void FireAction()
    {
        MenuAction?.Invoke();
    }

    public void Navigate()
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
                        SubMenus[cursorPosition].FireAction();
                        break;
                    }
            }
        }
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
}
