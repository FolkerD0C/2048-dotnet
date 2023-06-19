using Game2048.Interfaces;
using Game2048.Static;

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

    MenuResult result;

    protected MenuResult Result
    {
        get
        {
            return result;
        }
        private set
        {
            result = value;
        }
    }

    List<MenuResult> acceptedResults;

    protected List<MenuResult> AcceptedResults
    {
        get
        {
            return acceptedResults;
        }
        private set
        {
            acceptedResults = value;
        }
    }

    int cursorPosition;

    protected int CursorPosition
    {
        get
        {
            return cursorPosition;
        }
        set
        {
            cursorPosition = value;
        }
    }

    public NavigationMenu(string displayName, List<IMenu> subMenus)
    {
        this.displayName = displayName;
        this.subMenus = subMenus;
        result = MenuResult.OK;
        acceptedResults = new List<MenuResult>()
        {
            MenuResult.OK
        };
    }

    public virtual MenuResult MenuAction()
    {
        Display.NewLayout();
        Navigate();
        Display.PreviousLayout();
        return Result;
    }

    public void SetReturnValue(MenuResult result)
    {
        Result = result;
    }

    public void AddAcceptedResult(MenuResult result)
    {
        if (!AcceptedResults.Contains(result))
        {
            AcceptedResults.Add(result);
        }
    }

    protected virtual void Navigate()
    {
        MenuResult navigation = MenuResult.OK;
        Func<int, int> moveCursor = x =>
            x < 0 ? SubMenus.Count - 1 :
            x >= SubMenus.Count ? 0 : x;
        int prevPos = 0;
        DrawMenu();
        while (AcceptedResults.Contains(navigation))
        {
            SelectionChanged(prevPos);
            switch(HandleKeyboardInput())
            {
                case InputAction.Up:
                    {
                        prevPos = CursorPosition;
                        CursorPosition = moveCursor(CursorPosition - 1);
                        break;
                    }
                case InputAction.Down:
                    {
                        prevPos = CursorPosition;
                        CursorPosition = moveCursor(CursorPosition + 1);
                        break;
                    }
                case InputAction.Activate:
                    {
                        navigation = SubMenus[CursorPosition].MenuAction();
                        break;
                    }
            }
        }
    }

    protected void SelectionChanged(int prevPos)
    {
        Display.PrintText(SubMenus[prevPos].DisplayName, 0, prevPos + MenuPosition,
                ConsoleColor.White, ConsoleColor.Black);
        Display.PrintText(SubMenus[CursorPosition].DisplayName, 0, CursorPosition + MenuPosition,
                ConsoleColor.White, ConsoleColor.Red);
    }

    protected void DrawMenu()
    {
        for (int i = 0; i < SubMenus.Count; i++)
        {
            Display.PrintText(SubMenus[i].DisplayName, 0, i + MenuPosition,
                    ConsoleColor.White, ConsoleColor.Black);
        }
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

