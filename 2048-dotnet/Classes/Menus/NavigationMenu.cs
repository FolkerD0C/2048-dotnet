using Game2048.Interfaces;

namespace Game2048.Classes.Menus;

public class NavigationMenu : IMenu
{
    protected enum InputAction
    {
        Up,
        Down,
        Activate
    }

    string displayName;
    public string DisplayName
    {
        get
        {
            return displayName;
        }
    }

    int menuPosition;
    protected int MenuPosition
    {
        get
        {
            return menuPosition;
        }
        set
        {
            menuPosition = value;
        }
    }

    List<IMenu> subMenus;
    public List<IMenu> SubMenus
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

    public NavigationMenu(string displayName, List<IMenu> subMenus)
    {
        this.displayName = displayName;
        this.subMenus = subMenus;
    }

    public MenuResult MenuAction()
    {
        return Navigate();
    }

    protected virtual MenuResult Navigate()
    {
        MenuResult navigation = MenuResult.OK;
        int cursorPosition = 0;
        Func<int, int> moveCursor = x =>
            x < 0 ? SubMenus.Count - 1 :
            x >= SubMenus.Count ? 0 : x;
        while (navigation == MenuResult.OK)
        {
            DrawMenu(cursorPosition);
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
                        navigation = SubMenus[cursorPosition].MenuAction();
                        break;
                    }
            }
        }
        ClearDisplay();
        return MenuResult.OK;
    }

    protected void DrawMenu(int cursorPosition)
    {
        ClearDisplay();
        for (int i = 0; i < SubMenus.Count; i++)
        {
            if (i == cursorPosition)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(SubMenus[i].DisplayName);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.WriteLine(SubMenus[i].DisplayName);
            }
        }
    }

    protected void ClearDisplay()
    {
        Console.SetCursorPosition(0, menuPosition);
        for (int i = menuPosition; i < Console.BufferHeight; i++)
        {
            Console.WriteLine(new string(' ', Console.BufferWidth));
        }
        Console.SetCursorPosition(0, menuPosition);
    }

    protected virtual InputAction HandleKeyboardInput()
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
}

